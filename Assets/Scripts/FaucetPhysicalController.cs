using UnityEngine;

public class FaucetPhysicalController : MonoBehaviour
{
    public FaucetLogic waterScript;
    public HingeJoint hinge;
    public float maxAngle = 90f;

    void Update()
    {
        float currentAngle = Mathf.Abs(hinge.angle); 
        float normalizedValue = Mathf.Clamp01(currentAngle / maxAngle);

        waterScript.UpdateFlow(normalizedValue);
    }
}