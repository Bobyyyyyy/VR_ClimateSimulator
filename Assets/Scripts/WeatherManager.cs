using System.Diagnostics;
using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    public static WeatherManager Instance;

    public GameObject rain;
    public UnityEngine.Color fogColor = new UnityEngine.Color(0.5f, 0.5f, 0.5f);
    public float fogDensity = 0.03f;
    public float transitionSpeed = 0.5f;

    public Material goodSkybox;
    public Material neutralSkybox;
    public Material badSkybox;

    [Header("Prop esterni: solo quello dello stato corrente resta attivo")]
    public GameObject goodWeatherProps;
    public GameObject neutralWeatherProps;
    public GameObject badWeatherProps;

    private bool isBadWeather = false;
    private float targetFogDensity = 0f;

    void Awake() => Instance = this;

    void Start()
    {
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.fogDensity = 0f;

        UpdateForCurrentState();

        if (TaskManager.Instance != null)
            TaskManager.Instance.onTaskChanged.AddListener(OnTaskChanged);
    }

    void OnDestroy()
    {
        if (TaskManager.Instance != null)
            TaskManager.Instance.onTaskChanged.RemoveListener(OnTaskChanged);
    }

    // Richiamata ogni volta che un task finisce e si passa al successivo: ricontrolliamo
    // lo stato attuale del mondo, così i prop restano coerenti anche se non è cambiata
    // soglia (onWorldGood/onWorldNeutral/onWorldBad non sarebbero scattati).
    private void OnTaskChanged(TaskData newCurrentTask) => UpdateForCurrentState();

    private void UpdateForCurrentState()
    {
        if (TaskManager.Instance == null) return;

        switch (TaskManager.Instance.current_state)
        {
            case World_State.Good:
            case World_State.Neutral_Good:
                SetGoodWeather();
                break;
            case World_State.Neutral_Bad:
            case World_State.Bad:
                StartBadWeather();
                break;
            default:
                StopBadWeather();
                break;
        }
    }

    private void SetActiveProps(GameObject active)
    {
        if (goodWeatherProps != null) goodWeatherProps.SetActive(active == goodWeatherProps);
        if (neutralWeatherProps != null) neutralWeatherProps.SetActive(active == neutralWeatherProps);
        if (badWeatherProps != null) badWeatherProps.SetActive(active == badWeatherProps);
    }

    void Update()
    {
        RenderSettings.fogDensity = Mathf.MoveTowards(
            RenderSettings.fogDensity,
            targetFogDensity,
            transitionSpeed * Time.deltaTime
        );
    }

    [ContextMenu("Start Bad Weather")]
    public void StartBadWeather()
    {
        isBadWeather = true;
        targetFogDensity = fogDensity;
        RenderSettings.fogColor = fogColor;
        SetSkybox(badSkybox);
        if (rain != null) rain.SetActive(true);
        SetActiveProps(badWeatherProps);
    }

    [ContextMenu("Start Neutral Weather")]
    public void StopBadWeather()
    {
        isBadWeather = false;
        targetFogDensity = 0f;
        SetSkybox(neutralSkybox);
        if (rain != null) rain.SetActive(false);
        SetActiveProps(neutralWeatherProps);
    }


    [ContextMenu("Start Good Weather")]
    public void SetGoodWeather()
    {
        isBadWeather = false;
        targetFogDensity = 0f;
        SetSkybox(goodSkybox);
        if (rain != null) rain.SetActive(false);
        SetActiveProps(goodWeatherProps);
    }


    public void SetSkybox(Material skybox)
    {
    RenderSettings.skybox = skybox;
    DynamicGI.UpdateEnvironment();
    }
}