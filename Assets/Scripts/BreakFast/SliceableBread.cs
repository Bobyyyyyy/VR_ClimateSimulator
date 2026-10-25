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

        if (!other.CompareTag("Knife")) return;

        Slice();
    }

    private void Slice()
    {
        _hasBeenSliced = true;

        topHalf.transform.SetParent(null);
        bottomHalf.transform.SetParent(null);

        topHalf.SetActive(true);
        bottomHalf.SetActive(true);

        Rigidbody topRb = topHalf.GetComponent<Rigidbody>();
        Rigidbody bottomRb = bottomHalf.GetComponent<Rigidbody>();

        if (topRb != null)
            topRb.AddForce(Vector3.up * separationForce, ForceMode.Impulse);
        if (bottomRb != null)
            bottomRb.AddForce(Vector3.down * separationForce, ForceMode.Impulse);

        gameObject.SetActive(false);
    }
}