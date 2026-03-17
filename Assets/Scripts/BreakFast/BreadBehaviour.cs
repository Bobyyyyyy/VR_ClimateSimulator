using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;


public class BreadSliceVR : MonoBehaviour
{
    public GameObject jamDecal; 
    private bool hasJam = false;

    private void Start()
    {
        if (jamDecal != null) jamDecal.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasJam && collision.gameObject.CompareTag("Knife"))
        {
            XRGrabInteractable grabInteractable = collision.gameObject.GetComponent<XRGrabInteractable>();
            
            // Verifica che il coltello sia impugnato
            if (grabInteractable != null && grabInteractable.isSelected)
            {
                ApplyJam();
            }
        }
    }

    private void ApplyJam()
    {
        hasJam = true;
        if (jamDecal != null) jamDecal.SetActive(true);
        BreakfastTask.Instance.CompleteStep(1);
        Destroy(this);
    }
}