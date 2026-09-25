using UnityEngine;
using TMPro;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Collections;

public class ThermostatController : MonoBehaviour
{
    
    public TextMeshProUGUI displayScreen;
    float targetTemperature = 19f;
    public float currentTemperature;
    
    float minTemp = 0;
    float maxTemp = 60;

    public AudioSource idleSound;

    private void SetTemperature(float temperature)
    {
        currentTemperature = temperature;
        UpdateDisplay(temperature);

        if (currentTemperature <= targetTemperature)
        {
            PlayIdleSound(false);
        }
        else
        {
            PlayIdleSound(true);
            
        }
    }

    public float GetTemperature()
    {
        return currentTemperature; 
    }

    public void IncreaseTemperature(float value)
    {
        if(currentTemperature + value < maxTemp )
        {
            SetTemperature(currentTemperature+value);
            LogAdjustment(value);
        }

    }

    public void DecreaseTemperature(float value)
    {
        if (currentTemperature - value > minTemp )
        {
            SetTemperature(currentTemperature-value);
            LogAdjustment(-value);
        }

    }

    // Collegati ai due bottoni HUD del termostato (task giornata): la scelta è binaria,
    // esattamente come le altre task del giorno (luce, colazione, cena, lavoro).
    public void TurnOff() => ThermostatTask.Instance?.TurnOff();

    public void LeaveOn() => ThermostatTask.Instance?.LeaveOn();

    private void LogAdjustment(float delta)
    {
        string taskName = ThermostatTask.Instance != null && ThermostatTask.Instance.task != null
            ? ThermostatTask.Instance.task.taskName : "Termostato";

        ActionLogger.Instance?.LogAction(taskName, "Regolazione temperatura",
            $"Variazione={delta:F1} | TemperaturaAttuale={currentTemperature:F1}");
    }

    void UpdateDisplay(float temp)
    {
        displayScreen.text = temp.ToString("F1") + "C";
    }

    public void SetTargetTemperature(float temp)
    {
        targetTemperature = temp;
    }

    void PlayIdleSound(bool play)
    {
        if (play)
        {
            if(!idleSound.isPlaying)
            {
                idleSound.Play();
                idleSound.volume = 0;
                idleSound.loop = true;
                this.StartFade(idleSound,idleSound.volume, 1f, 1.5f);
            }
        }
        else
        {
            if(idleSound.isPlaying)
            {
                this.StartFade(idleSound,idleSound.volume, 0, 1.5f);
            }
        }
    
    }

    private void Start()
    {
        SetTemperature(currentTemperature);

        PlayIdleSound(true);
    }




}
