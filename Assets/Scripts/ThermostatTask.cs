using UnityEngine;

public class ThermostatTask : MonoBehaviour
{

    public static ThermostatTask Instance;

    public TaskData task;

    

    public void CompleteTask()
        {
        if (task != null && !task.isCompleted)
        {
            task.isCompleted = true;
            TaskManager.Instance.UpdateTaskProgress(task);
        }
    }
}
