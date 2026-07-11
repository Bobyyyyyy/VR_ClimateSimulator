using UnityEngine;
using System.Collections.Generic;

public class TrashTask : MonoBehaviour
{
    public static TrashTask Instance;
    public TaskData trashTask;
    public List<GameObject> prefabs;
     

    /// <summary>
    /// Counts the remaining trash in scene
    /// </summary>
    int remainingTrash; 

    void Awake() => Instance = this;



    public void beginTask()
    {
        foreach (var prefab in prefabs) {
            Instantiate(prefab);
        }
        remainingTrash = GameObject.FindGameObjectsWithTag("Trash").Length;
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
        if (trashTask == null || trashTask.isCompleted) return;
        if (TaskManager.Instance != null && !TaskManager.Instance.IsCurrentTask(trashTask)) return;

        trashTask.isCompleted = true;
        TaskManager.Instance.addPoints(TaskManager.Instance.pointsPerChoice);
        TaskManager.Instance.UpdateTaskProgress(trashTask);
        Destroy(this);
    }
}