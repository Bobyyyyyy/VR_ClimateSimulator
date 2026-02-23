using UnityEngine;


public class BinDetector : MonoBehaviour
{

    public TrashType BinType;
    private Rigidbody rb;
    private void OnTriggerEnter(Collider other)
    {
        GameObject collidedObject = other.gameObject;
        if (collidedObject.CompareTag("Trash"))
        {
            if (BinType == TrashType.undifferentiated)
            {
                // User decided to throw in the big undifferentiated bin instead of separating it
                Destroy(collidedObject);
            }
            else if(collidedObject.GetComponent<TrashScript>().data.type == BinType)
            {

                // Here there will be the code to store the fact that the user decided to use the right bin
                Destroy(collidedObject);
            }

            else
            {
                rb = collidedObject.GetComponent<Rigidbody>();
                rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
            }

        }
    }
}
