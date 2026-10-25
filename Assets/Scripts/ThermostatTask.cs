using UnityEngine;

public class ThermostatTask : MonoBehaviour
{
    public static ThermostatTask Instance;

    public TaskData task;

    private bool choiceMade = false;

    void Awake() => Instance = this;

    public void TurnOff() => MakeChoice(ecological: true);

    public void LeaveOn() => MakeChoice(ecological: false);

    private void MakeChoice(bool ecological)
    {
        if (choiceMade || task == null || TaskManager.Instance == null) return;
        if (!TaskManager.Instance.IsCurrentTask(task)) return;

        choiceMade = true;

        if (ecological)
            TaskManager.Instance.addPoints(TaskManager.Instance.pointsPerChoice);
        else
            TaskManager.Instance.removePoints(TaskManager.Instance.pointsPerChoice);

        task.isCompleted = true;

        ActionLogger.Instance?.LogAction(task.taskName, "Task completata",
            $"Scelta termostato={(ecological ? "spento (ecologica)" : "lasciato acceso (non ecologica)")}");

        TaskManager.Instance.UpdateTaskProgress(task);
    }
}
