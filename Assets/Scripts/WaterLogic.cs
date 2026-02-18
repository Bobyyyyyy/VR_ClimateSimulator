using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RubinettoVR : MonoBehaviour
{
    [Header("Trascina qui l'oggetto Acqua")]
    public ParticleSystem acquaEffect; 

    private HingeJoint mioHinge;

    void Start()
    {
        mioHinge = GetComponent<HingeJoint>();
        if(acquaEffect != null) acquaEffect.Stop();
    }

    void Update()
    {
        if (acquaEffect == null) return;

        // Leggiamo l'angolo. NON usiamo Abs, così se va in negativo (errore fisico) l'acqua non parte.
        float angolo = mioHinge.angle;

        // LOGICA MIGLIORATA
        // Accendiamo l'acqua solo se superiamo i 10 gradi positivi
        if (angolo > 10) 
        {
            if (!acquaEffect.isPlaying) acquaEffect.Play();
        }
        // Spegniamo l'acqua se scendiamo sotto gli 8 gradi.
        // Lasciare questo "gap" tra 10 e 8 evita che l'acqua "tremi" se tieni la mano ferma al limite.
        else if (angolo < 8) 
        {
            if (acquaEffect.isPlaying) acquaEffect.Stop();
        }
    }
}