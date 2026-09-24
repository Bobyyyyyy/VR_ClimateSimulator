using UnityEngine;

public class LightChoiceTask : MonoBehaviour
{
    public TaskData task;

    [Header("Tapparelle: la scelta naturale si conferma solo quando TUTTE sono aperte")]
    public MovimentoLineare[] shutters;

    [Header("Interruttore luce artificiale")]
    public LightSwitchVR lightSwitch;

    private bool choiceMade = false;
    private int shuttersOpenCount = 0;

    // Collegato all'evento onFullyOpen di OGNI tapparella
    public void NotifyShutterOpen()
    {
        shuttersOpenCount++;
        if (shuttersOpenCount >= shutters.Length)
            MakeChoice(positive: true);
    }

    // Collegato all'evento onTurnedOn dell'interruttore
    public void ChooseArtificial()
    {
        MakeChoice(positive: false);
    }

    private void MakeChoice(bool positive)
    {
        if (choiceMade || task == null || TaskManager.Instance == null) return;
        if (!TaskManager.Instance.IsCurrentTask(task)) return;

        choiceMade = true;

        if (positive)
            TaskManager.Instance.addPoints(TaskManager.Instance.pointsPerChoice);
        else
            TaskManager.Instance.removePoints(TaskManager.Instance.pointsPerChoice);

        task.isCompleted = true;

        ActionLogger.Instance?.LogAction(task.taskName, "Task completata",
            $"Luce scelta={(positive ? "naturale (tapparelle)" : "artificiale (interruttore)")}");

        TaskManager.Instance.UpdateTaskProgress(task);

        LockInteractions();
    }

    private void LockInteractions()
    {
        foreach (var shutter in shutters)
            if (shutter != null) shutter.Lock();

        if (lightSwitch != null) lightSwitch.Lock();
    }
}
