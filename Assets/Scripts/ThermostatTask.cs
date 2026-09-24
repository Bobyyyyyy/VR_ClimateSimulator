using UnityEngine;

public class ThermostatTask : MonoBehaviour
{

    public static ThermostatTask Instance;

    public TaskData task;

        void Awake() => Instance = this;

    public void CompleteTask()
        {
        if (task != null && !task.isCompleted)
        {
            task.isCompleted = true;

            ActionLogger.Instance?.LogAction(task.taskName, "Task completata", "Temperatura ideale raggiunta");

            TaskManager.Instance.UpdateTaskProgress(task);
            Destroy(this);
        }
    }
}
