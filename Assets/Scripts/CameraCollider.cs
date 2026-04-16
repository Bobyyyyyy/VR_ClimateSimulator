using System;
using UnityEngine;

public class CameraCollider : MonoBehaviour
{

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Food"))
        {
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("Beverage"))
        {
            Destroy(other.gameObject);
        }
    }

}
