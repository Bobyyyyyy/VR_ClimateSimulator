using UnityEngine;

public class RadioBehaviour : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip GoodAudio;
    public AudioClip NeutralAudio;
    public AudioClip BadAudio;


    [ContextMenu("Test good audio")]
    public void PlayGoodAudio() => Play(GoodAudio);
    [ContextMenu("Test neutral audio")]
    public void PlayNeutralAudio() => Play(NeutralAudio);
    [ContextMenu("Test bad audio")]
    public void PlayBadAudio() => Play(BadAudio);
    public void Stop() => audioSource.Stop();

    private void Play(AudioClip clip)
    {

        if (audioSource.isPlaying) return;
        audioSource.clip = clip;
        audioSource.Play();
    }
}