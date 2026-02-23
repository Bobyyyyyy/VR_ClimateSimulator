using UnityEngine;
public enum TrashType { Plastic, Paper, Glass, undifferentiated }

[CreateAssetMenu(fileName = "Rifiuto", menuName = "Recycling/Trash Data")]
public class TrashData : ScriptableObject
{
    [Header("GameLogic")]
    public TrashType type;    

    [Header("VR & Feedback")]
    public AudioClip collisionSound;


}