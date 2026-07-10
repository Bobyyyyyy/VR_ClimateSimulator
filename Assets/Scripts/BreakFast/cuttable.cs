using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Tagliabile : MonoBehaviour
{
    [Header("Prefab delle due metà")]
    public GameObject metaSinistraPrefab;
    public GameObject metaDestraPrefab;

    [Header("Separazione")]
    public float distanzaSeparazione = 0.05f; // offset iniziale fra le due metà
    public float forzaSeparazione = 1f;       // spinta impulsiva laterale

    [Header("Layer del socket pomodoro")]
    public InteractionLayerMask tomatoSocketLayer;

    public void EseguiTaglio()
    {
        Vector3 offset = transform.right * distanzaSeparazione;

        // Spawna le due metà spostate lateralmente
        GameObject sinistra = Instantiate(metaSinistraPrefab, transform.position - offset, transform.rotation);
        GameObject destra   = Instantiate(metaDestraPrefab,  transform.position + offset, transform.rotation);

        // Mantieni la stessa scala del pomodoro intero
        sinistra.transform.localScale = transform.lossyScale;
        destra.transform.localScale   = transform.lossyScale;

        // Aggiungi i componenti necessari se mancano
        ConfiguraMeta(sinistra);
        ConfiguraMeta(destra);

        // Spinta per farle staccare
        sinistra.GetComponent<Rigidbody>().AddForce(-transform.right * forzaSeparazione, ForceMode.Impulse);
        destra.GetComponent<Rigidbody>().AddForce(transform.right * forzaSeparazione, ForceMode.Impulse);

        Destroy(gameObject);
    }

    private void ConfiguraMeta(GameObject meta)
    {
        // BoxCollider
        if (meta.GetComponent<Collider>() == null)
            meta.AddComponent<BoxCollider>();

        // Rigidbody (deve esserci PRIMA dell'XRGrabInteractable)
        if (meta.GetComponent<Rigidbody>() == null)
            meta.AddComponent<Rigidbody>();

        // XR Grab Interactable
        var grab = meta.GetComponent<XRGrabInteractable>();
        if (grab == null)
            grab = meta.AddComponent<XRGrabInteractable>();

        // Imposta il layer per matchare il socket pomodoro
        grab.interactionLayers = tomatoSocketLayer;
    }
}