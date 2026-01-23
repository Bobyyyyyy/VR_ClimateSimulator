using UnityEngine;

public class WaterLogic : MonoBehaviour
{
    private ParticleSystem _ps;

    void Start()
    {
        _ps = GetComponent<ParticleSystem>();
        StopWater(); // Parte chiuso di default
    }

    public void SetWaterFlow(float intensity) 
    {
        var emission = _ps.emission;
        emission.rateOverTime = intensity * 200f; // 200 è il flusso massimo
        
        if (intensity > 0.01f && !_ps.isPlaying) _ps.Play();
        else if (intensity <= 0.01f) _ps.Stop();
    }

    public void StopWater() => _ps.Stop();
}