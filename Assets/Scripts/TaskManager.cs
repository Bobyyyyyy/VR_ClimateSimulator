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
    public void UpdateTaskProgress(TaskData completedTask)
    {
        TaskData task = activeTasks.Find(task => task.name == completedTask.name && task.isCompleted != completedTask.isCompleted);

        if (task != null)
        {
            if(completedTask.isCompleted) {
                completedTasks++;
            }
            else {
                completedTasks--;
            }
            task.isCompleted = completedTask.isCompleted;
        }

        CheckTasksStatus();
    }



    void CheckTasksStatus()
    {
        if (completedTasks >= activeTasks.Count)
        {
            Debug.Log("All tasks complete!");
        }
    }
}