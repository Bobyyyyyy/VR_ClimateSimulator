using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BreakfastTask : MonoBehaviour
{
    public static BreakfastTask Instance;

    public GameObject JuiceStation;
    public GameObject ToastStation;
    public List<GameObject> SnackPrefabs;
    public GameObject BreakfastCanvas;
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

    [ContextMenu("Start Task")]
    public void BeginTask()
    {
        isHealthy = true;

        if (isHealthy)
        {
            currentStation = Instantiate(JuiceStation);
            instantiatedObjects.Add(currentStation);
        }
        else
        {
            foreach (var prefab in SnackPrefabs)
                instantiatedObjects.Add(Instantiate(prefab));
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
                            Destroy(currentStation);
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
        Debug.Log("Task Completata!");
        if (task != null && !task.isCompleted)
        {
            task.isCompleted = true;
            TaskManager.Instance.UpdateTaskProgress(task);
        }
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