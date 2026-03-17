using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class OrangeSqueezer : MonoBehaviour
{
    public int requiredOranges = 3;
    public float requiredSqueezeTime = 2.0f;
    private float currentSqueezeTime = 0f;

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Orange"))
        {
            XRGrabInteractable grabInteractable = collision.gameObject.GetComponent<XRGrabInteractable>();
            
            if (grabInteractable != null && grabInteractable.isSelected)
            {
                // Aggiunge il tempo trascorso dall'ultimo frame
                currentSqueezeTime += Time.deltaTime;
                Debug.Log($"Spremitura in corso... {currentSqueezeTime:F1}/{requiredSqueezeTime} sec");

                if (currentSqueezeTime >= requiredSqueezeTime)
                {
                    requiredOranges--;
                    collision.gameObject.SetActive(false); 
                    currentSqueezeTime = 0f;
                    if(requiredOranges == 0)
                    {
                        BreakfastTask.Instance.CompleteStep(0);
                    }
                }
            }
        }
    }
}