using UnityEngine;
using UnityEngine.Events;

public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance;

    public bool IsRunning { get; private set; }
    public float ElapsedTime { get; private set; }

    public UnityEvent<float> onElapsedTimeChanged;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (TaskManager.Instance != null)
            TaskManager.Instance.onTaskChanged.AddListener(OnTaskChanged);
    }

    public void BeginSession()
    {
        IsRunning = true;
    }

    void OnDestroy()
    {
        if (TaskManager.Instance != null)
            TaskManager.Instance.onTaskChanged.RemoveListener(OnTaskChanged);
    }

    void Update()
    {
        if (!IsRunning) return;

        ElapsedTime += Time.deltaTime;
        onElapsedTimeChanged?.Invoke(ElapsedTime);
    }

    private void OnTaskChanged(TaskData currentTask)
    {
        if (currentTask == null)
            IsRunning = false;
    }

    public string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(ElapsedTime / 60f);
        int seconds = Mathf.FloorToInt(ElapsedTime % 60f);
        return $"{minutes:00}:{seconds:00}";
    }
}
