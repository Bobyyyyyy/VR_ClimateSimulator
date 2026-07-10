using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class FridgeDinnerMenu : MonoBehaviour
{
    [Header("Set 'Crea la cena' (burger, node_id105, UncrackedEgg_239: già in scena, disattivati di default)")]
    public GameObject makeDinnerSet;
    public Transform makeDinnerExtraAnchor;
    public GameObject uncookedPattyPrefab;
    public GameObject panPrefab;

    [Header("Materiali padella/cottura per la patty (assegnati qui perché non caricabili da codice a runtime)")]
    public Material pattyRawMaterial;
    public Material pattyCookedMaterial;
    public Material pattyBurntMaterial;

    [Header("Set 'Ordina cena' (vuoto: viene popolato instanziando i prefab)")]
    public Transform orderDinnerAnchor;
    public GameObject pizzaPrefab;
    public GameObject sodaPrefab;

    private readonly List<GameObject> spawnedMakeExtras = new List<GameObject>();
    private readonly List<GameObject> spawnedOrderItems = new List<GameObject>();

    void Start()
    {
        if (makeDinnerSet != null) makeDinnerSet.SetActive(false);
        if (orderDinnerAnchor != null) orderDinnerAnchor.gameObject.SetActive(false);
    }

    [ContextMenu("Ordina cena")]
    public void OrderDinner()
    {
        if (makeDinnerSet != null) makeDinnerSet.SetActive(false);
        ClearSpawned(spawnedMakeExtras);

        if (orderDinnerAnchor == null) return;
        orderDinnerAnchor.gameObject.SetActive(true);

        if (spawnedOrderItems.Count == 0)
        {
            SpawnGrabbable(pizzaPrefab, orderDinnerAnchor, new Vector3(0f, 0f, 0f), spawnedOrderItems);
            SpawnGrabbable(sodaPrefab, orderDinnerAnchor, new Vector3(0.25f, 0f, 0f), spawnedOrderItems);
        }
    }

    [ContextMenu("Crea la cena")]
    public void MakeDinner()
    {
        if (orderDinnerAnchor != null) orderDinnerAnchor.gameObject.SetActive(false);
        ClearSpawned(spawnedOrderItems);

        if (makeDinnerSet != null) makeDinnerSet.SetActive(true);
        if (makeDinnerExtraAnchor == null) return;

        if (spawnedMakeExtras.Count == 0)
        {
            SpawnPatty(uncookedPattyPrefab, makeDinnerExtraAnchor, new Vector3(0.3f, 0f, 0f));
            SpawnPan(panPrefab, makeDinnerExtraAnchor, new Vector3(-0.3f, 0f, 0.2f));
        }
    }

    private void ClearSpawned(List<GameObject> list)
    {
        foreach (var go in list)
            if (go != null) Destroy(go);
        list.Clear();
    }

    private GameObject SpawnGrabbable(GameObject prefab, Transform parent, Vector3 localOffset, List<GameObject> track)
    {
        if (prefab == null) return null;
        var instance = Instantiate(prefab, parent);
        instance.transform.localPosition = localOffset;

        if (instance.GetComponentInChildren<Collider>() == null)
            FitBoxCollider(instance, false);

        if (instance.GetComponent<Rigidbody>() == null)
            instance.AddComponent<Rigidbody>();

        if (instance.GetComponent<XRGrabInteractable>() == null)
            instance.AddComponent<XRGrabInteractable>();

        track.Add(instance);
        return instance;
    }

    private void SpawnPatty(GameObject prefab, Transform parent, Vector3 localOffset)
    {
        var instance = SpawnGrabbable(prefab, parent, localOffset, spawnedMakeExtras);
        if (instance == null) return;

        var cookable = instance.GetComponent<CookableFood>();
        if (cookable == null) cookable = instance.AddComponent<CookableFood>();

        cookable.timeToCook = 5f;
        cookable.timeToBurn = 10f;
        cookable.panTag = "Pan";
        cookable.rawPrefab = prefab;
        cookable.foodParts = new[]
        {
            new FoodPart
            {
                partName = "Hamburger",
                partRenderer = instance.GetComponentInChildren<Renderer>(),
                rawMaterial = pattyRawMaterial,
                cookedMaterial = pattyCookedMaterial,
                burntMaterial = pattyBurntMaterial,
            }
        };
    }

    private void SpawnPan(GameObject prefab, Transform parent, Vector3 localOffset)
    {
        var instance = SpawnGrabbable(prefab, parent, localOffset, spawnedMakeExtras);
        if (instance == null) return;

        instance.tag = "Pan";
        if (instance.GetComponent<Pan>() == null) instance.AddComponent<Pan>();

        // Serve un collider trigger per rilevare cibo/fornello, in aggiunta all'eventuale collider fisico del prefab.
        FitBoxCollider(instance, true);
    }

    private void FitBoxCollider(GameObject go, bool isTrigger)
    {
        var renderers = go.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return;

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
            bounds.Encapsulate(renderers[i].bounds);

        var box = go.AddComponent<BoxCollider>();
        box.isTrigger = isTrigger;
        box.center = go.transform.InverseTransformPoint(bounds.center);

        Vector3 lossyScale = go.transform.lossyScale;
        box.size = new Vector3(
            lossyScale.x != 0f ? bounds.size.x / lossyScale.x : bounds.size.x,
            lossyScale.y != 0f ? bounds.size.y / lossyScale.y : bounds.size.y,
            lossyScale.z != 0f ? bounds.size.z / lossyScale.z : bounds.size.z);
    }
}
