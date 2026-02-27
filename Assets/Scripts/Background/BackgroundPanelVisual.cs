using UnityEngine;

[DisallowMultipleComponent]
public class BackgroundPanelVisual : MonoBehaviour
{
    [Header("Optional explicit references")]
    [SerializeField]
    private SpriteRenderer backgroundSpriteRenderer;

    [Header("Decoration Child")]
    [SerializeField]
    private string decorationChildName = "BackgroundDecoration";

    private SpriteRenderer decorationSpriteRenderer;
    private Transform decorationTransform;

    public bool HasValidBackgroundTarget => backgroundSpriteRenderer != null;

    private void Awake()
    {
        AutoWire();
        EnsureDecorationTarget();
    }

    private void Reset()
    {
        AutoWire();
    }

    private void AutoWire()
    {
        if (backgroundSpriteRenderer == null)
        {
            backgroundSpriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    public void ApplyBackground(Sprite sprite)
    {
        if (backgroundSpriteRenderer != null)
        {
            backgroundSpriteRenderer.sprite = sprite;
        }
    }

    public void ApplyDecoration(Sprite sprite, bool mirrored)
    {
        EnsureDecorationTarget();

        if (sprite == null)
        {
            ClearDecoration();
            return;
        }

        if (decorationSpriteRenderer != null)
        {
            decorationSpriteRenderer.sprite = sprite;
            decorationSpriteRenderer.enabled = true;
        }

        ApplyDecorationMirror(mirrored);

        if (decorationTransform != null)
        {
            decorationTransform.gameObject.SetActive(true);
        }
    }

    public void ClearDecoration()
    {
        if (decorationSpriteRenderer != null)
        {
            decorationSpriteRenderer.sprite = null;
            decorationSpriteRenderer.enabled = false;
        }

        if (decorationTransform != null)
        {
            decorationTransform.gameObject.SetActive(false);
        }
    }

    public float GetWorldHeight()
    {
        AutoWire();

        if (backgroundSpriteRenderer != null && backgroundSpriteRenderer.sprite != null)
        {
            return backgroundSpriteRenderer.bounds.size.y;
        }

        return transform.lossyScale.y;
    }

    private void EnsureDecorationTarget()
    {
        if (decorationTransform == null)
        {
            Transform existing = transform.Find(decorationChildName);
            if (existing != null)
            {
                decorationTransform = existing;
            }
            else
            {
                GameObject child = new GameObject(decorationChildName);
                child.transform.SetParent(transform, false);
                decorationTransform = child.transform;
            }
        }

        if (backgroundSpriteRenderer != null)
        {
            EnsureSpriteRendererDecoration();
        }
    }

    private void EnsureSpriteRendererDecoration()
    {
        decorationSpriteRenderer = decorationTransform.GetComponent<SpriteRenderer>();
        if (decorationSpriteRenderer == null)
        {
            decorationSpriteRenderer =
                decorationTransform.gameObject.AddComponent<SpriteRenderer>();
        }

        decorationTransform.localPosition = Vector3.zero;
        decorationTransform.localRotation = Quaternion.identity;
        decorationTransform.localScale = Vector3.one;

        decorationSpriteRenderer.sortingLayerID = backgroundSpriteRenderer.sortingLayerID;
        decorationSpriteRenderer.sortingOrder = backgroundSpriteRenderer.sortingOrder + 1;
        decorationSpriteRenderer.enabled = false;
    }

    private void ApplyDecorationMirror(bool mirrored)
    {
        if (decorationTransform == null)
            return;

        Vector3 scale = decorationTransform.localScale;
        scale.x = Mathf.Abs(scale.x) * (mirrored ? -1f : 1f);
        decorationTransform.localScale = scale;
    }
}
