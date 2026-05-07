using UnityEngine;


public class Tagliabile : MonoBehaviour
{
    [Header("Prefab delle due metà")]
    public GameObject metaSinistraPrefab;
    public GameObject metaDestraPrefab;

    [Header("Separazione")]
    public float distanzaSeparazione = 0.05f; // offset iniziale fra le due metà
    public float forzaSeparazione = 1f;       // spinta impulsiva laterale

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
        if (meta.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>() == null)
            meta.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
    }
}