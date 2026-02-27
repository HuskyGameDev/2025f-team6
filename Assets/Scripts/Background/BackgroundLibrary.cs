using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BackgroundLibrary", menuName = "Game/Backgrounds/Background Library")]
public class BackgroundLibrary : ScriptableObject
{
    [SerializeField]
    private List<BackgroundStageDefinition> stages = new List<BackgroundStageDefinition>();

    [SerializeField]
    private List<BackgroundTypeDefinition> backgroundTypes = new List<BackgroundTypeDefinition>();

    [SerializeField]
    private List<BackgroundTransitionDefinition> transitions =
        new List<BackgroundTransitionDefinition>();

    public IReadOnlyList<BackgroundStageDefinition> Stages => stages;
    public IReadOnlyList<BackgroundTypeDefinition> BackgroundTypes => backgroundTypes;
    public IReadOnlyList<BackgroundTransitionDefinition> Transitions => transitions;

    public List<BackgroundStageDefinition> MutableStages => stages;
    public List<BackgroundTypeDefinition> MutableBackgroundTypes => backgroundTypes;
    public List<BackgroundTransitionDefinition> MutableTransitions => transitions;

    private void OnValidate()
    {
        if (stages == null)
            stages = new List<BackgroundStageDefinition>();

        if (backgroundTypes == null)
            backgroundTypes = new List<BackgroundTypeDefinition>();

        if (transitions == null)
            transitions = new List<BackgroundTransitionDefinition>();
    }

    public void ValidateEntries(out List<string> errors, out List<string> warnings)
    {
        errors = new List<string>();
        warnings = new List<string>();

        if (stages == null || stages.Count == 0)
        {
            errors.Add("Library has no stages.");
        }

        if (backgroundTypes == null || backgroundTypes.Count == 0)
        {
            errors.Add("Library has no background types.");
        }

        if (stages != null)
        {
            foreach (BackgroundStageDefinition stage in stages)
            {
                if (stage == null)
                {
                    errors.Add("Library contains a null stage reference.");
                    continue;
                }

                if (stage.BackgroundTypes == null || stage.BackgroundTypes.Count == 0)
                {
                    errors.Add($"Stage '{stage.name}' has no background types.");
                }
            }
        }

        if (backgroundTypes != null)
        {
            foreach (BackgroundTypeDefinition type in backgroundTypes)
            {
                if (type == null)
                {
                    errors.Add("Library contains a null background type reference.");
                    continue;
                }

                if (type.Sprites == null || type.Sprites.Count == 0)
                {
                    warnings.Add($"Background type '{type.name}' has no sprites.");
                }

                if (type.BaseWeight <= 0f)
                {
                    warnings.Add($"Background type '{type.name}' has non-positive base weight.");
                }

                if (type.AllowedNextTypes != null)
                {
                    foreach (BackgroundConnectionRule rule in type.AllowedNextTypes)
                    {
                        if (rule == null)
                        {
                            warnings.Add(
                                $"Background type '{type.name}' has a null connection rule."
                            );
                            continue;
                        }

                        if (rule.TargetType == null)
                        {
                            warnings.Add(
                                $"Background type '{type.name}' has a connection with no target type."
                            );
                        }
                    }
                }
            }
        }

        if (transitions != null)
        {
            foreach (BackgroundTransitionDefinition transition in transitions)
            {
                if (transition == null)
                {
                    errors.Add("Library contains a null transition reference.");
                    continue;
                }

                if (transition.FromType == null || transition.ToType == null)
                {
                    warnings.Add($"Transition '{transition.name}' is missing from/to type.");
                }

                if (transition.TransitionSprite == null)
                {
                    warnings.Add($"Transition '{transition.name}' is missing a transition sprite.");
                }
            }
        }
    }
}
