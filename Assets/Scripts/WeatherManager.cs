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
    
    private bool isBadWeather = false;
    private float targetFogDensity = 0f;

    void Awake() => Instance = this;

    void Start()
    {
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.fogDensity = 0f;
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
    }

    [ContextMenu("Start Neutral Weather")]
    public void StopBadWeather()
    {
        isBadWeather = false;
        targetFogDensity = 0f;
        SetSkybox(neutralSkybox);
        if (rain != null) rain.SetActive(false);
    }


    [ContextMenu("Start Good Weather")]
    public void SetGoodWeather()
    {
        isBadWeather = false;
        targetFogDensity = 0f;
        SetSkybox(goodSkybox);
        if (rain != null) rain.SetActive(false);
    }


    public void SetSkybox(Material skybox)
    {
    RenderSettings.skybox = skybox;
    DynamicGI.UpdateEnvironment();
    }
}