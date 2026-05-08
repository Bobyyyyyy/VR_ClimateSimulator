using UnityEngine;

public class RadioBehaviour : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip GoodAudio;
    public AudioClip NeutralAudio;
    public AudioClip BadAudio;

    public void Stop() => audioSource.Stop();

    public void Play()
    {
        if (audioSource.isPlaying) return;
        switch(TaskManager.Instance.current_state)
        {
            case World_State.Good:
                audioSource.clip = GoodAudio;
                break;
            case World_State.Neutral:
                audioSource.clip = NeutralAudio;
                break;
            case World_State.Bad:
                audioSource.clip = BadAudio;
                break;

        }
        audioSource.Play();
    }
}