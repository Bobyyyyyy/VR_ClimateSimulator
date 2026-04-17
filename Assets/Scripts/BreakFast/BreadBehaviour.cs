using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;


public class BreadBehaviour : MonoBehaviour
{
    public GameObject jamDecal; 

    public int spreads = 4;
    private int currentSpreads = 0;

    public Vector3 initialScale;
    public Vector3 finalScale;

    private void Start()
    {
        if (jamDecal != null) jamDecal.SetActive(false);
        finalScale = jamDecal.transform.localScale;
        initialScale = finalScale / spreads;
        
    }

    [ContextMenu("Apply Jam")]
    public void ApplyJam()
    {
         if (currentSpreads == 0 && jamDecal != null)
        {
            jamDecal.transform.localScale = initialScale;
            jamDecal.SetActive(true);
        }

        if (currentSpreads < spreads)
        {
            currentSpreads++;
            float progresso = (float)currentSpreads / spreads;
            
            if (jamDecal != null)
            {
                jamDecal.transform.localScale = Vector3.Lerp(initialScale, finalScale, progresso);
            }
        }
        else {
            BreakfastTask.Instance.CompleteStep(1);  
        }
    }
}