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

    [Header("Respawn quando bruciato")]
    [Tooltip("Prefab dell'oggetto crudo da spawnare (può essere lo stesso prefab)")]
    public GameObject rawPrefab;
    [Tooltip("Punto dove spawna il nuovo oggetto crudo")]
    public Transform spawnPoint;
    [Tooltip("Secondi dopo il bruciato prima di spawnare il nuovo")]
    public float respawnDelay = 2f;

    [Header("Debug (Solo per visualizzare)")]
    [SerializeField] private float currentCookTime = 0f;
    [SerializeField] private bool isInPan = false;
    [SerializeField] private bool isActuallyCooking = false;

    private CookState currentState = CookState.Raw;
    private Pan currentPan;
    private bool hasRespawned = false;

    // Proprietà pubbliche per DinnerTask
    public bool IsCooked => currentState == CookState.Cooked;
    public bool IsBurnt => currentState == CookState.Burnt;

    void Start()
    {
        SetStateMaterials(CookState.Raw);

        // Se non hai assegnato uno spawn point, salva la posizione iniziale
        if (spawnPoint == null)
        {
            GameObject sp = new GameObject($"SpawnPoint_{gameObject.name}");
            sp.transform.position = transform.position;
            sp.transform.rotation = transform.rotation;
            spawnPoint = sp.transform;
        }
    }

    void Update()
    {
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

            // Spawna un nuovo oggetto crudo dopo un delay
            if (!hasRespawned && rawPrefab != null)
            {
                hasRespawned = true;
                Invoke(nameof(SpawnNewRaw), respawnDelay);
            }
        }
    }

    private void SpawnNewRaw()
    {
        GameObject newFood = Instantiate(rawPrefab, spawnPoint.position, spawnPoint.rotation);
        
        // Il nuovo oggetto avrà il suo CookableFood già configurato dal prefab
        // Assegna lo stesso spawn point così anche il prossimo respawna lì
        CookableFood newCookable = newFood.GetComponent<CookableFood>();
        if (newCookable != null)
        {
            newCookable.spawnPoint = spawnPoint;
            newCookable.currentCookTime = 0f;
            newCookable.currentState = CookState.Raw;
            newCookable.isInPan = false;
            newCookable.isActuallyCooking = false;
            newCookable.hasRespawned = false;
        }

        Debug.Log($"Nuovo {rawPrefab.name} crudo spawnato in {spawnPoint.position}!");
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