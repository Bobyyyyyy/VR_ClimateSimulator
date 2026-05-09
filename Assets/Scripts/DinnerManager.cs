using UnityEngine;


public class DinnerTask : MonoBehaviour
{
    public static DinnerTask Instance;
    public TaskData dinnerTask;

    [Header("Socket del piatto")]
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor paninoSocket;
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor eggSocket;

    private bool paninoReady = false;
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
        paninoReady = CheckPaninoSocket(paninoSocket);
        eggReady = CheckCookedSocket(eggSocket);

        if (paninoReady && eggReady)
        {
            CompleteTask();
        }
    }

    /// <summary>
    /// Controlla se nel socket del panino è presente un oggetto (non deve essere cuocibile)
    /// </summary>
    private bool CheckPaninoSocket(UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socket)
    {
        return socket.hasSelection;
    }

    /// <summary>
    /// Controlla se il socket ha un oggetto cotto (non crudo, non bruciato)
    /// </summary>
    private bool CheckCookedSocket(UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socket)
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