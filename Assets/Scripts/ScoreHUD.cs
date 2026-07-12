using UnityEngine;
using TMPro;

public class ScoreHUD : MonoBehaviour
{
    public TextMeshProUGUI generalScoreText;
    public TextMeshProUGUI trashScoreText;
    public TextMeshProUGUI remainingTrashText;
    public TextMeshProUGUI currentTaskText;

    void Awake()
    {
        var cam = Camera.main;
        if (cam != null && transform.parent != cam.transform)
            transform.SetParent(cam.transform, false);
    }

    void Start()
    {
        if (TaskManager.Instance != null)
        {
            TaskManager.Instance.onPointsChanged.AddListener(OnGeneralScoreChanged);
            OnGeneralScoreChanged(TaskManager.Instance.getPoints());

            TaskManager.Instance.onTaskChanged.AddListener(OnCurrentTaskChanged);
            OnCurrentTaskChanged(TaskManager.Instance.CurrentTask);
        }

        if (TrashTask.Instance != null)
        {
            TrashTask.Instance.onSortingScoreChanged.AddListener(OnTrashScoreChanged);
            OnTrashScoreChanged(TrashTask.Instance.SortingScore);

            TrashTask.Instance.onRemainingTrashChanged.AddListener(OnRemainingTrashChanged);
            OnRemainingTrashChanged(TrashTask.Instance.RemainingTrash);
        }
        else if (trashScoreText != null)
        {
            OnTrashScoreChanged(0);
        }
    }

    void OnDestroy()
    {
        if (TaskManager.Instance != null)
        {
            TaskManager.Instance.onPointsChanged.RemoveListener(OnGeneralScoreChanged);
            TaskManager.Instance.onTaskChanged.RemoveListener(OnCurrentTaskChanged);
        }

        if (TrashTask.Instance != null)
        {
            TrashTask.Instance.onSortingScoreChanged.RemoveListener(OnTrashScoreChanged);
            TrashTask.Instance.onRemainingTrashChanged.RemoveListener(OnRemainingTrashChanged);
        }
    }

    private void OnGeneralScoreChanged(int points)
    {
        if (generalScoreText != null) generalScoreText.text = $"Punteggio: {points}";
    }

    private void OnTrashScoreChanged(int score)
    {
        if (trashScoreText != null) trashScoreText.text = $"Spazzatura: {score}";
    }

    private void OnRemainingTrashChanged(int remaining)
    {
        if (remainingTrashText != null) remainingTrashText.text = $"Rifiuti rimasti: {remaining}";
    }

    private void OnCurrentTaskChanged(TaskData currentTask)
    {
        if (currentTaskText == null) return;

        currentTaskText.text = currentTask == null
            ? "Tutte le task completate!"
            : $"{currentTask.taskName}\n{currentTask.description}";
    }
}
