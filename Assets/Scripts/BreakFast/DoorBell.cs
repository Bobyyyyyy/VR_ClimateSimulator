using UnityEngine;
using UnityEngine.Events;

public class DoorBell : MonoBehaviour
{
    public float cooldown = 1.5f;
    private bool isPressed = false;

    void OnTriggerEnter(Collider other)
    {
        if (!isPressed && other.CompareTag("Player"))
        {
            isPressed = true;

            if (BreakfastTask.Instance.IsCurrentStepReady())
            {
                BreakfastTask.Instance.TryCompleteCurrentStep();
            }
            else
            {
                Debug.Log("Step non terminato");
            }

            Invoke(nameof(Reset), cooldown);
        }
    }

    void Reset()
    {
        isPressed = false;
    }
}