using UnityEngine;
public enum TrashType { Plastic, Paper, Glass, undifferentiated }

[CreateAssetMenu(fileName = "Rifiuto", menuName = "Recycling/Trash Data")]
public class TrashData : ScriptableObject
{
    [Header("Infos")]
    public string objectName; 

    [Header("GameLogic")]
    public TrashType type;    

    [Header("VR & Feedback")]
    public AudioClip collisionSound;


}