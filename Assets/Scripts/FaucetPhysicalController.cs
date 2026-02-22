using UnityEngine;

public class FaucetPhysicalController : MonoBehaviour
{
    public FaucetLogic waterScript; // Trascina qui l'oggetto con lo script dell'acqua
    public HingeJoint hinge;
    public float maxAngle = 90f; // Deve essere uguale al Max Limit dell'Hinge Joint

    void Update()
    {
        // Calcoliamo quanto è girata la manopola in percentuale (0 a 1)
        // L'angolo dell'hinge joint di solito è negativo o positivo a seconda della rotazione
        float currentAngle = Mathf.Abs(hinge.angle); 
        float normalizedValue = Mathf.Clamp01(currentAngle / maxAngle);

        // Passiamo il valore allo script dell'acqua
        waterScript.UpdateFlow(normalizedValue);
    }
}