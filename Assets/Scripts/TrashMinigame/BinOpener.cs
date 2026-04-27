using UnityEngine;
using UnityEngine.Events;

public class BinOpener : MonoBehaviour
{
    public Animator binAnimator;
    public UnityEvent onBinOpened;
    public float cooldown = 1.5f;
    private bool isPressed = false;

    void OnTriggerEnter(Collider other)
    {
        if (!isPressed && other.CompareTag("Player"))
        {
            isPressed = true;
            binAnimator.SetTrigger("OpenBin");
            Invoke(nameof(Reset), cooldown);
        }
    }

    void Reset() => isPressed = false;
}