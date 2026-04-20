using UnityEngine;

public class StoveKnob : MonoBehaviour
{
    public enum RotationAxis { X, Y, Z }

    [Header("Impostazioni Manopola")]
    [Tooltip("Asse locale su cui ruota la manopola")]
    public RotationAxis axis = RotationAxis.Y;

    [Tooltip("Gradi necessari per considerare la manopola 'accesa'")]
    public float onThreshold = 80f;

    [Header("Debug")]
    [SerializeField] private float currentAngle;
    [SerializeField] private bool isOn;

    /// <summary>
    /// True quando la manopola è ruotata almeno di onThreshold gradi.
    /// Viene letto da CookableFood.
    /// </summary>
    public bool IsOn => isOn;

    void Update()
    {
        // Legge la rotazione locale sull'asse scelto
        currentAngle = axis switch
        {
            RotationAxis.X => NormalizeAngle(transform.localEulerAngles.x),
            RotationAxis.Y => NormalizeAngle(transform.localEulerAngles.y),
            RotationAxis.Z => NormalizeAngle(transform.localEulerAngles.z),
            _ => 0f
        };

        isOn = Mathf.Abs(currentAngle) >= onThreshold;
    }

    /// <summary>
    /// Converte un angolo 0-360 in -180/+180 così il confronto funziona
    /// sia ruotando in senso orario che antiorario.
    /// </summary>
    private float NormalizeAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}