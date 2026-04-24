using UnityEngine;
using System.Collections.Generic;

public class BinDetector : MonoBehaviour
{

    public TrashType BinType;
    private Rigidbody rb;
    
    private List<GameObject> insideTrash = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        GameObject collidedObject = other.gameObject;
        if (collidedObject.CompareTag("Trash"))
        {
            if (BinType == TrashType.undifferentiated)
            {
                // User decided to throw in the big undifferentiated bin instead of separating it
                if (!insideTrash.Contains(collidedObject))
                {
                    insideTrash.Add(collidedObject);
                }
            }
            else if(collidedObject.GetComponent<TrashScript>().data.type == BinType)
            {
                // Correct bin
                if (!insideTrash.Contains(collidedObject))
                {
                    insideTrash.Add(collidedObject);
                }
            }
            else
            {
                rb = collidedObject.GetComponent<Rigidbody>();
                rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject collidedObject = other.gameObject;
        if (collidedObject.CompareTag("Trash"))
        {
            if (insideTrash.Contains(collidedObject))
            {
                insideTrash.Remove(collidedObject);
            }
        }
    }

    public void CompressTrash()
    {
        foreach (GameObject trash in insideTrash)
        {
            if (trash != null)
            {
                if (BinType != TrashType.undifferentiated)
                {
                    TrashTask.Instance.NotifyTrashInsertion();
                    // Here there will be the code to store the fact that the user decided to use the right bin
                }
                Destroy(trash);
            }
        }
        insideTrash.Clear();
    }
}
