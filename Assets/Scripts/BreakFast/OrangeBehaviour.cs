using UnityEngine;

public class OrangeBehaviour : MonoBehaviour
{
    public GameObject slicedOrangePrefab;
    
    [ContextMenu("Test Slice")]
    public void Slice()
    {
        if (slicedOrangePrefab != null)
        {
            GameObject slicedInstance = Instantiate(slicedOrangePrefab, transform.position, transform.rotation);
            slicedInstance.transform.localScale = transform.localScale;
            slicedInstance.transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
            
            
            Destroy(gameObject);
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(""))
        {
     
            Slice();
        
        }
    }

}