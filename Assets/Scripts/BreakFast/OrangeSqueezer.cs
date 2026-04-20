using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class OrangeSqueezer : MonoBehaviour
{
    public bool isSqueezing = false;
    public int requiredOranges = 3;
    public float requiredJuiceTime = 15f;
    private float maxJuiceHeight = 0.05f;
    private float juiceFlowSpeed;
    private float juicePerOrange;
    private float currentOrangeJuice = 0f;

    public GameObject juiceFlow;
    
    public GameObject mugLiquid;



    void Start()
    {
        juiceFlowSpeed = maxJuiceHeight / requiredJuiceTime;
        juicePerOrange = maxJuiceHeight / requiredOranges;
    }

    void Update()
    {
        if (isSqueezing && mugLiquid.transform.localPosition.z <= maxJuiceHeight && currentOrangeJuice < juicePerOrange)
        {

            if(juiceFlow != null) juiceFlow.SetActive(true);

            if(mugLiquid != null) mugLiquid.SetActive(true);

            float delta = juiceFlowSpeed * Time.deltaTime;
            currentOrangeJuice += delta;
            mugLiquid.transform.localPosition += new Vector3(0,0, delta);

            if(mugLiquid.transform.localPosition.z >= maxJuiceHeight)
            {
                isSqueezing = false;
            }

        }

        else
        {
            if(juiceFlow != null) juiceFlow.SetActive(false);
        }

        //fix flusso non arriva alla maxheight

    }

    void OnTriggerStay(Collider other)
    {
         if (other.gameObject.CompareTag("Fruit"))
        {
            XRGrabInteractable grabInteractable = other.gameObject.GetComponent<XRGrabInteractable>();
            
            if (grabInteractable != null && grabInteractable.isSelected && mugLiquid.transform.localPosition.z <= maxJuiceHeight )
            {
                isSqueezing = true;

                if (currentOrangeJuice >= juicePerOrange)
                {
                    isSqueezing = false;
                    requiredOranges--;
                    Destroy(other.gameObject);
                    currentOrangeJuice = 0f;
                    if (requiredOranges == 0)
                    {
                        BreakfastTask.Instance.CompleteStep(0);
                    }
                }
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        if(isSqueezing) {
            isSqueezing = false;
            Debug.Log($"Spremitura fermata");
        } 
    }




}