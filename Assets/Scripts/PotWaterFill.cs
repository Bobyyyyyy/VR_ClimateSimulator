using UnityEngine;

public class PotWaterFill : MonoBehaviour
{
    [Header("Impostazioni Acqua")]
    [Tooltip("Trascina qui il cilindro dell'acqua dalla Hierarchy")]
    public Transform waterCylinder; 
    
    [Tooltip("La scala Y massima quando la pentola è piena")]
    public float maxWaterScaleY = 1.0f; 
    
    [Tooltip("Quanto velocemente si riempie la pentola?")]
    public float fillSpeed = 0.2f; 

    [Header("Riferimenti Esterni")]
    [Tooltip("Trascina qui il Particle System del rubinetto")]
    public ParticleSystem faucetParticleSystem; 
    
    [Tooltip("Il Tag assegnato al trigger invisibile sotto il rubinetto")]
    public string sinkTriggerTag = "SinkTrigger"; 

    private bool isUnderTap = false;

    void Start()
    {
        if (waterCylinder != null)
        {
            Vector3 startScale = waterCylinder.localScale;
            startScale.y = 0f;
            waterCylinder.localScale = startScale;

            waterCylinder.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (isUnderTap && faucetParticleSystem != null && faucetParticleSystem.isPlaying)
        {
            if (waterCylinder.localScale.y < maxWaterScaleY)
            {
                if (!waterCylinder.gameObject.activeSelf)
                {
                    waterCylinder.gameObject.SetActive(true);
                }

                Vector3 newScale = waterCylinder.localScale;
                newScale.y += fillSpeed * Time.deltaTime;
                
                newScale.y = Mathf.Clamp(newScale.y, 0f, maxWaterScaleY);
                
                waterCylinder.localScale = newScale;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(sinkTriggerTag))
        {
            isUnderTap = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(sinkTriggerTag))
        {
            isUnderTap = false;
        }
    }
}