using UnityEngine;
using UnityEngine.Events;

public class ButtonBehaviour : MonoBehaviour
{
    public UnityEvent onPressed;

    void OnTriggerEnter(Collider other)
    {
        onPressed?.Invoke();
    }

}