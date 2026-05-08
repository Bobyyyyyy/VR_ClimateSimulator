using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public enum World_State
{
    Good,
    Neutral,
    Bad
}

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance;
    public List<TaskData> activeTasks;
    public int completedTasks = 0;

    public World_State current_state = World_State.Neutral;

    public int goodThreshold = 10;
    public int badThreshold = -10;

    public UnityEvent onWorldGood;
    public UnityEvent onWorldNeutral;
    public UnityEvent onWorldBad;
    public UnityEvent<int> onPointsChanged;

    private int points = 0;

    void Awake() => Instance = this;

    public int getPoints() => points;

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
        else if (points <= badThreshold)
            newState = World_State.Bad;
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
                case World_State.Neutral:
                    onWorldNeutral?.Invoke();
                    break;
                case World_State.Bad:
                    onWorldBad?.Invoke();
                    break;
            }
        }
    }

    public void UpdateTaskProgress(TaskData completedTask)
    {
        TaskData task = activeTasks.Find(task => task.name == completedTask.name && task.isCompleted != completedTask.isCompleted);

        if (task != null)
        {
            if (completedTask.isCompleted)
                completedTasks++;
            else
                completedTasks--;

            task.isCompleted = completedTask.isCompleted;
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