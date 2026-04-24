using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.Events;

public class JarLid : MonoBehaviour
{
    public UnityEvent onLidOpened;
    public InputActionProperty pressAction;
    public int pressesRequired = 5;
    public float rotationPerPress = 72f;
    public float rotationSpeed = 360f;
    public float popForce = 3f;
    public float spinForce = 0.5f;

    private XRGrabInteractable grabInteractable;
    private bool isGrabbed = false;
    private bool isOpened = false;
    private int pressCount = 0;
    private float targetRotation = 0f;
    private float currentRotation = 0f;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    void OnEnable() => pressAction.action.Enable();
    void OnDisable() => pressAction.action.Disable();

    void OnGrab(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        pressAction.action.performed += OnButtonPressed;
    }

    void OnRelease(SelectExitEventArgs args)
    {
        isGrabbed = false;
        pressAction.action.performed -= OnButtonPressed;
    }

    void OnButtonPressed(InputAction.CallbackContext ctx)
    {
        if (isOpened || !isGrabbed) return;

        pressCount++;
        targetRotation += rotationPerPress;
        Debug.Log($"Press {pressCount}/{pressesRequired}");

        if (pressCount >= pressesRequired)
            OpenLid();
    }

    void Update()
    {
        if (Mathf.Abs(currentRotation - targetRotation) > 0.01f)
        {
            float newRotation = Mathf.MoveTowards(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
            float delta = newRotation - currentRotation;
            transform.Rotate(Vector3.up, delta, Space.World);
            currentRotation = newRotation;
        }
    }

void OpenLid()
{
    isOpened = true;
    grabInteractable.enabled = false;
    transform.SetParent(null);

    Rigidbody rb = GetComponent<Rigidbody>();
    rb.isKinematic = false;
    rb.useGravity = true;

    rb.AddForce(Vector3.up * popForce, ForceMode.Impulse);
    rb.AddTorque(Random.insideUnitSphere * spinForce, ForceMode.Impulse);

    onLidOpened?.Invoke();
}
}