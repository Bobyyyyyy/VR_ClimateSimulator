using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class LightSwitchVR : MonoBehaviour
{
    [Header("Luci da controllare")]
    [Tooltip("Trascina qui tutte le Light che vuoi accendere/spegnere")]
    public Light[] lights;

    [Tooltip("Intensità di ogni luce quando accesa (stesso ordine dell'array 'lights')")]
    public float[] onIntensities;
    [Tooltip("Intensità di ogni luce quando spenta (stesso ordine dell'array 'lights')")]
    public float[] offIntensities;

    [Header("Task giornata (opzionale)")]
    [Tooltip("Sparato quando l'interruttore viene acceso")]
    public UnityEvent onTurnedOn;

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

    private bool locked = false;

    public void Lock() => locked = true;

    void Awake()
    {
        simpleInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        simpleInteractable.selectEntered.AddListener(OnPress);

        ApplyLightState();

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
        if (locked) return;

        isOn = !isOn;
        targetAngle = isOn ? onAngle : offAngle;

        ApplyLightState();

        if (isOn) onTurnedOn?.Invoke();
    }

    private void ApplyLightState()
    {
        for (int i = 0; i < lights.Length; i++)
        {
            if (lights[i] == null) continue;

            float onValue = i < onIntensities.Length ? onIntensities[i] : 0f;
            float offValue = i < offIntensities.Length ? offIntensities[i] : 0f;
            lights[i].intensity = isOn ? onValue : offValue;
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
