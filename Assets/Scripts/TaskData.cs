using UnityEngine;

[CreateAssetMenu(fileName = "New Task", menuName = "Game/Task")]
public class TaskData : ScriptableObject
{
    public string taskName;
    public string description;
    public bool isCompleted;

}