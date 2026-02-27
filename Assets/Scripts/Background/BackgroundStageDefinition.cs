using System.Collections.Generic;
using UnityEngine;

public class BackgroundStageDefinition : ScriptableObject
{
    [Header("Selection Tuning")]
    [SerializeField]
    [Min(0f)]
    private float sameTypePersistenceBonus = 2f;

    [SerializeField]
    [Min(0f)]
    private float recentPenaltyPerOccurrence = 0.5f;

    [SerializeField]
    [Min(1)]
    private int recentHistorySize = 4;

    [SerializeField]
    private bool allowDirectFallbackWhenTransitionMissing = false;

    [Header("Stage Content")]
    [SerializeField]
    private List<BackgroundTypeDefinition> backgroundTypes = new List<BackgroundTypeDefinition>();

    [SerializeField]
    private List<BackgroundTransitionDefinition> transitions =
        new List<BackgroundTransitionDefinition>();

    public float SameTypePersistenceBonus => sameTypePersistenceBonus;
    public float RecentPenaltyPerOccurrence => recentPenaltyPerOccurrence;
    public int RecentHistorySize => Mathf.Max(1, recentHistorySize);
    public bool AllowDirectFallbackWhenTransitionMissing =>
        allowDirectFallbackWhenTransitionMissing;
    public List<BackgroundTypeDefinition> BackgroundTypes => backgroundTypes;
    public List<BackgroundTransitionDefinition> Transitions => transitions;

    private void OnValidate()
    {
        sameTypePersistenceBonus = Mathf.Max(0f, sameTypePersistenceBonus);
        recentPenaltyPerOccurrence = Mathf.Max(0f, recentPenaltyPerOccurrence);
        recentHistorySize = Mathf.Max(1, recentHistorySize);

        if (backgroundTypes == null)
            backgroundTypes = new List<BackgroundTypeDefinition>();

        if (transitions == null)
            transitions = new List<BackgroundTransitionDefinition>();
    }
}
