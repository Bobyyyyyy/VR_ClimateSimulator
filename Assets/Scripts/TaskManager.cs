using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance;
    public List<TaskData> activeTasks;
    public int completedTasks = 0;

    private int points;

    void Awake() => Instance = this;

    public int getPoints()
    {
        return points;
    }


    /// <summary>
    /// Checks if the current task is in the activeTasks and needs to be updated, if it is the function updates the completed status of the task.
    /// </summary>
    /// <param name="completedTask"></param>
    public void UpdateTaskProgress(TaskData task)
    {
        if (activeTasks.Contains(task) && !task.isCompleted)
    {
        task.isCompleted = true;
        completedTasks++;
        Debug.Log($"Task completata: {task.taskName} ({completedTasks}/{activeTasks.Count})");
        CheckTasksStatus();
    }
    }



    void CheckTasksStatus()
    {
        if (completedTasks >= activeTasks.Count)
        {
            Debug.Log("All tasks complete!");
        }
    }
}