using UnityEngine;


public class DinnerTask : MonoBehaviour
{
    public static DinnerTask Instance;
    public TaskData dinnerTask;

    [Header("Socket del piatto")]
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor burgerSocket;
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor eggSocket;

    private bool burgerReady = false;
    private bool eggReady = false;
    private bool taskCompleted = false;

    void Awake() => Instance = this;

    void Start()
    {
        // Reset dello stato perché gli ScriptableObject mantengono i valori tra le sessioni
        if (dinnerTask != null)
        {
            dinnerTask.isCompleted = false;
        }
    }

    void Update()
    {
        if (taskCompleted) return;

        // Controlla continuamente lo stato degli oggetti nei socket
        burgerReady = CheckSocket(burgerSocket);
        eggReady = CheckSocket(eggSocket);

        if (burgerReady && eggReady)
        {
            CompleteTask();
        }
    }

    /// <summary>
    /// Controlla se il socket ha un oggetto cotto (non crudo, non bruciato)
    /// </summary>
    private bool CheckSocket(UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socket)
    {
        if (!socket.hasSelection) return false;

        // Compatibile con tutte le versioni di XR Interaction Toolkit
        foreach (var interactable in socket.interactablesSelected)
        {
            var cookable = interactable.transform.GetComponent<CookableFood>();
            if (cookable != null && cookable.IsCooked && !cookable.IsBurnt)
                return true;
        }

        return false;
    }

    private void CompleteTask()
    {
        if (dinnerTask != null && !dinnerTask.isCompleted)
        {
            taskCompleted = true;
            TaskManager.Instance.UpdateTaskProgress(dinnerTask);
            Debug.Log("Cena pronta! Task completata!");
        }
    }
}