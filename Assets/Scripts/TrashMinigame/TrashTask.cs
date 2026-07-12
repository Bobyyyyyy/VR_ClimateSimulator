using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

[System.Serializable]
public class TrashSpawnEntry
{
    public GameObject prefab;
    [Tooltip("Posizione/rotazione specifica per questo rifiuto. Se vuoto, usa lo Spawn Point generale (con dispersione) o, in mancanza anche di quello, la posizione originale del prefab.")]
    public Transform spawnPoint;
}

public class TrashTask : MonoBehaviour
{
    public static TrashTask Instance;
    public TaskData trashTask;
    public List<TrashSpawnEntry> prefabs;

    [Header("Spawn")]
    [Tooltip("Punto di fallback in cui generare i rifiuti che non hanno un proprio Spawn Point. Se vuoto, ognuno di questi compare alla posizione originale del suo prefab (sparsa e non affidabile).")]
    public Transform spawnPoint;
    [Tooltip("Raggio di dispersione casuale attorno al punto di spawn di fallback")]
    public float spawnScatterRadius = 0.3f;

    [Header("Punteggio interno di differenziazione")]
    [Tooltip("Punti guadagnati per ogni oggetto gettato nel cestino corretto")]
    public int pointsPerCorrectSort = 1;
    [Tooltip("Punti persi per ogni oggetto gettato nell'indifferenziata")]
    public int pointsPerWrongSort = 1;
    [Tooltip("Se il punteggio interno a fine task è >= a questa soglia, la task è considerata fatta bene")]
    public int sortingScoreThreshold = 0;

    public UnityEvent<int> onSortingScoreChanged;
    public UnityEvent<int> onRemainingTrashChanged;

    public int SortingScore => sortingScore;
    public int RemainingTrash => remainingTrash;

    private int sortingScore = 0;

    /// <summary>
    /// Counts the remaining trash in scene
    /// </summary>
    int remainingTrash;

    private bool hasBegun = false;

    void Awake() => Instance = this;

    void Start()
    {
        if (TaskManager.Instance != null)
            TaskManager.Instance.onTaskChanged.AddListener(OnTaskChanged);

        TryBeginIfCurrent();
    }

    void OnDestroy()
    {
        if (TaskManager.Instance != null)
            TaskManager.Instance.onTaskChanged.RemoveListener(OnTaskChanged);
    }

    private void OnTaskChanged(TaskData _) => TryBeginIfCurrent();

    // La task spazzatura parte da sola appena diventa quella corrente (luce completata),
    // stesso pattern di TaskGatedVisibility/BreakfastTask per mostrare/attivare contenuti a turno.
    private void TryBeginIfCurrent()
    {
        if (hasBegun || trashTask == null || trashTask.isCompleted) return;
        if (TaskManager.Instance == null || !TaskManager.Instance.IsCurrentTask(trashTask)) return;

        hasBegun = true;
        beginTask();
    }

    public void beginTask()
    {
        foreach (var entry in prefabs)
        {
            if (entry == null || entry.prefab == null) continue;

            if (entry.spawnPoint != null)
            {
                Instantiate(entry.prefab, entry.spawnPoint.position, entry.spawnPoint.rotation);
            }
            else if (spawnPoint != null)
            {
                Vector2 offset = Random.insideUnitCircle * spawnScatterRadius;
                Vector3 position = spawnPoint.position + new Vector3(offset.x, 0f, offset.y);
                Instantiate(entry.prefab, position, spawnPoint.rotation);
            }
            else
            {
                Instantiate(entry.prefab);
            }
        }
        remainingTrash = GameObject.FindGameObjectsWithTag("Trash").Length;
        sortingScore = 0;
        onSortingScoreChanged?.Invoke(sortingScore);
        onRemainingTrashChanged?.Invoke(remainingTrash);
    }

    public void NotifyCorrectSort()
    {
        sortingScore += pointsPerCorrectSort;
        onSortingScoreChanged?.Invoke(sortingScore);
    }

    public void NotifyWrongSort()
    {
        sortingScore -= pointsPerWrongSort;
        onSortingScoreChanged?.Invoke(sortingScore);
    }

    public void NotifyTrashInsertion()
    {
        remainingTrash--;
        onRemainingTrashChanged?.Invoke(remainingTrash);
        if (remainingTrash <= 0)
        {
            CompleteTask();
        }
    }

    private void CompleteTask()
    {
        if (trashTask == null || trashTask.isCompleted) return;
        if (TaskManager.Instance != null && !TaskManager.Instance.IsCurrentTask(trashTask)) return;

        trashTask.isCompleted = true;

        if (sortingScore >= sortingScoreThreshold)
            TaskManager.Instance.addPoints(TaskManager.Instance.pointsPerChoice);
        else
            TaskManager.Instance.removePoints(TaskManager.Instance.pointsPerChoice);

        TaskManager.Instance.UpdateTaskProgress(trashTask);
        Destroy(this);
    }
}
