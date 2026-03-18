using UnityEngine;

public class BackgroundTransitionDefinition : ScriptableObject
{
    [SerializeField]
    private BackgroundTypeDefinition fromType;

    [SerializeField]
    private BackgroundTypeDefinition toType;

    [SerializeField]
    private Sprite transitionSprite;

    [SerializeField]
    private bool bidirectional = true;

    public BackgroundTypeDefinition FromType => fromType;
    public BackgroundTypeDefinition ToType => toType;
    public Sprite TransitionSprite => transitionSprite;
    public bool Bidirectional => bidirectional;
}
