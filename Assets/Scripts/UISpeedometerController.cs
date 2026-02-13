using TMPro;
using UnityEngine;
using UnityEngine.UI; // RawImage

[DisallowMultipleComponent]
public class UISpeedometerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private RawImage fillMaskRawImage;

    [Tooltip("TMP text for MPH display.")]
    [SerializeField]
    private TextMeshProUGUI mphText;

    [SerializeField]
    private float bottomSpeed = 0f;

    [SerializeField]
    private float topSpeed = 15f;

    [Header("MPH Display")]
    [Tooltip("mph = CurrentSpeed * multiplier")]
    [SerializeField]
    private float speedToMphMultiplier = 10f;

    [Tooltip("How many decimals to show in the MPH text.")]
    [SerializeField]
    private int mphDecimals = 0;

    [Header("Text Color By Fill")]
    [Tooltip("If enabled, MPH text color will be driven by fill amount (0..1).")]
    [SerializeField]
    private bool driveTextColorByFill = true;

    [Tooltip("Color ramp evaluated by fill amount (0..1).")]
    [SerializeField]
    private Gradient textColorByFill = new Gradient
    {
        colorKeys = new[]
        {
            new GradientColorKey(new Color(0.70f, 0.95f, 1.00f, 1f), 0f), // cool/cyan
            new GradientColorKey(new Color(0.20f, 1.00f, 0.25f, 1f), 0.4f), // green
            new GradientColorKey(new Color(1.00f, 0.90f, 0.20f, 1f), 0.7f), // yellow
            new GradientColorKey(new Color(1.00f, 0.25f, 0.20f, 1f), 1f), // red
        },
        alphaKeys = new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) },
    };

    [Tooltip(
        "Optional multiplier for how strongly the gradient affects the text. 0 = no change, 1 = full gradient."
    )]
    [Range(0f, 1f)]
    [SerializeField]
    private float textColorStrength = 1f;

    [Header("Shader Properties")]
    [SerializeField]
    private string fillProperty = "_Fill";

    [Header("Optional Direction Control")]
    [SerializeField]
    private bool driveFlipDirection = false;

    [SerializeField]
    private bool flipDirection = false;

    [SerializeField]
    private string flipProperty = "_FlipDirection";

    [Header("Behavior")]
    [Tooltip("If true, clones the material so the project asset isn't modified.")]
    [SerializeField]
    private bool cloneMaterialInstance = true;

    [SerializeField]
    private bool updateContinuously = true;

    private Material _mat;
    private int _fillID;
    private int _flipID;

    // For blending back to original color if desired
    private Color _baseTextColor = Color.white;
    private bool _cachedBaseTextColor;

    private void Awake()
    {
        _fillID = Shader.PropertyToID(fillProperty);
        _flipID = Shader.PropertyToID(flipProperty);

        if (!fillMaskRawImage)
        {
            Debug.LogError("[UISpeedometerController] Fill Mask RawImage is not assigned.", this);
            enabled = false;
            return;
        }

        var sourceMat = fillMaskRawImage.material;
        if (!sourceMat)
        {
            Debug.LogError(
                "[UISpeedometerController] Fill Mask RawImage has no material assigned.",
                this
            );
            enabled = false;
            return;
        }

        if (cloneMaterialInstance)
        {
            _mat = new Material(sourceMat);
            _mat.name = sourceMat.name + " (UISpeedometer Instance)";
            fillMaskRawImage.material = _mat;
        }
        else
        {
            _mat = sourceMat;
        }

        if (mphText && !_cachedBaseTextColor)
        {
            _baseTextColor = mphText.color;
            _cachedBaseTextColor = true;
        }
    }

    private void Start()
    {
        Refresh();
    }

    private void Update()
    {
        if (updateContinuously)
            Refresh();
    }

    public void Refresh()
    {
        float speed = GetCurrentSpeedSafe();

        float fill01 = RemapTo01(speed, bottomSpeed, topSpeed);
        _mat.SetFloat(_fillID, fill01);

        if (driveFlipDirection && _mat.HasProperty(_flipID))
            _mat.SetFloat(_flipID, flipDirection ? 1f : 0f);

        if (mphText)
        {
            float mph = speed * speedToMphMultiplier;
            string format = "F" + Mathf.Clamp(mphDecimals, 0, 3);
            mphText.text = mph.ToString(format) + " MPH";

            if (driveTextColorByFill)
            {
                // eval gradient by fill amount
                Color target = textColorByFill.Evaluate(fill01);

                // blend with base color using strength
                if (!_cachedBaseTextColor)
                {
                    _baseTextColor = mphText.color;
                    _cachedBaseTextColor = true;
                }

                mphText.color = Color.Lerp(
                    _baseTextColor,
                    target,
                    Mathf.Clamp01(textColorStrength)
                );
            }
            else if (_cachedBaseTextColor)
            {
                // restore original if disabled
                mphText.color = _baseTextColor;
            }
        }
    }

    private float GetCurrentSpeedSafe()
    {
        if (GameSpeedController.Instance != null)
            return GameSpeedController.Instance.CurrentSpeed;

        var gsc = GameSpeedController.GetOrCreate();
        return gsc != null ? gsc.CurrentSpeed : 0f;
    }

    private static float RemapTo01(float value, float min, float max)
    {
        if (Mathf.Approximately(max, min))
            return 0f;

        return Mathf.Clamp01((value - min) / (max - min));
    }

    private void OnDestroy()
    {
        if (cloneMaterialInstance && _mat != null)
        {
            Destroy(_mat);
            _mat = null;
        }
    }

    public void SetBottomSpeed(float v) => bottomSpeed = v;

    public void SetTopSpeed(float v) => topSpeed = v;

    public void SetFlipDirection(bool v) => flipDirection = v;

    public void SetDriveTextColorByFill(bool v)
    {
        driveTextColorByFill = v;
        if (!v && mphText && _cachedBaseTextColor)
            mphText.color = _baseTextColor;
    }
}
