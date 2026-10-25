using UnityEngine;

public class StoveBurner : MonoBehaviour
{
    [Header("Riferimenti")]
    [Tooltip("La manopola che controlla questo fuoco")]
    public StoveKnob stoveKnob;

    [Tooltip("Il disco/overlay posizionato sopra la piastra")]
    public Renderer glowOverlay;

    [Header("Emissione calore")]
    [Tooltip("Colore del fornello acceso")]
    public Color hotColor = new Color(1f, 0.2f, 0f, 0.85f);

    [Tooltip("Colore highlight quando si afferra la manopola (prima di girare)")]
    public Color highlightColor = new Color(1f, 0.6f, 0f, 0.3f);

    [Tooltip("Velocità della transizione visiva")]
    public float transitionSpeed = 3f;

    private Material _glowMaterial;
    private float _currentIntensity;
    private bool _isHighlighted;
    private Color _transparentColor = new Color(0f, 0f, 0f, 0f);

    private static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");

    public bool IsActive => stoveKnob != null && stoveKnob.IsOn;

    public float CurrentIntensity => _currentIntensity;

    void Awake()
    {
        if (glowOverlay != null)
        {
            _glowMaterial = glowOverlay.material;
            _glowMaterial.SetColor(BaseColorID, _transparentColor);
        }
    }

    void Update()
    {
        if (_glowMaterial == null) return;

        float target = GetTargetIntensity();
        _currentIntensity = Mathf.MoveTowards(_currentIntensity, target, transitionSpeed * Time.deltaTime);

        ApplyVisuals();
    }

    private float GetTargetIntensity()
    {
        if (stoveKnob == null) return 0f;

        return stoveKnob.IsOn ? 1f : 0f;
    }

    private void ApplyVisuals()
    {
        Color targetColor;

        if (_currentIntensity > 0.01f)
        {
            targetColor = Color.Lerp(_transparentColor, hotColor, _currentIntensity);
        }
        else if (_isHighlighted)
        {
            targetColor = highlightColor;
        }
        else
        {
            targetColor = _transparentColor;
        }

        _glowMaterial.SetColor(BaseColorID, targetColor);
    }

    public void OnKnobGrabbed()
    {
        _isHighlighted = true;
    }

    public void OnKnobReleased()
    {
        _isHighlighted = false;
    }
}