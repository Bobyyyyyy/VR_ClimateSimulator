using UnityEngine;

public class StoveBurner : MonoBehaviour
{
    [Tooltip("La manopola che controlla questo fuoco")]
    public StoveKnob stoveKnob;

    /// <summary>
    /// True se la manopola di questo fuoco è girata.
    /// </summary>
    public bool IsActive => stoveKnob != null && stoveKnob.IsOn;
}