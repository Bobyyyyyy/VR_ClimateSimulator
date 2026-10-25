using UnityEngine;

public class TaskGatedVisibility : MonoBehaviour
{
    [Tooltip("Il task che deve essere completato prima che l'oggetto diventi visibile")]
    public TaskData requiredTask;

    [Tooltip("Oggetto da mostrare/nascondere. Se vuoto, usa questo stesso GameObject")]
    public GameObject target;

    void Start()
    {
        if (target == null) target = gameObject;

        UpdateVisibility();

        if (TaskManager.Instance != null)
            TaskManager.Instance.onTaskChanged.AddListener(OnTaskChanged);
    }

    void OnDestroy()
    {
        if (TaskManager.Instance != null)
            TaskManager.Instance.onTaskChanged.RemoveListener(OnTaskChanged);
    }

    private void OnTaskChanged(TaskData _) => UpdateVisibility();

    private void UpdateVisibility()
    {
        bool shouldShow = requiredTask == null || requiredTask.isCompleted;
        target.SetActive(shouldShow);
    }
}
