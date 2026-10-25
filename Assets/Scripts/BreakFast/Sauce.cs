using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class SauceBottle : MonoBehaviour
{
    [Header("Configurazione salsa")]
    public SauceType sauceType;

    [Header("Effetti (opzionali)")]
    public ParticleSystem sauceParticles;
    public AudioSource sauceSound;

    private XRGrabInteractable grabInteractable;
    private Panino paninoInRange;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    void OnEnable()
    {
        grabInteractable.activated.AddListener(OnActivated);
    }

    void OnDisable()
    {
        grabInteractable.activated.RemoveListener(OnActivated);
    }

    private void OnActivated(ActivateEventArgs args)
    {
        if (sauceParticles != null) sauceParticles.Play();
        if (sauceSound != null) sauceSound.Play();

        if (paninoInRange != null)
        {
            paninoInRange.AddSauce(sauceType);
            Debug.Log($"Salsa {sauceType} aggiunta al panino!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        var panino = other.GetComponentInParent<Panino>();
        if (panino != null) paninoInRange = panino;
    }

    void OnTriggerExit(Collider other)
    {
        var panino = other.GetComponentInParent<Panino>();
        if (panino != null && panino == paninoInRange) paninoInRange = null;
    }
}

public enum SauceType
{
    Ketchup,
    Maionese
}