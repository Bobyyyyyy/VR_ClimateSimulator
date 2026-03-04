using UnityEngine;

public class TrashTask : MonoBehaviour
{
    public static TrashTask Instance;
    public TaskData trashTask; // Assign your ScriptableObject here

    int remainingTrash; //count the remaining trash in scene

    void Awake() => Instance = this;

    //voglio che la spazzatura non sia sempre nello stesso posto, idea possibile: creare prefab in cui la spazzatura è in posti diversi e creare un rng
    //che sceglie un prefab a caso tra quelli disponibili.

    void Start()
    {
        remainingTrash = GameObject.FindGameObjectsWithTag("Trash").Length;
        Debug.Log(remainingTrash);
    }

    public void NotifyTrashInsertion()
    {
        remainingTrash--;
        if (remainingTrash <= 0)
        {
            CompleteTrashTask();
        }
    }

    private void CompleteTrashTask()
    {
        if (trashTask != null && !trashTask.isCompleted)
        {
            trashTask.isCompleted = true;
            TaskManager.Instance.UpdateTaskProgress(trashTask);
            Debug.Log("Trash deposited! Task Manager notified.");
        }
    }
}