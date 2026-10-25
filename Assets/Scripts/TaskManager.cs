using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public enum World_State
{
    Good,
    Neutral_Good,
    Neutral,

    Neutral_Bad,
    Bad
}

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance;

    [Header("Sequenza della giornata: l'ordine di questa lista è l'ordine dei task")]
    public List<TaskData> activeTasks;
    public int completedTasks = 0;
    public int currentTaskIndex = 0;

    [Header("Punteggio")]
    public int pointsPerChoice = 10;

    public World_State current_state = World_State.Neutral;

    public int goodThreshold = 10;
    public int badThreshold = -10;

    public UnityEvent onWorldGood;
    public UnityEvent onWorldNeutralGood;

    public UnityEvent onWorldNeutral;
    public UnityEvent onWorldBad;
    public UnityEvent onWorldNeutralBad;

    public UnityEvent<int> onPointsChanged;
    public UnityEvent<TaskData> onTaskChanged;


    private int points = 0;

    void Awake()
    {
        Instance = this;

        if (activeTasks != null)
        {
            foreach (var task in activeTasks)
                if (task != null) task.isCompleted = false;
        }
    }

    public int getPoints() => points;

    public TaskData CurrentTask =>
        (activeTasks != null && currentTaskIndex >= 0 && currentTaskIndex < activeTasks.Count)
            ? activeTasks[currentTaskIndex]
            : null;

    public bool IsCurrentTask(TaskData task) => task != null && CurrentTask == task;


    public void addPoints(int amount)
    {
        points += amount;
        onPointsChanged?.Invoke(points);
        EvaluateWorld();
    }

    public void removePoints(int amount)
    {
        points -= amount;
        onPointsChanged?.Invoke(points);
        EvaluateWorld();
    }

    private void EvaluateWorld()
    {
        World_State newState;

        if (points >= goodThreshold)
            newState = World_State.Good;
        else if (points >= goodThreshold / 2)
            newState = World_State.Neutral_Good;
        else if (points <= badThreshold)
            newState = World_State.Bad;
        else if (points <= badThreshold / 2)
            newState = World_State.Neutral_Bad;
        else
            newState = World_State.Neutral;


        if (newState != current_state)
        {
            current_state = newState;
            switch (current_state)
            {
                case World_State.Good:
                    onWorldGood?.Invoke();
                    break;
                case World_State.Neutral_Good:
                    onWorldNeutralGood?.Invoke();
                    break;
                case World_State.Neutral:
                    onWorldNeutral?.Invoke();
                    break;
                case World_State.Neutral_Bad:
                    onWorldNeutralBad?.Invoke();
                 break;
                case World_State.Bad:
                    onWorldBad?.Invoke();
                    break;
                
            }
        }
    }

    public void UpdateTaskProgress(TaskData completedTask)
    {
        TaskData task = activeTasks.Find(task => task.name == completedTask.name);

        if (task != null)
        {
            if (completedTask.isCompleted)
                completedTasks++;

            else
                completedTasks--;

            task.isCompleted = completedTask.isCompleted;

            if (completedTask.isCompleted && IsCurrentTask(task))
            {
                currentTaskIndex++;
                onTaskChanged?.Invoke(CurrentTask);
            }
        }
        EvaluateWorld();
        CheckTasksStatus();
    }

    void CheckTasksStatus()
    {
        if (completedTasks >= activeTasks.Count)
        {
            Debug.Log("All tasks complete!");
            EvaluateWorld();
        }
    }


}