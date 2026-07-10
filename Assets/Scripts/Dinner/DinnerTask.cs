using System.Collections.Generic;
using UnityEngine;

public class DinnerTask : MonoBehaviour
{
    public static DinnerTask Instance;

    [Header("Stazioni (prefab)")]
    public GameObject BurgerStation;    // stazione panino
    public GameObject TomatoStation;    // stazione pomodori

    [Header("UI iniziale")]
    public GameObject DinnerCanvas;

    [Header("Task")]
    public TaskData task;

    private List<GameObject> instantiatedObjects = new List<GameObject>();
    private GameObject currentStation;
    private int step = 0;
    private bool[] stepFlags = new bool[2];  // 0 = panino, 1 = pomodori

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
        if (BurgerStation != null)
        {
            currentStation = Instantiate(BurgerStation);
            instantiatedObjects.Add(currentStation);
        }

        if (DinnerCanvas != null) Destroy(DinnerCanvas);
    }

    // Chiamato dall'evento onAssemblyComplete del BurgerAssembler
    [ContextMenu("Set Burger Task Ready")]
    public void FinishBurger() => stepFlags[0] = true;

    // Chiamato quando le 8 metà di pomodoro sono tutte negli slot
    [ContextMenu("Set Tomatoes Task Ready")]
    public void FinishTomatoes() => stepFlags[1] = true;

    public void CompleteStep(int step)
    {
        switch (step)
        {
            case 0:
                if (this.step == 0)
                {
                    // Panino completato → distruggi stazione panino, spawna pomodori
                    if (currentStation != null)
                    {
                        instantiatedObjects.Remove(currentStation);
                        Destroy(currentStation);
                    }

                    if (TomatoStation != null)
                    {
                        currentStation = Instantiate(TomatoStation);
                        instantiatedObjects.Add(currentStation);
                    }

                    this.step++;
                }
                break;

            case 1:
                if (this.step == 1)
                {
                    // Pomodori completati → fine task
                    if (currentStation != null)
                    {
                        instantiatedObjects.Remove(currentStation);
                        Destroy(currentStation);
                    }

                    this.step++;
                    CompleteTask();
                }
                break;
        }
    }

    public void CompleteTask()
    {
        Debug.Log("Cena Completata!");
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