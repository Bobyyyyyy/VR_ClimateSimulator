using UnityEngine;


public class BinDetector : MonoBehaviour
{

    public TrashType BinType;

    private void OnTriggerEnter(Collider other)
    {
        GameObject collidedObject = other.gameObject;
        if (collidedObject.CompareTag("Trash"))
        {
            if(collidedObject.GetComponent<TrashScript>().data.type == BinType)
            {
                Destroy(collidedObject);
            }
            
        }
    }
}
