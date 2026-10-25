using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class WorldMusicManager : MonoBehaviour
{
    [Header("Musiche di sottofondo per stato del mondo")]
    public AudioClip goodMusic;
    public AudioClip neutralMusic;
    public AudioClip badMusic;

    [Range(0f, 1f)]
    public float volume = 0.15f;

    [Header("Ducking (quando parla la radio)")]
    [Range(0f, 1f)]
    public float duckedVolumeFactor = 0.2f;

    private AudioSource source;
    private AudioClip currentClip;
    private bool isDucked = false;

    void Awake()
    {
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = true;
        source.spatialBlend = 0f;
        source.volume = volume;
    }

    void Start()
    {
        UpdateForCurrentState();

        if (TaskManager.Instance != null)
            TaskManager.Instance.onTaskChanged.AddListener(OnTaskChanged);
    }

    void OnDestroy()
    {
        if (TaskManager.Instance != null)
            TaskManager.Instance.onTaskChanged.RemoveListener(OnTaskChanged);
    }

    private void OnTaskChanged(TaskData newCurrentTask) => UpdateForCurrentState();

    private void UpdateForCurrentState()
    {
        if (TaskManager.Instance == null) return;

        switch (TaskManager.Instance.current_state)
        {
            case World_State.Good:
            case World_State.Neutral_Good:
                PlayGood();
                break;
            case World_State.Neutral_Bad:
            case World_State.Bad:
                PlayBad();
                break;
            default:
                PlayNeutral();
                break;
        }
    }

    public void PlayGood() => PlayClip(goodMusic);
    public void PlayNeutral() => PlayClip(neutralMusic);
    public void PlayBad() => PlayClip(badMusic);

    public void SetDucked(bool ducked)
    {
        isDucked = ducked;
        source.volume = isDucked ? volume * duckedVolumeFactor : volume;
    }

    private void PlayClip(AudioClip clip)
    {
        if (clip == null || clip == currentClip) return;

        if (source.isPlaying) source.Stop();

        currentClip = clip;
        source.clip = clip;
        source.volume = isDucked ? volume * duckedVolumeFactor : volume;
        source.Play();
    }
}
