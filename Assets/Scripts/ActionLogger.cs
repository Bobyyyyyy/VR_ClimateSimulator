using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class ActionLogger : MonoBehaviour
{
    public static ActionLogger Instance;

    [Tooltip("Nome del file unico in cui si accumulano tutte le sessioni, nella cartella Logs/ accanto ad Assets (root del progetto in Editor, accanto all'eseguibile in build)")]
    public string fileName = "results_log.csv";

    public string Username { get; private set; } = "Unknown";

    public event Action OnSessionComplete;

    private readonly List<(string TaskName, float Seconds)> taskDurations = new List<(string, float)>();
    public IReadOnlyList<(string TaskName, float Seconds)> TaskDurations => taskDurations;

    private string filePath;
    private string currentTaskName = "";
    private float currentTaskStartTime;
    private bool sessionStarted = false;

    void Awake()
    {
        Instance = this;

        string folder = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Logs");
        Directory.CreateDirectory(folder);

        filePath = Path.Combine(folder, fileName);

        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "sep=;" + Environment.NewLine
                + "Utente;Timestamp;Task;Azione;Dettagli;TempoTask(s);Punteggio" + Environment.NewLine);
        }
    }

    void Start()
    {
        if (TaskManager.Instance != null)
            TaskManager.Instance.onTaskChanged.AddListener(OnTaskChanged);
    }

    void OnDestroy()
    {
        if (TaskManager.Instance != null)
            TaskManager.Instance.onTaskChanged.RemoveListener(OnTaskChanged);
    }

    public void BeginSession(string username)
    {
        Username = string.IsNullOrEmpty(username) ? "Unknown" : username;
        sessionStarted = true;
        OnTaskChanged(TaskManager.Instance != null ? TaskManager.Instance.CurrentTask : null);
    }

    private void OnTaskChanged(TaskData newTask)
    {
        if (!sessionStarted) return;

        if (!string.IsNullOrEmpty(currentTaskName))
            taskDurations.Add((currentTaskName, ElapsedInCurrentTask()));

        currentTaskName = newTask != null ? newTask.taskName : "Day completed";
        currentTaskStartTime = Time.time;
        LogAction(currentTaskName, "Task avviata", "");

        if (newTask == null)
            OnSessionComplete?.Invoke();
    }

    public float ElapsedInCurrentTask() => Time.time - currentTaskStartTime;

    public void LogAction(string taskName, string action, string details)
    {
        if (string.IsNullOrEmpty(filePath) || !sessionStarted) return;

        int score = TaskManager.Instance != null ? TaskManager.Instance.getPoints() : 0;

        string row = string.Join(";", new[]
        {
            Escape(Username),
            Escape(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)),
            Escape(taskName),
            Escape(action),
            Escape(details),
            Escape(ElapsedInCurrentTask().ToString("F1", CultureInfo.InvariantCulture)),
            Escape(score.ToString(CultureInfo.InvariantCulture))
        });

        File.AppendAllText(filePath, row + Environment.NewLine);
    }

    private static string Escape(string field)
    {
        if (string.IsNullOrEmpty(field)) return "";
        if (field.IndexOfAny(new[] { ';', '"', '\n' }) >= 0)
            return "\"" + field.Replace("\"", "\"\"") + "\"";
        return field;
    }
}
