using System;
using System.Globalization;
using System.IO;
using UnityEngine;

// Registra su file (CSV) ogni azione rilevante compiuta dall'utente durante la sessione,
// una riga per azione: quando è avvenuta, per quale task, cosa è successo, con quali
// dettagli specifici e il punteggio generale in quel momento. Ogni task-script chiama
// LogAction(...) nei propri punti di scelta/completamento (vedi LightChoiceTask, BinDetector,
// TrashTask, BreakfastTask, FridgeDinnerMenu, VRLoadingOverlay, ThermostatTask/Controller).
// La sessione (e quindi il logging vero e proprio) parte solo dopo BeginSession(username),
// chiamato da UsernamePrompt una volta che il giocatore ha confermato il proprio nome.
public class ActionLogger : MonoBehaviour
{
    public static ActionLogger Instance;

    [Tooltip("Nome del file unico in cui si accumulano tutte le sessioni, nella cartella Logs/ accanto ad Assets (root del progetto in Editor, accanto all'eseguibile in build)")]
    public string fileName = "results_log.csv";

    public string Username { get; private set; } = "Sconosciuto";

    private string filePath;
    private float currentTaskStartTime;
    private bool sessionStarted = false;

    void Awake()
    {
        Instance = this;

        // Application.dataPath è ".../Assets" in Editor e ".../<Prodotto>_Data" in build:
        // risalendo di un livello si ottiene rispettivamente la root del progetto o la cartella
        // accanto all'eseguibile, entrambe facili da trovare senza scavare in AppData.
        string folder = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Logs");
        Directory.CreateDirectory(folder);

        filePath = Path.Combine(folder, fileName);

        // Un unico file accumula tutte le sessioni: lo creiamo con l'intestazione solo se non
        // esiste ancora, altrimenti le righe successive vanno semplicemente in coda a quelle
        // già presenti (File.AppendAllText in LogAction non tronca mai il file).
        if (!File.Exists(filePath))
        {
            // "sep=;" è un'estensione riconosciuta da Excel per forzare il delimitatore di colonna
            // a prescindere dal separatore di lista configurato nel Pannello di controllo/locale
            // (in italiano è ";" perché "," è il separatore decimale, quindi un CSV con "," come
            // delimitatore viene aperto da Excel IT come un'unica colonna).
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

    // Chiamato da UsernamePrompt dopo la conferma dello username: da qui in poi ogni riga del
    // log riporta l'utente, e il task corrente (già selezionato da TaskManager all'avvio) viene
    // registrato come "avviato" solo adesso, non prima che l'utente si sia identificato.
    public void BeginSession(string username)
    {
        Username = string.IsNullOrEmpty(username) ? "Sconosciuto" : username;
        sessionStarted = true;
        OnTaskChanged(TaskManager.Instance != null ? TaskManager.Instance.CurrentTask : null);
    }

    private void OnTaskChanged(TaskData newTask)
    {
        if (!sessionStarted) return;

        string taskName = newTask != null ? newTask.taskName : "Giornata completata";
        currentTaskStartTime = Time.time;
        LogAction(taskName, "Task avviata", "");
    }

    // Secondi trascorsi dall'inizio del task correntemente attivo: usato per popolare
    // "TempoTask(s)" su ogni riga senza che ogni singolo script debba tenere un proprio timer.
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
