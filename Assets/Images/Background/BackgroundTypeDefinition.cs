using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BackgroundDecorationEntry
{
    [SerializeField]
    private Sprite sprite;

    [SerializeField]
    [Min(0.001f)]
    private float weight = 1f;

    [SerializeField]
    private bool allowMirroring = true;

    [SerializeField]
    private bool randomizeMirror = true;

    [SerializeField]
    private bool mirrored = false;

    public Sprite Sprite => sprite;
    public float Weight => Mathf.Max(0.001f, weight);
    public bool AllowMirroring => allowMirroring;
    public bool RandomizeMirror => randomizeMirror;
    public bool Mirrored => mirrored;
}

[System.Serializable]
public class BackgroundConnectionRule
{
    [SerializeField]
    private BackgroundTypeDefinition targetType;

    [SerializeField]
    private bool requiresTransition = true;

    [SerializeField]
    private bool allowDirectIfTransitionMissing = false;

    public BackgroundTypeDefinition TargetType => targetType;
    public bool RequiresTransition => requiresTransition;
    public bool AllowDirectIfTransitionMissing => allowDirectIfTransitionMissing;
}

public class BackgroundTypeDefinition : ScriptableObject
{
    [SerializeField]
    [Min(0.001f)]
    private float baseWeight = 1f;

    [Header("Background Sprites")]
    [SerializeField]
    private List<Sprite> sprites = new List<Sprite>();

    [Header("Decorations")]
    [SerializeField]
    [Range(0f, 1f)]
    private float decorationSpawnChance = 0.35f;

    [SerializeField]
    private List<BackgroundDecorationEntry> decorations = new List<BackgroundDecorationEntry>();

    [Header("Adjacency Rules")]
    [SerializeField]
    private List<BackgroundConnectionRule> allowedNextTypes = new List<BackgroundConnectionRule>();

    public float BaseWeight => Mathf.Max(0.001f, baseWeight);
    public List<Sprite> Sprites => sprites;
    public float DecorationSpawnChance => Mathf.Clamp01(decorationSpawnChance);
    public List<BackgroundDecorationEntry> Decorations => decorations;
    public List<BackgroundConnectionRule> AllowedNextTypes => allowedNextTypes;

    private void OnValidate()
    {
        baseWeight = Mathf.Max(0.001f, baseWeight);
        decorationSpawnChance = Mathf.Clamp01(decorationSpawnChance);

        if (sprites == null)
            sprites = new List<Sprite>();

        if (decorations == null)
            decorations = new List<BackgroundDecorationEntry>();

        if (allowedNextTypes == null)
            allowedNextTypes = new List<BackgroundConnectionRule>();
    }
}
