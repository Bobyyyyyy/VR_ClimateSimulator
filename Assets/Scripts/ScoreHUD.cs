using UnityEngine;
using TMPro;

public class ScoreHUD : MonoBehaviour
{
    public TextMeshProUGUI generalScoreText;
    public TextMeshProUGUI trashScoreText;
    public TextMeshProUGUI remainingTrashText;
    public TextMeshProUGUI currentTaskText;
    public TextMeshProUGUI timerText;

    void Awake()
    {
        var cam = Camera.main;
        if (cam != null && transform.parent != cam.transform)
            transform.SetParent(cam.transform, false);
    }

    // Promemoria dei comandi, volutamente generici (valgono per qualsiasi task): le azioni
    // sono quelle del rig, Select = Grip e Activate = Trigger su entrambi i controller.
    private const string ControlHints = "Grip button: grab\nTrigger button: use";

    private TextMeshProUGUI controlHints;

    // Il pannello dello username occupa la stessa zona: i comandi compaiono solo dopo la conferma.
    void Update()
    {
        if (controlHints != null)
            controlHints.gameObject.SetActive(!UsernamePrompt.IsActive);
    }

    private void BuildControlHints()
    {
        if (currentTaskText == null) return;

        GameObject go = new GameObject("Control Hints", typeof(RectTransform));
        go.transform.SetParent(transform, false);

        RectTransform rect = (RectTransform)go.transform;
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0f, -30f);
        rect.sizeDelta = new Vector2(380f, 60f);

        TextMeshProUGUI hints = controlHints = go.AddComponent<TextMeshProUGUI>();
        hints.font = currentTaskText.font;
        hints.fontSharedMaterial = currentTaskText.fontSharedMaterial;
        hints.fontSize = 18;
        hints.color = currentTaskText.color;
        hints.alignment = TextAlignmentOptions.Center;
        hints.text = ControlHints;
    }

    void Start()
    {
        BuildControlHints();

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

        if (TimerManager.Instance != null)
        {
            TimerManager.Instance.onElapsedTimeChanged.AddListener(OnElapsedTimeChanged);
            OnElapsedTimeChanged(TimerManager.Instance.ElapsedTime);
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

        if (TimerManager.Instance != null)
        {
            TimerManager.Instance.onElapsedTimeChanged.RemoveListener(OnElapsedTimeChanged);
        }
    }

    private void OnGeneralScoreChanged(int points)
    {
        if (generalScoreText != null) generalScoreText.text = $"Score: {points}";
    }

    private void OnTrashScoreChanged(int score)
    {
        if (trashScoreText != null) trashScoreText.text = $"Trash: {score}";
    }

    private void OnRemainingTrashChanged(int remaining)
    {
        if (remainingTrashText != null) remainingTrashText.text = $"Remaining trash: {remaining}";
    }

    private void OnCurrentTaskChanged(TaskData currentTask)
    {
        if (currentTaskText == null) return;

        currentTaskText.text = currentTask == null
            ? "All tasks completed!"
            : $"{currentTask.taskName}\n{currentTask.description}";
    }

    private void OnElapsedTimeChanged(float elapsedTime)
    {
        if (timerText != null) timerText.text = $"Time: {TimerManager.Instance.GetFormattedTime()}";
    }
}
