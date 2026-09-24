using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class FridgeDinnerMenu : MonoBehaviour
{
    [Header("Task giornata")]
    [Tooltip("Completato da 'Ordina cena' subito, oppure da BurgerAssembler.onAssemblyComplete per 'Crea la cena'")]
    public TaskData dinnerTask;

    [Header("Menu bottoni: sparisce non appena si sceglie")]
    public GameObject menuCanvas;

    [Header("Ricetta a muro: visibile solo per 'Crea la cena'")]
    public GameObject recipeOnWall;

    [Header("Set 'Crea la cena' (carne, padella, uovo ecc. già presenti come figli statici)")]
    public GameObject makeDinnerSet;

    [Header("Set 'Ordina cena'")]
    public Transform orderDinnerAnchor;
    public GameObject pizzaPrefab;
    public GameObject sodaPrefab;

    private readonly List<GameObject> spawnedOrderItems = new List<GameObject>();

    void Start()
    {
        if (makeDinnerSet != null) makeDinnerSet.SetActive(false);
        if (orderDinnerAnchor != null) orderDinnerAnchor.gameObject.SetActive(false);
        if (recipeOnWall != null) recipeOnWall.SetActive(false);

        UpdateMenuVisibility();

        if (TaskManager.Instance != null)
            TaskManager.Instance.onTaskChanged.AddListener(OnTaskChanged);
    }

    void OnDestroy()
    {
        if (TaskManager.Instance != null)
            TaskManager.Instance.onTaskChanged.RemoveListener(OnTaskChanged);
    }

    private void OnTaskChanged(TaskData newCurrentTask)
    {
        UpdateMenuVisibility();
    }

    // Il menu del frigo compare solo quando è il turno del task cena
    // (cioè non prima che il task del lavoro sia finito).
    private void UpdateMenuVisibility()
    {
        if (menuCanvas == null) return;

        bool shouldShow = dinnerTask != null && !dinnerTask.isCompleted
            && TaskManager.Instance != null && TaskManager.Instance.IsCurrentTask(dinnerTask);

        menuCanvas.SetActive(shouldShow);
    }

    [ContextMenu("Ordina cena")]
    public void OrderDinner()
    {
        ActionLogger.Instance?.LogAction(dinnerTask != null ? dinnerTask.taskName : "Cena", "Scelta cena",
            "Percorso scelto=Ordina cena (pizza+bibita)");

        if (menuCanvas != null) menuCanvas.SetActive(false);
        if (recipeOnWall != null) recipeOnWall.SetActive(false);

        if (makeDinnerSet != null) makeDinnerSet.SetActive(false);

        if (orderDinnerAnchor == null) return;
        orderDinnerAnchor.gameObject.SetActive(true);

        if (spawnedOrderItems.Count == 0)
        {
            SpawnGrabbable(pizzaPrefab, orderDinnerAnchor, new Vector3(0f, 0f, 0f), spawnedOrderItems, Vector3.one * 0.3f);
            SpawnGrabbable(sodaPrefab, orderDinnerAnchor, new Vector3(0.25f, 0f, 0f), spawnedOrderItems, Vector3.one * 0.3f);
        }

        // Ordinare la cena non richiede altri passaggi: è già pronta da mangiare.
        CompleteDinnerTask();
    }

    public void CompleteDinnerTask()
    {
        if (dinnerTask == null || dinnerTask.isCompleted) return;
        if (TaskManager.Instance != null && !TaskManager.Instance.IsCurrentTask(dinnerTask)) return;

        dinnerTask.isCompleted = true;
        TaskManager.Instance.addPoints(TaskManager.Instance.pointsPerChoice);

        ActionLogger.Instance?.LogAction(dinnerTask.taskName, "Task completata", "");

        TaskManager.Instance.UpdateTaskProgress(dinnerTask);
    }

    [ContextMenu("Crea la cena")]
    public void MakeDinner()
    {
        ActionLogger.Instance?.LogAction(dinnerTask != null ? dinnerTask.taskName : "Cena", "Scelta cena",
            "Percorso scelto=Crea la cena (cucina)");

        if (menuCanvas != null) menuCanvas.SetActive(false);
        if (recipeOnWall != null) recipeOnWall.SetActive(true);

        if (orderDinnerAnchor != null) orderDinnerAnchor.gameObject.SetActive(false);
        ClearSpawned(spawnedOrderItems);

        if (makeDinnerSet != null) makeDinnerSet.SetActive(true);
    }

    private void ClearSpawned(List<GameObject> list)
    {
        foreach (var go in list)
            if (go != null) Destroy(go);
        list.Clear();
    }

    private GameObject SpawnGrabbable(GameObject prefab, Transform parent, Vector3 localOffset, List<GameObject> track, Vector3? localScale = null)
    {
        if (prefab == null) return null;
        var instance = Instantiate(prefab, parent);
        instance.transform.localPosition = localOffset;
        if (localScale.HasValue) instance.transform.localScale = localScale.Value;

        if (instance.GetComponentInChildren<Collider>() == null)
            FitBoxCollider(instance, false);

        if (instance.GetComponent<Rigidbody>() == null)
            instance.AddComponent<Rigidbody>();

        if (instance.GetComponent<XRGrabInteractable>() == null)
            instance.AddComponent<XRGrabInteractable>();

        track.Add(instance);
        return instance;
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
