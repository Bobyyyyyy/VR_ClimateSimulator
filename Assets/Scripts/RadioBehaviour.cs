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

    // Tiene traccia dell'ultimo stato per cui la radio ha effettivamente parlato: onTaskChanged
    // scatta ad ogni singolo task, non solo quando cambia lo stato del mondo, quindi senza
    // questo controllo la radio (e il conseguente ducking della musica) ripartirebbe da capo
    // anche restando nello stesso stato.
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

    // Richiamata ogni volta che un task finisce e si passa al successivo.
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

        // Ricontrolliamo sempre lo stato corrente a fine task: se la radio sta ancora
        // trasmettendo la clip di uno stato precedente, va fermata prima di far partire
        // quella giusta per lo stato attuale.
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

    // Abbassa la musica di sottofondo finché la radio sta parlando, poi la ripristina.
    private IEnumerator DuckMusicWhilePlaying()
    {
        if (musicManager != null) musicManager.SetDucked(true);

        yield return null; // lascia partire audioSource.Play() prima di controllare isPlaying
        while (audioSource.isPlaying)
            yield return null;

        if (musicManager != null) musicManager.SetDucked(false);
    }
}
