using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance;
    public List<TaskData> activeTasks;
    public int completedTasks = 0;

    void Awake() => Instance = this;

    public void UpdateTaskProgress(TaskData completedTask)
    {
        TaskData task = activeTasks.Find(task => task.name == completedTask.name);

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