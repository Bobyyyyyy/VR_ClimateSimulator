using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BreakfastTask : MonoBehaviour
{
    public static BreakfastTask Instance;

    public GameObject JuiceStation;
    public GameObject ToastStation;
    public GameObject SnackStation;
    public GameObject BreakfastCanvas;
    public GameObject StationSpawnPoint;
    public GameObject Bell;
    public GameObject BellSpawnPoint;

    public TaskData task;

    private List<GameObject> instantiatedObjects = new List<GameObject>();
    private GameObject currentStation;
    private bool isHealthy;
    private int step = 0;
    private bool[] stepFlags = new bool[2];

    void Awake() => Instance = this;

    public bool IsCurrentStepReady() => stepFlags[step];

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
        if (isHealthy)
        {
            currentStation = Instantiate(JuiceStation, StationSpawnPoint.transform.position,StationSpawnPoint.transform.rotation);
            instantiatedObjects.Add(currentStation);
            instantiatedObjects.Add(Instantiate(Bell, BellSpawnPoint.transform.position, Bell.transform.rotation));
        }
        else
        {
            currentStation = Instantiate(SnackStation, StationSpawnPoint.transform.position,StationSpawnPoint.transform.rotation);
            instantiatedObjects.Add(currentStation);
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

                    if(mug == null)
                    {
                            if (currentStation != null)
                        {
                            instantiatedObjects.Remove(currentStation);
                            Destroy(currentStation);
                        }

                        if (isHealthy)
                        {
                            currentStation = Instantiate(ToastStation);
                            instantiatedObjects.Add(currentStation);
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
                    
                    if(toast == null)
                    {
                        if (currentStation != null)
                        {
                            instantiatedObjects.Remove(currentStation);
                            instantiatedObjects.Remove(Bell);
                            Destroy(currentStation);
                            Destroy(Bell);
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
            instantiatedObjects.RemoveAll(obj => obj == null);

            if (instantiatedObjects.Count == 0)
            {
                CompleteTask();
            }
        }
        
    }

    public void CompleteTask()
    {
        if (task == null || task.isCompleted) return;
        if (TaskManager.Instance != null && !TaskManager.Instance.IsCurrentTask(task)) return;

        Debug.Log("Task Completata!");
        task.isCompleted = true;
        TaskManager.Instance.addPoints(TaskManager.Instance.pointsPerChoice);
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