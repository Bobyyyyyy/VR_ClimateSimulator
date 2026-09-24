using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BreakfastTask : MonoBehaviour
{
    public static BreakfastTask Instance;

    [Header("Stazioni già presenti in scena (disattivate di default)")]
    public GameObject JuiceStation;
    public GameObject ToastStation;
    public GameObject SnackStation;

    public GameObject BreakfastCanvas;
    public GameObject StationSpawnPoint;
    public GameObject Bell;
    public GameObject BellSpawnPoint;

    public TaskData task;

    [Tooltip("Quanti oggetti dello spuntino bisogna mangiare/bere per completare il percorso (non serve finire tutta la Snack Station)")]
    public int minSnackItemsToEat = 5;

    private List<GameObject> instantiatedObjects = new List<GameObject>();
    private List<GameObject> snackItems = new List<GameObject>();
    private int initialSnackItemCount = 0;
    private GameObject bellInstance;
    private bool isHealthy;
    private int step = 0;
    private bool[] stepFlags = new bool[2];

    void Awake() => Instance = this;

    void Start()
    {
        if (JuiceStation != null) JuiceStation.SetActive(false);
        if (ToastStation != null) ToastStation.SetActive(false);
        if (SnackStation != null) SnackStation.SetActive(false);

        UpdateCanvasVisibility();

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
        UpdateCanvasVisibility();
    }

    // Il canvas compare solo quando è il turno del task colazione (spazzatura completata).
    private void UpdateCanvasVisibility()
    {
        if (BreakfastCanvas == null) return;

        bool shouldShow = task != null && !task.isCompleted
            && TaskManager.Instance != null && TaskManager.Instance.IsCurrentTask(task);

        BreakfastCanvas.SetActive(shouldShow);
    }

    // Il percorso spuntino non ha uno step di preparazione (il cibo è già pronto da mangiare),
    // quindi la campanella deve poter far scattare il controllo di completamento in ogni momento.
    public bool IsCurrentStepReady() => !isHealthy || stepFlags[step];

    [ContextMenu("Test Completion Step")]
    public void TryCompleteCurrentStep()
    {   
        Debug.Log(step);
        Debug.Log(IsCurrentStepReady());
        if (IsCurrentStepReady())
            CompleteStep(step);
    }

    public void BeginTask(bool isHealthy)
    {
        this.isHealthy = isHealthy;

        ActionLogger.Instance?.LogAction(task != null ? task.taskName : "Colazione", "Scelta colazione",
            $"Percorso scelto={(isHealthy ? "sano (succo + toast)" : "spuntino")}");

        if (isHealthy)
        {
            // Colazione sana: juice station e toast station appaiono insieme, subito.
            if (JuiceStation != null)
            {
                JuiceStation.SetActive(true);
                instantiatedObjects.Add(JuiceStation);
            }
            if (ToastStation != null)
            {
                ToastStation.SetActive(true);
                instantiatedObjects.Add(ToastStation);
            }
        }
        else if (SnackStation != null)
        {
            // Percorso spuntino: gli oggetti dentro la Snack Station sono già pronti da mangiare
            // (già taggati Food/Beverage e afferrabili), quindi basta tracciarli per sapere quando
            // sono stati tutti consumati.
            SnackStation.SetActive(true);
            snackItems.AddRange(FindConsumables(SnackStation));
            initialSnackItemCount = snackItems.Count;
        }

        if (Bell != null && BellSpawnPoint != null)
        {
            bellInstance = Instantiate(Bell, BellSpawnPoint.transform.position, Bell.transform.rotation);
            instantiatedObjects.Add(bellInstance);
        }

        Destroy(BreakfastCanvas);
    }

    private GameObject FindChildInInstances(string objectName)
    {
        foreach (var instance in instantiatedObjects)
        {
            if (instance == null) continue;
            Transform[] allChildren = instance.GetComponentsInChildren<Transform>(true);
            Transform found = System.Array.Find(allChildren, t => t.name == objectName);
            if (found != null) return found.gameObject;
        }
        return null;
    }

    private List<GameObject> FindConsumables(GameObject station)
    {
        var result = new List<GameObject>();
        foreach (Transform t in station.GetComponentsInChildren<Transform>(true))
        {
            if (t.CompareTag("Food") || t.CompareTag("Beverage"))
                result.Add(t.gameObject);
        }
        return result;
    }

    [ContextMenu("Set Juice Task Ready")]
    public void FinishJuice() => stepFlags[0] = true;

    [ContextMenu("Set Toast Task Ready")]
    public void FinishToast() => stepFlags[1] = true;

    public void CompleteStep(int step)
    {   
        if(isHealthy)
        {
            switch (step)
        {
            case 0:
                if (this.step == 0)
                {
                    GameObject mug = FindChildInInstances("mug");
                    if (mug != null)
                    {
                        if (!mug.TryGetComponent<Rigidbody>(out _)) mug.AddComponent<Rigidbody>();
                        if (!mug.TryGetComponent<XRGrabInteractable>(out _)) mug.AddComponent<XRGrabInteractable>();
                        mug.tag = "Beverage";
                    }
                    else
                    {
                        // Il mug è stato bevuto (consumato): nascondi di nuovo la juice station.
                        if (JuiceStation != null)
                        {
                            instantiatedObjects.Remove(JuiceStation);
                            JuiceStation.SetActive(false);
                        }

                        this.step++;
                    }
                }
                break;
            case 1:
                if (this.step == 1)
                {
                    GameObject toast = FindChildInInstances("Toast");
                    if (toast != null)
                    {
                        if (!toast.TryGetComponent<Rigidbody>(out _)) toast.AddComponent<Rigidbody>();
                        if (!toast.TryGetComponent<XRGrabInteractable>(out _)) toast.AddComponent<XRGrabInteractable>();
                        toast.tag = "Food";
                    }
                    else
                    {
                        // Il toast è stato mangiato: nascondi di nuovo la toast station e togli la campanella.
                        if (ToastStation != null)
                        {
                            instantiatedObjects.Remove(ToastStation);
                            ToastStation.SetActive(false);
                        }
                        if (bellInstance != null)
                        {
                            instantiatedObjects.Remove(bellInstance);
                            Destroy(bellInstance);
                        }

                        this.step++;
                        CompleteTask();
                    }
                }
                break;

        }

        }
        
        else
        {
            snackItems.RemoveAll(obj => obj == null);
            int eaten = initialSnackItemCount - snackItems.Count;

            // Non serve finire tutta la Snack Station: basta averne mangiati/bevuti almeno
            // minSnackItemsToEat (o proprio tutti, se ce ne sono meno della soglia).
            if (eaten >= minSnackItemsToEat || snackItems.Count == 0)
            {
                if (SnackStation != null) SnackStation.SetActive(false);
                if (bellInstance != null)
                {
                    instantiatedObjects.Remove(bellInstance);
                    Destroy(bellInstance);
                }

                CompleteTask();
            }
        }

    }

    public void CompleteTask()
    {
        if (task == null || task.isCompleted) return;
        if (TaskManager.Instance != null && !TaskManager.Instance.IsCurrentTask(task)) return;

        task.isCompleted = true;
        TaskManager.Instance.addPoints(TaskManager.Instance.pointsPerChoice);

        ActionLogger.Instance?.LogAction(task.taskName, "Task completata",
            $"Percorso={(isHealthy ? "sano" : "spuntino")}");

        TaskManager.Instance.UpdateTaskProgress(task);
    }

    [ContextMenu("Debug Print All Children Names")]
    public void DebugPrintAllChildren()
    {
        foreach (var instance in instantiatedObjects)
        {
            if (instance == null) continue;
            Transform[] allChildren = instance.GetComponentsInChildren<Transform>(true);
            foreach (var child in allChildren)
                Debug.Log($"[{instance.name}] → '{child.name}'");
        }
    }
}