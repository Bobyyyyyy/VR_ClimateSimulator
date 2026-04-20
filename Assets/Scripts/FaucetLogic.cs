using UnityEngine;

public class FaucetLogic : MonoBehaviour
{
    [Header("Riferimenti")]
    public ParticleSystem waterSystem;

    [Header("Impostazioni Getto")]
    public float maxEmission = 250f;

    // Metodo che chiameremo dalla manopola o dall'evento VR
    public void UpdateFlow(float value) 
    {
        if (waterSystem == null) return;

        var emission = waterSystem.emission;
        
        // Mappiamo il valore (0-1) sulla quantità di particelle
        emission.rateOverTime = value * maxEmission;

        // Gestione Play/Stop per ottimizzare
        if (value > 0.01f)
        {
            if (!waterSystem.isPlaying) waterSystem.Play();
        }
        else
        {
            if (waterSystem.isPlaying) waterSystem.Stop();
        }
    }
}