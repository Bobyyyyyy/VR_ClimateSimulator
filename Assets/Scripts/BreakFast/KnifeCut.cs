using UnityEngine;

public class Coltello : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Tagliabile tagliabile = other.GetComponent<Tagliabile>();
        if (tagliabile != null)
        {
            tagliabile.EseguiTaglio();
        }
    }
}