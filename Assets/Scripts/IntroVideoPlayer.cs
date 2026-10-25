using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;

public class IntroVideoPlayer : MonoBehaviour
{
    public static IntroVideoPlayer Instance;

    public VideoClip introClip;

    [Tooltip("Distanza (metri) dello schermo del video davanti alla camera")]
    public float distanceFromCamera = 2f;

    [Tooltip("Larghezza (metri) dello schermo del video")]
    public float videoWidthMeters = 2f;

    public UnityEvent onFinished;

    public bool IsPlaying { get; private set; }

    private const int IsolatedLayer = 31;

    private Camera cam;
    private int originalCullingMask;
    private CameraClearFlags originalClearFlags;
    private Color originalBackground;
    private GameObject canvasRoot;
    private GameObject playerRoot;
    private RenderTexture renderTexture;
    private VideoPlayer videoPlayer;
    private readonly List<AudioSource> mutedSources = new List<AudioSource>();

    void Awake()
    {
        Instance = this;

        if (introClip == null) return;

        cam = Camera.main;
        if (cam == null) return;

        IsPlaying = true;

        HideWorld();
        MuteOtherAudio();
        BuildVideoCanvas();
        StartVideo();
    }

    void OnDestroy()
    {
        if (IsPlaying) RestoreWorld();
    }

    private void HideWorld()
    {
        originalCullingMask = cam.cullingMask;
        originalClearFlags = cam.clearFlags;
        originalBackground = cam.backgroundColor;

        cam.cullingMask = 1 << IsolatedLayer;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black;
    }

    private void MuteOtherAudio()
    {
        foreach (AudioSource source in FindObjectsByType<AudioSource>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (source.mute) continue;
            source.mute = true;
            mutedSources.Add(source);
        }
    }

    private void BuildVideoCanvas()
    {
        float width = videoWidthMeters * 1000f;
        float height = width * introClip.height / introClip.width;

        canvasRoot = new GameObject("IntroVideoCanvas");
        Canvas canvas = canvasRoot.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;

        RectTransform canvasRect = canvasRoot.GetComponent<RectTransform>();
        canvasRoot.transform.SetParent(cam.transform, false);
        canvasRect.localPosition = new Vector3(0f, 0f, distanceFromCamera);
        canvasRect.localRotation = Quaternion.identity;
        canvasRect.localScale = Vector3.one * 0.001f;
        canvasRect.sizeDelta = new Vector2(width, height);

        GameObject backdrop = CreateUIObject("Backdrop", canvasRoot.transform);
        RectTransform backdropRect = backdrop.GetComponent<RectTransform>();
        backdropRect.anchorMin = Vector2.zero;
        backdropRect.anchorMax = Vector2.one;
        backdropRect.offsetMin = new Vector2(-4000f, -4000f);
        backdropRect.offsetMax = new Vector2(4000f, 4000f);
        backdrop.AddComponent<Image>().color = Color.black;

        renderTexture = new RenderTexture(introClip.width > 0 ? (int)introClip.width : 1920, introClip.height > 0 ? (int)introClip.height : 1080, 0);

        GameObject screen = CreateUIObject("Video", canvasRoot.transform);
        RectTransform screenRect = screen.GetComponent<RectTransform>();
        screenRect.anchorMin = Vector2.zero;
        screenRect.anchorMax = Vector2.one;
        screenRect.offsetMin = Vector2.zero;
        screenRect.offsetMax = Vector2.zero;
        screen.AddComponent<RawImage>().texture = renderTexture;

        SetLayerRecursively(canvasRoot.transform, IsolatedLayer);
    }

    private void StartVideo()
    {
        playerRoot = new GameObject("IntroVideoPlayer");
        playerRoot.transform.SetParent(transform, false);

        AudioSource audioSource = playerRoot.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;

        videoPlayer = playerRoot.AddComponent<VideoPlayer>();
        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = false;
        videoPlayer.source = VideoSource.VideoClip;
        videoPlayer.clip = introClip;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = renderTexture;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.controlledAudioTrackCount = 1;
        videoPlayer.EnableAudioTrack(0, true);
        videoPlayer.SetTargetAudioSource(0, audioSource);

        videoPlayer.loopPointReached += _ => Finish();
        videoPlayer.errorReceived += (_, message) =>
        {
            Debug.LogError($"[IntroVideoPlayer] Errore di riproduzione: {message}");
            Finish();
        };

        videoPlayer.Play();
    }

    private void Finish()
    {
        if (!IsPlaying) return;

        RestoreWorld();
        onFinished?.Invoke();
    }

    private void RestoreWorld()
    {
        IsPlaying = false;

        if (videoPlayer != null) videoPlayer.Stop();
        if (playerRoot != null) Destroy(playerRoot);
        if (canvasRoot != null) Destroy(canvasRoot);

        if (renderTexture != null)
        {
            renderTexture.Release();
            Destroy(renderTexture);
        }

        if (cam != null)
        {
            cam.cullingMask = originalCullingMask;
            cam.clearFlags = originalClearFlags;
            cam.backgroundColor = originalBackground;
        }

        foreach (AudioSource source in mutedSources)
            if (source != null) source.mute = false;
        mutedSources.Clear();
    }

    private static GameObject CreateUIObject(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go;
    }

    private static void SetLayerRecursively(Transform root, int layer)
    {
        root.gameObject.layer = layer;
        foreach (Transform child in root)
            SetLayerRecursively(child, layer);
    }
}
