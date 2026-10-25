using UnityEngine;

public class FaucetLogic : MonoBehaviour
{
    [Header("Riferimenti")]
    public ParticleSystem waterSystem;

    [Header("Impostazioni Getto")]
    public float maxEmission = 250f;

    public void UpdateFlow(float value) 
    {
        if (waterSystem == null) return;

        var emission = waterSystem.emission;
        
        emission.rateOverTime = value * maxEmission;

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