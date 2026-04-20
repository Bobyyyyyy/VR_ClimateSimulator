using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LightSwitchVR : MonoBehaviour
{
    [Header("Luci da controllare")]
    [Tooltip("Trascina qui tutte le Light che vuoi accendere/spegnere")]
    public Light[] lights;

    [Header("Animazione Interruttore")]
    [Tooltip("L'oggetto Switch che ruota visivamente")]
    public Transform switchTransform;

    [Tooltip("Angolo quando è acceso")]
    public float onAngle = 10f;
    [Tooltip("Angolo quando è spento")]
    public float offAngle = -10f;

    public enum Axis { X, Y, Z }
    public Axis rotationAxis = Axis.X;

    [Tooltip("Velocità animazione (gradi al secondo)")]
    public float animationSpeed = 180f;

    [Header("Debug")]
    [SerializeField] private bool isOn = true;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable simpleInteractable;
    private Vector3 baseEuler;
    private float currentAngle;
    private float targetAngle;

    // Salva le intensità originali per ripristinarle
    private float[] originalIntensities;

    void Awake()
    {
        simpleInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        simpleInteractable.selectEntered.AddListener(OnPress);

        originalIntensities = new float[lights.Length];
        for (int i = 0; i < lights.Length; i++)
        {
            if (lights[i] != null)
                originalIntensities[i] = lights[i].intensity;
        }

        if (switchTransform != null)
        {
            baseEuler = switchTransform.localEulerAngles;
            currentAngle = isOn ? onAngle : offAngle;
            targetAngle = currentAngle;
        }
    }

    void OnDestroy()
    {
        simpleInteractable.selectEntered.RemoveListener(OnPress);
    }

    private void OnPress(SelectEnterEventArgs args)
    {
        isOn = !isOn;
        targetAngle = isOn ? onAngle : offAngle;

        for (int i = 0; i < lights.Length; i++)
        {
            if (lights[i] != null)
                lights[i].intensity = isOn ? originalIntensities[i] : 0f;
        }
    }

    void Update()
    {
        if (switchTransform == null) return;
        if (Mathf.Approximately(currentAngle, targetAngle)) return;

        currentAngle = Mathf.MoveTowards(currentAngle, targetAngle, animationSpeed * Time.deltaTime);

        Vector3 euler = baseEuler;
        switch (rotationAxis)
        {
            case Axis.X: euler.x += currentAngle; break;
            case Axis.Y: euler.y += currentAngle; break;
            case Axis.Z: euler.z += currentAngle; break;
        }

        switchTransform.localEulerAngles = euler;
    }
}