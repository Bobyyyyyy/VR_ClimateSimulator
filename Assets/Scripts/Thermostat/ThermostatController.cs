using UnityEngine;
using TMPro;
public class ThermostatController : MonoBehaviour
{
    public TextMeshProUGUI displayScreen;
    float targetTemperature;
    public Vector2 idealRange;
    public float currentTemperature;

    private void SetTemperature(float temperature)
    {
        currentTemperature = temperature;
        UpdateDisplay(temperature);
    }

    public float GetTemperature()
    {
        return currentTemperature; 
    }

    public void IncreaseTemperature(float value)
    {
        SetTemperature(currentTemperature+=value);
    }

    public void DecreaseTemperature(float value)
    {
        SetTemperature(currentTemperature-=value);
    }

    void UpdateDisplay(float temp)
    {
        displayScreen.text = temp.ToString("F1") + "°C";

    }


    private void Start()
    {
        SetTemperature(currentTemperature);
    }
}
