using UnityEngine;

public class TrashTask : MonoBehaviour
{
    public static TrashTask Instance;
    public TaskData trashTask;

    /// <summary>
    /// Counts the remaining trash in scene
    /// </summary>
    int remainingTrash; 

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
            CompleteTask();
        }
    }

    private void CompleteTask()
    {
        if (trashTask != null && !trashTask.isCompleted)
        {
            trashTask.isCompleted = true;
            TaskManager.Instance.UpdateTaskProgress(trashTask);
        }
    }
}