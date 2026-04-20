using UnityEngine;

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
    public float intensitaLuceMassima = 2f; // Quanta luce c'è quando è aperta
    public float intensitaLuceMinima = 0f;  // Quanta luce c'è quando è chiusa

    private bool staSalendo = false;
    private bool staScendendo = false;

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

            // --- GESTIONE DELLA LUCE ---
            if (luceDiRimbalzo != null)
            {
                // Calcoliamo la percentuale di apertura (da 0.0 a 1.0)
                float percentualeApertura = Mathf.InverseLerp(yBassa, yAlta, nuovaY);
                
                // Applichiamo l'intensità in base alla percentuale
                luceDiRimbalzo.intensity = Mathf.Lerp(intensitaLuceMinima, intensitaLuceMassima, percentualeApertura);
            }
        }
    }

    public void IniziaSalita()
    {
        staSalendo = true;
        staScendendo = false;
    }

    public void IniziaDiscesa()
    {
        staScendendo = true;
        staSalendo = false;
    }

    public void FermaMovimento()
    {
        staSalendo = false;
        staScendendo = false;
    }
}