using System.Collections;
using UnityEngine;

public class RadioBehaviour : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip GoodAudio;
    public AudioClip NeutralGoodAudio;
    public AudioClip NeutralAudio;

    public AudioClip NeutralBadAudio;
    public AudioClip BadAudio;

    [Header("Musica di sottofondo da abbassare mentre parla la radio")]
    public WorldMusicManager musicManager;

    private World_State? lastPlayedState = null;

    void Start()
    {
        if (TaskManager.Instance != null)
            TaskManager.Instance.onTaskChanged.AddListener(OnTaskChanged);
    }

    void OnDestroy()
    {
        if (TaskManager.Instance != null)
            TaskManager.Instance.onTaskChanged.RemoveListener(OnTaskChanged);
    }

    private void OnTaskChanged(TaskData newCurrentTask)
    {
        if (TaskManager.Instance == null) return;
        if (lastPlayedState.HasValue && lastPlayedState.Value == TaskManager.Instance.current_state) return;

        Play();
    }

    public void Stop() => audioSource.Stop();

    public void Play()
    {
        lastPlayedState = TaskManager.Instance.current_state;

        if (audioSource.isPlaying) audioSource.Stop();

        switch (TaskManager.Instance.current_state)
        {
            case World_State.Good:
                audioSource.clip = GoodAudio;
                break;
            case World_State.Neutral_Good:
                audioSource.clip = NeutralGoodAudio;
                break;
            case World_State.Neutral:
                audioSource.clip = NeutralAudio;
                break;
            case World_State.Neutral_Bad:
                audioSource.clip = NeutralBadAudio;
                break;
            case World_State.Bad:
                audioSource.clip = BadAudio;
                break;
        }

        if (audioSource.clip == null) return;

        audioSource.Play();

        StopAllCoroutines();
        StartCoroutine(DuckMusicWhilePlaying());
    }

    private IEnumerator DuckMusicWhilePlaying()
    {
        if (musicManager != null) musicManager.SetDucked(true);

        yield return null;
        while (audioSource.isPlaying)
            yield return null;

        if (musicManager != null) musicManager.SetDucked(false);
    }
}
