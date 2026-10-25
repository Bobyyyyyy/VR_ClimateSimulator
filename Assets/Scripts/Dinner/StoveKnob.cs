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

    public bool IsOn => isOn;

    void Update()
    {
        currentAngle = axis switch
        {
            RotationAxis.X => NormalizeAngle(transform.localEulerAngles.x),
            RotationAxis.Y => NormalizeAngle(transform.localEulerAngles.y),
            RotationAxis.Z => NormalizeAngle(transform.localEulerAngles.z),
            _ => 0f
        };

        isOn = Mathf.Abs(currentAngle) >= onThreshold;
    }

    private float NormalizeAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}