using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Tagliabile : MonoBehaviour
{
    [Header("Prefab delle due metà")]
    public GameObject metaSinistraPrefab;
    public GameObject metaDestraPrefab;

    [Header("Separazione")]
    public float distanzaSeparazione = 0.05f;
    public float forzaSeparazione = 1f;

    [Header("Layer del socket pomodoro")]
    public InteractionLayerMask tomatoSocketLayer;

    public void EseguiTaglio()
    {
        Vector3 offset = transform.right * distanzaSeparazione;

        GameObject sinistra = Instantiate(metaSinistraPrefab, transform.position - offset, transform.rotation);
        GameObject destra   = Instantiate(metaDestraPrefab,  transform.position + offset, transform.rotation);

        sinistra.transform.localScale = transform.lossyScale;
        destra.transform.localScale   = transform.lossyScale;

        ConfiguraMeta(sinistra);
        ConfiguraMeta(destra);

        sinistra.GetComponent<Rigidbody>().AddForce(-transform.right * forzaSeparazione, ForceMode.Impulse);
        destra.GetComponent<Rigidbody>().AddForce(transform.right * forzaSeparazione, ForceMode.Impulse);

        Destroy(gameObject);
    }

    private void ConfiguraMeta(GameObject meta)
    {
        if (meta.GetComponent<Collider>() == null)
            meta.AddComponent<BoxCollider>();

        if (meta.GetComponent<Rigidbody>() == null)
            meta.AddComponent<Rigidbody>();

        var grab = meta.GetComponent<XRGrabInteractable>();
        if (grab == null)
            grab = meta.AddComponent<XRGrabInteractable>();

        grab.interactionLayers = tomatoSocketLayer;
    }
}