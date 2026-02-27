using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class BackgroundOrchestrator : MonoBehaviour
{
    [Header("Scroller")]
    [SerializeField]
    private BackgroundScroller scroller;

    [Header("Panels")]
    [SerializeField]
    private List<BackgroundPanelVisual> panels = new List<BackgroundPanelVisual>();

    [Header("Content")]
    [SerializeField]
    private BackgroundLibrary library;

    [Header("Runtime Stage")]
    [SerializeField]
    private int currentStageIndex = 0;

    [Header("Initialization")]
    [SerializeField]
    private bool initializeOnStart = true;

    [SerializeField]
    private bool autoFindPanelsIfEmpty = true;

    [Header("Debug")]
    [SerializeField]
    private bool verboseLogging = false;

    private readonly Queue<BackgroundTypeDefinition> recentNormalHistory =
        new Queue<BackgroundTypeDefinition>();

    private readonly List<BackgroundPanelVisual> orderedPanels = new List<BackgroundPanelVisual>();

    private bool isInitialized;
    private BackgroundTypeDefinition lastNormalType;
    private BackgroundTypeDefinition pendingDestinationType;

    private class EmittedSegment
    {
        public bool IsTransition;
        public BackgroundTypeDefinition Type;
        public BackgroundTypeDefinition TransitionFromType;
        public BackgroundTypeDefinition TransitionToType;
        public Sprite BackgroundSprite;
        public Sprite DecorationSprite;
        public bool DecorationMirrored;
    }

    private struct DecorationSelection
    {
        public Sprite Sprite;
        public bool Mirrored;
    }

    private void Reset()
    {
        if (scroller == null)
        {
            scroller = GetComponentInChildren<BackgroundScroller>(true);
        }

        if (autoFindPanelsIfEmpty && (panels == null || panels.Count == 0))
        {
            panels = GetComponentsInChildren<BackgroundPanelVisual>(true).ToList();
        }
    }

    private void Awake()
    {
        if (scroller == null)
        {
            scroller = GetComponentInChildren<BackgroundScroller>(true);
        }

        if (autoFindPanelsIfEmpty && (panels == null || panels.Count == 0))
        {
            panels = GetComponentsInChildren<BackgroundPanelVisual>(true).ToList();
        }
    }

    private void OnEnable()
    {
        SubscribeToScroller();
    }

    private void OnDisable()
    {
        UnsubscribeFromScroller();
    }

    private void Start()
    {
        if (initializeOnStart)
        {
            Initialize();
        }
    }

    public void Initialize()
    {
        if (scroller == null)
        {
            Debug.LogError("[BackgroundOrchestrator] No BackgroundScroller assigned.");
            return;
        }

        if (panels == null || panels.Count == 0)
        {
            Debug.LogError("[BackgroundOrchestrator] No panels assigned.");
            return;
        }

        if (library == null || library.Stages == null || library.Stages.Count == 0)
        {
            Debug.LogError(
                "[BackgroundOrchestrator] No BackgroundLibrary or no stages configured."
            );
            return;
        }

        UnsubscribeFromScroller();
        SubscribeToScroller();

        recentNormalHistory.Clear();
        lastNormalType = null;
        pendingDestinationType = null;

        orderedPanels.Clear();
        orderedPanels.AddRange(panels.Where(p => p != null).OrderBy(p => p.transform.position.y));

        if (orderedPanels.Count == 0)
        {
            Debug.LogError("[BackgroundOrchestrator] No valid panels found.");
            return;
        }

        foreach (BackgroundPanelVisual panel in orderedPanels)
        {
            AssignNextSegmentToPanel(panel);
        }

        isInitialized = true;
    }

    public void SetStageIndex(int newStageIndex)
    {
        if (library == null || library.Stages == null || library.Stages.Count == 0)
            return;

        currentStageIndex = Mathf.Clamp(newStageIndex, 0, library.Stages.Count - 1);

        if (verboseLogging)
        {
            Debug.Log(
                $"[BackgroundOrchestrator] Stage changed to {currentStageIndex} ({GetCurrentStage()?.name})."
            );
        }
    }

    public int GetStageIndex()
    {
        return currentStageIndex;
    }

    private void SubscribeToScroller()
    {
        if (scroller == null)
            return;

        scroller.PanelWrapped -= HandlePanelWrapped;
        scroller.PanelWrapped += HandlePanelWrapped;
    }

    private void UnsubscribeFromScroller()
    {
        if (scroller == null)
            return;

        scroller.PanelWrapped -= HandlePanelWrapped;
    }

    private void HandlePanelWrapped(BackgroundPanelVisual wrappedPanel)
    {
        if (!isInitialized)
        {
            Initialize();
            return;
        }

        if (wrappedPanel == null)
        {
            Debug.LogWarning("[BackgroundOrchestrator] Wrapped panel was null.");
            return;
        }

        AssignNextSegmentToPanel(wrappedPanel);
    }

    private void AssignNextSegmentToPanel(BackgroundPanelVisual panel)
    {
        if (panel == null)
            return;

        EmittedSegment segment = EmitNextSegment();
        if (segment == null || segment.BackgroundSprite == null)
        {
            Debug.LogWarning(
                $"[BackgroundOrchestrator] Could not emit a valid segment for panel '{panel.name}'."
            );
            return;
        }

        panel.ApplyBackground(segment.BackgroundSprite);

        if (segment.IsTransition)
        {
            panel.ClearDecoration();
        }
        else
        {
            if (segment.DecorationSprite != null)
            {
                panel.ApplyDecoration(segment.DecorationSprite, segment.DecorationMirrored);
            }
            else
            {
                panel.ClearDecoration();
            }
        }

        if (verboseLogging)
        {
            if (segment.IsTransition)
            {
                Debug.Log(
                    $"[BackgroundOrchestrator] Panel '{panel.name}' <- TRANSITION {segment.TransitionFromType?.name} -> {segment.TransitionToType?.name}"
                );
            }
            else
            {
                Debug.Log(
                    $"[BackgroundOrchestrator] Panel '{panel.name}' <- TYPE '{segment.Type?.name}'"
                );
            }
        }
    }

    private EmittedSegment EmitNextSegment()
    {
        BackgroundStageDefinition stage = GetCurrentStage();
        if (stage == null)
            return null;

        if (pendingDestinationType != null)
        {
            BackgroundTypeDefinition forcedType = pendingDestinationType;
            pendingDestinationType = null;
            return BuildNormalSegment(stage, forcedType);
        }

        BackgroundTypeDefinition nextType = ChooseNextBackgroundType(stage, lastNormalType);

        if (nextType == null)
            return null;

        if (lastNormalType == null)
        {
            return BuildNormalSegment(stage, nextType);
        }

        if (nextType == lastNormalType)
        {
            return BuildNormalSegment(stage, nextType);
        }

        BackgroundConnectionRule link = FindConnectionRule(lastNormalType, nextType);

        if (link == null)
        {
            return BuildNormalSegment(stage, lastNormalType);
        }

        if (!link.RequiresTransition)
        {
            return BuildNormalSegment(stage, nextType);
        }

        Sprite transitionSprite = FindTransitionSprite(stage, lastNormalType, nextType);

        if (transitionSprite != null)
        {
            pendingDestinationType = nextType;

            return new EmittedSegment
            {
                IsTransition = true,
                TransitionFromType = lastNormalType,
                TransitionToType = nextType,
                BackgroundSprite = transitionSprite,
                DecorationSprite = null,
                DecorationMirrored = false,
            };
        }

        bool allowDirectFallback =
            link.AllowDirectIfTransitionMissing || stage.AllowDirectFallbackWhenTransitionMissing;

        if (allowDirectFallback)
        {
            return BuildNormalSegment(stage, nextType);
        }

        return BuildNormalSegment(stage, lastNormalType);
    }

    private EmittedSegment BuildNormalSegment(
        BackgroundStageDefinition stage,
        BackgroundTypeDefinition type
    )
    {
        if (type == null)
            return null;

        Sprite bgSprite = PickRandomSprite(type.Sprites);
        if (bgSprite == null)
            return null;

        Sprite decorationSprite = null;
        bool decorationMirrored = false;

        if (
            type.Decorations != null
            && type.Decorations.Count > 0
            && Random.value <= type.DecorationSpawnChance
        )
        {
            DecorationSelection decoration = PickWeightedDecoration(type.Decorations);
            decorationSprite = decoration.Sprite;
            decorationMirrored = decoration.Mirrored;
        }

        lastNormalType = type;
        PushRecentType(stage, type);

        return new EmittedSegment
        {
            IsTransition = false,
            Type = type,
            BackgroundSprite = bgSprite,
            DecorationSprite = decorationSprite,
            DecorationMirrored = decorationMirrored,
        };
    }

    private BackgroundTypeDefinition ChooseNextBackgroundType(
        BackgroundStageDefinition stage,
        BackgroundTypeDefinition currentType
    )
    {
        List<BackgroundTypeDefinition> allValidTypes = stage
            .BackgroundTypes.Where(t => t != null && t.Sprites != null && t.Sprites.Count > 0)
            .ToList();

        if (allValidTypes.Count == 0)
        {
            Debug.LogError(
                $"[BackgroundOrchestrator] Stage '{stage.name}' has no valid background types with sprites."
            );
            return null;
        }

        List<WeightedCandidate> candidates = new List<WeightedCandidate>();

        if (currentType == null)
        {
            foreach (BackgroundTypeDefinition type in allValidTypes)
            {
                candidates.Add(new WeightedCandidate(type, Mathf.Max(0.001f, type.BaseWeight)));
            }

            return WeightedPick(candidates);
        }

        if (currentType.Sprites != null && currentType.Sprites.Count > 0)
        {
            float sameWeight = Mathf.Max(
                0.001f,
                currentType.BaseWeight
                    + stage.SameTypePersistenceBonus
                    - GetRecentPenalty(stage, currentType)
            );

            candidates.Add(new WeightedCandidate(currentType, sameWeight));
        }

        if (currentType.AllowedNextTypes != null)
        {
            foreach (BackgroundConnectionRule rule in currentType.AllowedNextTypes)
            {
                if (rule == null || rule.TargetType == null)
                    continue;

                if (rule.TargetType == currentType)
                    continue;

                BackgroundTypeDefinition targetType = rule.TargetType;
                if (targetType.Sprites == null || targetType.Sprites.Count == 0)
                    continue;

                if (!stage.BackgroundTypes.Contains(targetType))
                    continue;

                float weight = Mathf.Max(
                    0.001f,
                    targetType.BaseWeight - GetRecentPenalty(stage, targetType)
                );

                candidates.Add(new WeightedCandidate(targetType, weight));
            }
        }

        if (candidates.Count == 0)
        {
            return currentType;
        }

        return WeightedPickMerged(candidates);
    }

    private float GetRecentPenalty(BackgroundStageDefinition stage, BackgroundTypeDefinition type)
    {
        if (type == null)
            return 0f;

        int count = 0;
        foreach (BackgroundTypeDefinition recent in recentNormalHistory)
        {
            if (recent == type)
            {
                count++;
            }
        }

        return count * stage.RecentPenaltyPerOccurrence;
    }

    private void PushRecentType(BackgroundStageDefinition stage, BackgroundTypeDefinition type)
    {
        if (type == null)
            return;

        recentNormalHistory.Enqueue(type);

        int max = Mathf.Max(1, stage.RecentHistorySize);
        while (recentNormalHistory.Count > max)
        {
            recentNormalHistory.Dequeue();
        }
    }

    private BackgroundConnectionRule FindConnectionRule(
        BackgroundTypeDefinition fromType,
        BackgroundTypeDefinition toType
    )
    {
        if (fromType == null || fromType.AllowedNextTypes == null)
            return null;

        return fromType.AllowedNextTypes.FirstOrDefault(r => r != null && r.TargetType == toType);
    }

    private Sprite FindTransitionSprite(
        BackgroundStageDefinition stage,
        BackgroundTypeDefinition fromType,
        BackgroundTypeDefinition toType
    )
    {
        IEnumerable<BackgroundTransitionDefinition> transitionsToSearch =
            stage.Transitions != null && stage.Transitions.Count > 0
                ? stage.Transitions
                : library.Transitions;

        foreach (BackgroundTransitionDefinition transition in transitionsToSearch)
        {
            if (transition == null || transition.TransitionSprite == null)
                continue;

            bool directMatch = transition.FromType == fromType && transition.ToType == toType;
            bool reverseMatch =
                transition.Bidirectional
                && transition.FromType == toType
                && transition.ToType == fromType;

            if (directMatch || reverseMatch)
            {
                return transition.TransitionSprite;
            }
        }

        return null;
    }

    private BackgroundStageDefinition GetCurrentStage()
    {
        if (library == null || library.Stages == null || library.Stages.Count == 0)
            return null;

        int clamped = Mathf.Clamp(currentStageIndex, 0, library.Stages.Count - 1);
        return library.Stages[clamped];
    }

    private Sprite PickRandomSprite(List<Sprite> sprites)
    {
        if (sprites == null || sprites.Count == 0)
            return null;

        List<Sprite> valid = sprites.Where(s => s != null).ToList();
        if (valid.Count == 0)
            return null;

        return valid[Random.Range(0, valid.Count)];
    }

    private DecorationSelection PickWeightedDecoration(List<BackgroundDecorationEntry> entries)
    {
        DecorationSelection result = new DecorationSelection { Sprite = null, Mirrored = false };

        if (entries == null || entries.Count == 0)
            return result;

        float total = 0f;
        foreach (BackgroundDecorationEntry entry in entries)
        {
            if (entry != null && entry.Sprite != null && entry.Weight > 0f)
            {
                total += entry.Weight;
            }
        }

        if (total <= 0f)
            return result;

        float roll = Random.value * total;
        float running = 0f;
        BackgroundDecorationEntry chosen = null;

        foreach (BackgroundDecorationEntry entry in entries)
        {
            if (entry == null || entry.Sprite == null || entry.Weight <= 0f)
                continue;

            running += entry.Weight;
            if (roll <= running)
            {
                chosen = entry;
                break;
            }
        }

        if (chosen == null)
        {
            chosen = entries.LastOrDefault(e => e != null && e.Sprite != null);
        }

        if (chosen == null)
            return result;

        result.Sprite = chosen.Sprite;

        if (!chosen.AllowMirroring)
        {
            result.Mirrored = false;
        }
        else if (chosen.RandomizeMirror)
        {
            result.Mirrored = Random.value > 0.5f;
        }
        else
        {
            result.Mirrored = chosen.Mirrored;
        }

        return result;
    }

    private BackgroundTypeDefinition WeightedPick(List<WeightedCandidate> candidates)
    {
        if (candidates == null || candidates.Count == 0)
            return null;

        float total = candidates.Sum(c => c.Weight);
        if (total <= 0f)
            return candidates[0].Type;

        float roll = Random.value * total;
        float running = 0f;

        foreach (WeightedCandidate candidate in candidates)
        {
            running += candidate.Weight;
            if (roll <= running)
            {
                return candidate.Type;
            }
        }

        return candidates[candidates.Count - 1].Type;
    }

    private BackgroundTypeDefinition WeightedPickMerged(List<WeightedCandidate> candidates)
    {
        Dictionary<BackgroundTypeDefinition, float> merged =
            new Dictionary<BackgroundTypeDefinition, float>();

        foreach (WeightedCandidate candidate in candidates)
        {
            if (candidate.Type == null || candidate.Weight <= 0f)
                continue;

            if (merged.ContainsKey(candidate.Type))
            {
                merged[candidate.Type] += candidate.Weight;
            }
            else
            {
                merged[candidate.Type] = candidate.Weight;
            }
        }

        List<WeightedCandidate> mergedList = new List<WeightedCandidate>();
        foreach (KeyValuePair<BackgroundTypeDefinition, float> kvp in merged)
        {
            mergedList.Add(new WeightedCandidate(kvp.Key, kvp.Value));
        }

        return WeightedPick(mergedList);
    }

    private readonly struct WeightedCandidate
    {
        public readonly BackgroundTypeDefinition Type;
        public readonly float Weight;

        public WeightedCandidate(BackgroundTypeDefinition type, float weight)
        {
            Type = type;
            Weight = weight;
        }
    }
}
