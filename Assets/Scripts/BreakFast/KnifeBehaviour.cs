using UnityEngine;

public class Knife : MonoBehaviour
{
    public GameObject jamDecal; 
    private bool hasJam = false;

    void Start()
    {
        // Assicuriamoci che il coltello parta pulito
        if (jamDecal != null)
            jamDecal.SetActive(false);
        
        hasJam = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasJam && other.TryGetComponent<JarBehaviour>(out _)) // Verifica che ci sia e scarta il risultato
        {
            ApplyJam();
        }

        if (hasJam && other.TryGetComponent<BreadBehaviour>(out BreadBehaviour bread))
        {
            ApplyJamBread(bread.gameObject);
        }
    }

    [ContextMenu("Apply Jam")]
    public void ApplyJam()
    {
        hasJam = true;
        
        if (jamDecal != null)
            jamDecal.SetActive(true);

    }

    void ApplyJamBread(GameObject bread)
    {
        
        bread.GetComponent<BreadBehaviour>().ApplyJam();
        hasJam = false;

        // Disattiviamo la decal visiva sulla lama
        if (jamDecal != null)
        {
            jamDecal.SetActive(false);
        }    

    }
}