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

    // Variabile per sapere se siamo sotto l'acqua
    private bool isUnderTap = false;

    void Start()
    {
        // All'inizio, assicuriamoci che la pentola sia vuota (Scala Y = 0)
        if (waterCylinder != null)
        {
            Vector3 startScale = waterCylinder.localScale;
            startScale.y = 0f;
            waterCylinder.localScale = startScale;

            // DISATTIVIAMO il cilindro all'avvio affinché non si veda il "disco"
            waterCylinder.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // LOGICA PRINCIPALE:
        // Se siamo sotto il rubinetto E il Particle System esiste ed è attivo (isPlaying)
        if (isUnderTap && faucetParticleSystem != null && faucetParticleSystem.isPlaying)
        {
            // Controlliamo che l'acqua non abbia già superato il livello massimo
            if (waterCylinder.localScale.y < maxWaterScaleY)
            {
                // SE il cilindro era invisibile, RIATTIVIAMOLO ora che l'acqua entra
                if (!waterCylinder.gameObject.activeSelf)
                {
                    waterCylinder.gameObject.SetActive(true);
                }

                // Aumentiamo la scala sull'asse Y nel tempo
                Vector3 newScale = waterCylinder.localScale;
                newScale.y += fillSpeed * Time.deltaTime;
                
                // Usiamo Mathf.Clamp per assicurarci che non superi MAI il valore massimo
                newScale.y = Mathf.Clamp(newScale.y, 0f, maxWaterScaleY);
                
                // Applichiamo la nuova scala al cilindro
                waterCylinder.localScale = newScale;
            }
        }
    }

    // Rileva quando la pentola ENTRA nel trigger del lavandino
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(sinkTriggerTag))
        {
            isUnderTap = true;
        }
    }

    // Rileva quando la pentola ESCE dal trigger del lavandino
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(sinkTriggerTag))
        {
            isUnderTap = false;
        }
    }
}