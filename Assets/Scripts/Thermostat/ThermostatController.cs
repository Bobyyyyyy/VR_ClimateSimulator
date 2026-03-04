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
    public Vector2 idealRange;
    public float currentTemperature;

    public AudioSource idleSound;


    private Coroutine fadeCoroutine;

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
        SetTemperature(currentTemperature+value);
    }

    public void DecreaseTemperature(float value)
    {
        SetTemperature(currentTemperature-value);
    }

    void UpdateDisplay(float temp)
    {
        displayScreen.text = temp.ToString("F1") + "�C";
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
