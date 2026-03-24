using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class OrangeSqueezer : MonoBehaviour
{
    public bool isSqueezing = false;
    public int requiredOranges = 3;
    public float requiredSqueezeTime = 5.0f;
    private float currentSqueezeTime = 0f;
    public float requiredJuiceTime = 15f;
    private float maxJuiceHeight = 0.05f;

    private float juiceFlowSpeed = 0.02f;


    public GameObject juiceFlow;
    
    public GameObject mugLiquid;


    void Start()
    {
        juiceFlowSpeed = maxJuiceHeight / requiredJuiceTime;  

    }

    void Update()
    {
        if (isSqueezing)
        {
            if(juiceFlow != null) juiceFlow.SetActive(true);

            if(mugLiquid != null) mugLiquid.SetActive(true);

            mugLiquid.transform.localPosition += new Vector3(0,0, juiceFlowSpeed * Time.deltaTime);

            if(mugLiquid.transform.localPosition.z >= maxJuiceHeight)
            {
                isSqueezing = false;
            }

        }

        else
        {
            if(juiceFlow != null) juiceFlow.SetActive(false);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Orange"))
        {
            XRGrabInteractable grabInteractable = collision.gameObject.GetComponent<XRGrabInteractable>();
            
            if (grabInteractable != null && grabInteractable.isSelected)
            {
                isSqueezing = true;
                currentSqueezeTime += Time.deltaTime;
                Debug.Log($"Spremitura in corso... {currentSqueezeTime:F1}/{requiredSqueezeTime} sec");

                if (currentSqueezeTime >= requiredSqueezeTime)
                {
                    isSqueezing = false;
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