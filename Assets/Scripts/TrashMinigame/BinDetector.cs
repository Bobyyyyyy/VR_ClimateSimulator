using UnityEngine;


public class BinDetector : MonoBehaviour
{

    public TrashType BinType;

    private void OnTriggerEnter(Collider other)
    {
        GameObject collidedObject = other.gameObject;
        if (collidedObject.CompareTag("Trash"))
        {
           
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.Play();
            if(collidedObject.GetComponent<Paper>().data.type == BinType)
            {
                Destroy(collidedObject);
            }
            
        }
    }
}
