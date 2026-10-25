using UnityEngine;
using UnityEngine.Events;

public class MovimentoLineare : MonoBehaviour
{
    [Header("Riferimenti")]
    public Transform oggettoDaMuovere;

    [Tooltip("Trascina qui la Point Light che hai creato dentro la stanza")]
    public Light luceDiRimbalzo;

    [Header("Impostazioni Posizione Y")]
    public float yAlta = 5f;
    public float yBassa = 0f;

    [Header("Impostazioni Movimento")]
    public float velocita = 2f;

    [Header("Impostazioni Luce")]
    public float intensitaLuceMassima = 2f;
    public float intensitaLuceMinima = 0f;

    [Header("Task giornata (opzionale)")]
    [Tooltip("Sparato una sola volta quando la tapparella raggiunge l'apertura massima")]
    public UnityEvent onFullyOpen;

    private bool staSalendo = false;
    private bool staScendendo = false;
    private bool fullyOpenFired = false;
    private bool locked = false;

    public void Lock() => locked = true;

    void Update()
    {
        if (oggettoDaMuovere != null)
        {
            Vector3 posizioneAttuale = oggettoDaMuovere.localPosition;
            float nuovaY = posizioneAttuale.y;

            if (staSalendo)
            {
                nuovaY += velocita * Time.deltaTime;
            }
            else if (staScendendo)
            {
                nuovaY -= velocita * Time.deltaTime;
            }

            nuovaY = Mathf.Clamp(nuovaY, yBassa, yAlta);
            oggettoDaMuovere.localPosition = new Vector3(posizioneAttuale.x, nuovaY, posizioneAttuale.z);

            float percentualeApertura = Mathf.InverseLerp(yBassa, yAlta, nuovaY);

            if (luceDiRimbalzo != null)
            {
                luceDiRimbalzo.intensity = Mathf.Lerp(intensitaLuceMinima, intensitaLuceMassima, percentualeApertura);
            }

            if (!fullyOpenFired && percentualeApertura >= 0.98f)
            {
                fullyOpenFired = true;
                onFullyOpen?.Invoke();
            }
        }
    }

    public void IniziaSalita()
    {
        if (locked) return;
        staSalendo = true;
        staScendendo = false;
    }

    public void IniziaDiscesa()
    {
        if (locked) return;
        staScendendo = true;
        staSalendo = false;
    }

    public void FermaMovimento()
    {
        staSalendo = false;
        staScendendo = false;
    }
}