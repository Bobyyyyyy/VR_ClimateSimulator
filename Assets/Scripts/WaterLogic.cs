using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RubinettoVR : MonoBehaviour
{
    public enum Axis { X, Y, Z }

    [Header("Acqua")]
    public ParticleSystem acquaEffect;

    [Header("Impostazioni Rotazione")]
    public Axis rotationAxis = Axis.Z;

    [Tooltip("Gradi a cui scatta quando è acceso")]
    public float snapAngle = 90f;

    [Tooltip("Velocità dell'animazione (gradi al secondo)")]
    public float rotationSpeed = 180f;

    [Header("Debug")]
    [SerializeField] private bool isOn;
    [SerializeField] private float currentAngle;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable simpleInteractable;
    private Quaternion baseRotation;
    private float targetAngle;

    void Awake()
    {
        simpleInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        baseRotation = transform.localRotation;

        simpleInteractable.selectEntered.AddListener(OnGrab);
    }

    void Start()
    {
        if (acquaEffect != null) acquaEffect.Stop();
    }

    void OnDestroy()
    {
        simpleInteractable.selectEntered.RemoveListener(OnGrab);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        isOn = !isOn;
        targetAngle = isOn ? snapAngle : 0f;

        // L'acqua parte/si ferma subito al grab
        if (acquaEffect == null) return;
        if (isOn && !acquaEffect.isPlaying) acquaEffect.Play();
        else if (!isOn && acquaEffect.isPlaying) acquaEffect.Stop();
    }

    void Update()
    {
        if (Mathf.Approximately(currentAngle, targetAngle)) return;

        currentAngle = Mathf.MoveTowards(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);

        Vector3 euler = Vector3.zero;
        switch (rotationAxis)
        {
            case Axis.X: euler.x = currentAngle; break;
            case Axis.Y: euler.y = currentAngle; break;
            case Axis.Z: euler.z = currentAngle; break;
        }

        transform.localRotation = baseRotation * Quaternion.Euler(euler);
    }
}