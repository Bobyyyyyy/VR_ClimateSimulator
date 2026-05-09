using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRSocketInteractor))]
public class LockOnSocketEnter : MonoBehaviour
{
    private XRSocketInteractor socket;

    void Awake() => socket = GetComponent<XRSocketInteractor>();

    void OnEnable()  => socket.selectEntered.AddListener(OnEnter);
    void OnDisable() => socket.selectEntered.RemoveListener(OnEnter);

    private void OnEnter(SelectEnterEventArgs args)
    {
        var go = args.interactableObject.transform.gameObject;
        go.transform.SetParent(transform, true);

        var rb = go.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        var grab = go.GetComponent<XRGrabInteractable>();
        if (grab != null) grab.enabled = false;
    }
}