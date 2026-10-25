using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
public class DevTaskSkip : MonoBehaviour
{
    public Key skipKey = Key.F9;

    void Update()
    {
        if (Keyboard.current == null || !Keyboard.current[skipKey].wasPressedThisFrame)
            return;

        TaskData task = TaskManager.Instance != null ? TaskManager.Instance.CurrentTask : null;
        if (task == null)
        {
            Debug.Log("[DevTaskSkip] Nessuna task corrente da skippare.");
            return;
        }

        task.isCompleted = true;
        TaskManager.Instance.UpdateTaskProgress(task);
        Debug.Log($"[DevTaskSkip] Task '{task.taskName}' segnata come completata (skip dev).");
    }
}
#endif
