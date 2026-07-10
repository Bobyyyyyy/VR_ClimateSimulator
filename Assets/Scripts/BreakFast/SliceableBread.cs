using UnityEngine;

public class SliceableBread : MonoBehaviour
{
    [Header("Halves prefabs or pre-placed inactive objects")]
    public GameObject topHalf;
    public GameObject bottomHalf;

    [Header("Optional: small force to separate halves")]
    public float separationForce = 0.3f;

    private bool _hasBeenSliced = false;

    private void OnTriggerEnter(Collider other)
    {
        if (_hasBeenSliced) return;

        // Il coltello deve avere il tag "Knife"
        if (!other.CompareTag("Knife")) return;

        Slice();
    }

    private void Slice()
    {
        _hasBeenSliced = true;

        // Sgancia le metà dal padre così diventano indipendenti
        topHalf.transform.SetParent(null);
        bottomHalf.transform.SetParent(null);

        // Attivale (se erano disattivate)
        topHalf.SetActive(true);
        bottomHalf.SetActive(true);

        // Piccola spinta per separarle
        Rigidbody topRb = topHalf.GetComponent<Rigidbody>();
        Rigidbody bottomRb = bottomHalf.GetComponent<Rigidbody>();

        if (topRb != null)
            topRb.AddForce(Vector3.up * separationForce, ForceMode.Impulse);
        if (bottomRb != null)
            bottomRb.AddForce(Vector3.down * separationForce, ForceMode.Impulse);

        // Disattiva il contenitore padre
        gameObject.SetActive(false);
    }
}