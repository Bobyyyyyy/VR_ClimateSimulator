using UnityEngine;

[System.Serializable] 
public class FoodPart
{
    public string partName; 
    public Renderer partRenderer;    
    public Material rawMaterial;     
    public Material cookedMaterial;  
    public Material burntMaterial;   
}

public class CookableFood : MonoBehaviour
{
    [Header("Impostazioni Cottura")]
    public float timeToCook = 5f;
    public float timeToBurn = 10f;
    public string panTag = "Pan";

    [Header("Parti del Cibo e Materiali")]
    public FoodPart[] foodParts; 

    [Header("Debug (Solo per visualizzare)")]
    // Aggiunto [SerializeField] così puoi vedere il timer che si ferma e riparte su Unity!
    [SerializeField] private float currentCookTime = 0f; 
    [SerializeField] private bool isCooking = false;

    private enum CookState { Raw, Cooking, Cooked, Burning, Burnt }
    private CookState currentState = CookState.Raw;

    void Start()
    {
        SetStateMaterials(CookState.Raw);
    }

    void Update()
    {
        // Se non è in padella, esce dall'Update e il tempo NON aumenta (rimane salvato)
        if (!isCooking) return;

        // Se è in padella, il tempo continua ad aumentare da dove si era fermato
        currentCookTime += Time.deltaTime;

        // Fase 1: in cottura → diventa cotto
        if (currentCookTime >= timeToCook && currentState == CookState.Cooking)
        {
            currentState = CookState.Cooked;
            SetStateMaterials(CookState.Cooked);
            Debug.Log("Tutto il cibo è COTTO!");
        }

        // Fase 2: continua a cuocere → si brucia
        if (currentCookTime >= timeToBurn && currentState == CookState.Cooked)
        {
            currentState = CookState.Burnt;
            SetStateMaterials(CookState.Burnt);
            isCooking = false; // Ferma il timer definitivamente
            Debug.Log("Tutto il cibo è BRUCIATO!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Modifica: Ora controlla solo che tocchi la padella e che NON sia già bruciato
        if (other.CompareTag(panTag) && currentState != CookState.Burnt)
        {
            isCooking = true;
            
            // Se era appena stato messo (crudo), passa allo stato in cottura
            if (currentState == CookState.Raw)
            {
                currentState = CookState.Cooking;
            }
            
            Debug.Log($"Cottura ripresa/iniziata! Tempo accumulato: {currentCookTime:F1} secondi");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(panTag))
        {
            isCooking = false; // Questo mette in pausa il timer in Update()
            Debug.Log($"Cibo tolto dalla padella. Tempo salvato: {currentCookTime:F1} secondi");
        }
    }

    private void SetStateMaterials(CookState state)
    {
        foreach (FoodPart part in foodParts)
        {
            if (part.partRenderer != null)
            {
                switch (state)
                {
                    case CookState.Raw:
                        part.partRenderer.material = part.rawMaterial;
                        break;
                    case CookState.Cooked:
                        part.partRenderer.material = part.cookedMaterial;
                        break;
                    case CookState.Burnt:
                        part.partRenderer.material = part.burntMaterial;
                        break;
                }
            }
        }
    }
}