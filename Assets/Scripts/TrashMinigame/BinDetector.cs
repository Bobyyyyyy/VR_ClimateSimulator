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
            if(collidedObject.GetComponent<TrashScript>().data.type == BinType)
            {

                // Qui andrà messo il codice per aggiungere punti 
                Destroy(collidedObject);
            }

            else
            {
                // codice per espellere l'oggetto
                rb = collidedObject.GetComponent<Rigidbody>();
                rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
            }

        }
    }
}
