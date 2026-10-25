using UnityEngine;

public class Knife : MonoBehaviour
{
    public GameObject jamDecal; 
    private bool hasJam = false;

    void Start()
    {
        if (jamDecal != null)
            jamDecal.SetActive(false);
        
        hasJam = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasJam && other.TryGetComponent<JarBehaviour>(out _))
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

        if (jamDecal != null)
        {
            jamDecal.SetActive(false);
        }    

    }
}