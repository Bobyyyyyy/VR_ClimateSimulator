using UnityEngine;

public class Pan : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool isOnActiveBurner = false;

    private StoveBurner currentBurner;

    /// <summary>
    /// True se la padella è su un fornello con la manopola girata.
    /// Viene letto da CookableFood.
    /// </summary>
    public bool IsHot => currentBurner != null && currentBurner.IsActive;

    private void OnTriggerEnter(Collider other)
    {
        if (currentBurner == null)
        {
            StoveBurner burner = other.GetComponent<StoveBurner>();
            if (burner != null)
            {
                currentBurner = burner;
                Debug.Log("Padella posizionata sul fornello.");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (currentBurner != null && other.GetComponent<StoveBurner>() == currentBurner)
        {
            Debug.Log("Padella tolta dal fornello.");
            currentBurner = null;
        }
    }

    void Update()
    {
        isOnActiveBurner = IsHot;
    }
}