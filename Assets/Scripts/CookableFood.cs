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
    private enum CookState { Raw, Cooking, Cooked, Burning, Burnt }
    [Header("Impostazioni Cottura")]
    public float timeToCook = 5f;
    public float timeToBurn = 10f;
    public string panTag = "Pan";

    [Header("Parti del Cibo e Materiali")]
    public FoodPart[] foodParts;

    [Header("Debug (Solo per visualizzare)")]
    [SerializeField] private float currentCookTime = 0f;
    [SerializeField] private bool isInPan = false;
    [SerializeField] private bool isActuallyCooking = false;

    private CookState currentState = CookState.Raw;

    private Pan currentPan;

    void Start()
    {
        SetStateMaterials(CookState.Raw);
    }

    void Update()
    {
        // Cuoce SOLO se: è in padella + la padella è su un fornello acceso
        bool panIsHot = currentPan != null && currentPan.IsHot;
        isActuallyCooking = isInPan && panIsHot && currentState != CookState.Burnt;

        if (!isActuallyCooking) return;

        currentCookTime += Time.deltaTime;

        if (currentCookTime >= timeToCook && currentState == CookState.Cooking)
        {
            currentState = CookState.Cooked;
            SetStateMaterials(CookState.Cooked);
            Debug.Log("Tutto il cibo è COTTO!");
        }

        if (currentCookTime >= timeToBurn && currentState == CookState.Cooked)
        {
            currentState = CookState.Burnt;
            SetStateMaterials(CookState.Burnt);
            Debug.Log("Tutto il cibo è BRUCIATO!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(panTag) && currentState != CookState.Burnt)
        {
            isInPan = true;
            currentPan = other.GetComponentInParent<Pan>();

            if (currentState == CookState.Raw)
            {
                currentState = CookState.Cooking;
            }

            Debug.Log($"Cibo in padella! Tempo accumulato: {currentCookTime:F1}s");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(panTag))
        {
            isInPan = false;
            currentPan = null;
            Debug.Log($"Cibo tolto dalla padella. Tempo salvato: {currentCookTime:F1}s");
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