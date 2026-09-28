using UnityEngine;
using UnityEngine.UI;
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

    [Header("Scelta task: bottoni Spegni/Lascia acceso, spariscono dopo la scelta")]
    public GameObject turnOffButton;
    public GameObject leaveOnButton;

    [Header("Se si spegne (scelta ecologica): +/- si disattivano")]
    public Button increaseButton;
    public Button decreaseButton;

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
    public void TurnOff()
    {
        ThermostatTask.Instance?.TurnOff();
        if (!IsChoiceMade()) return;

        HideChoiceButtons();

        // Spento (scelta ecologica): niente più display, suono o regolazione temperatura.
        if (displayScreen != null) displayScreen.gameObject.SetActive(false);
        PlayIdleSound(false);
        if (increaseButton != null) increaseButton.interactable = false;
        if (decreaseButton != null) decreaseButton.interactable = false;
    }

    public void LeaveOn()
    {
        ThermostatTask.Instance?.LeaveOn();
        if (IsChoiceMade()) HideChoiceButtons();
    }

    private bool IsChoiceMade() =>
        ThermostatTask.Instance != null && ThermostatTask.Instance.task != null && ThermostatTask.Instance.task.isCompleted;

    private void HideChoiceButtons()
    {
        if (turnOffButton != null) turnOffButton.SetActive(false);
        if (leaveOnButton != null) leaveOnButton.SetActive(false);
    }

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
