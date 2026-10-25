using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class UsernamePrompt : MonoBehaviour
{
    [Tooltip("Distanza (metri) del pannello davanti alla camera")]
    public float distanceFromCamera = 1.2f;

    private static readonly string[] Row1 = { "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P" };
    private static readonly string[] Row2 = { "A", "S", "D", "F", "G", "H", "J", "K", "L" };
    private static readonly string[] Row3 = { "Z", "X", "C", "V", "B", "N", "M" };

    private const float PanelWidth = 720f;
    private const float KeyHeight = 56f;
    private const float KeyGap = 6f;
    private const float RowGap = 10f;
    private const float SectionGap = 20f;

    private GameObject canvasRoot;
    private RectTransform canvasRect;
    private TMP_InputField inputField;

    public static bool IsActive { get; private set; }

    private LocomotionProvider[] providers;
    private bool locomotionLocked;

    void Awake()
    {
        IsActive = true;
        providers = FindObjectsByType<LocomotionProvider>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        DisableLocomotion();
    }

    void OnDestroy()
    {
        IsActive = false;
    }

    // Il blocco vale per tutta la durata del video e dell'inserimento nome: se qualcosa
    // riattiva un provider nel frattempo, lo rispegniamo subito.
    void LateUpdate()
    {
        if (!locomotionLocked) return;

        foreach (LocomotionProvider provider in providers)
            if (provider != null && provider.enabled) provider.enabled = false;
    }

    void Start()
    {
        if (IntroVideoPlayer.Instance != null && IntroVideoPlayer.Instance.IsPlaying)
            IntroVideoPlayer.Instance.onFinished.AddListener(BuildUI);
        else
            BuildUI();
    }

    private void BuildUI()
    {
        Camera cam = Camera.main;

        canvasRoot = new GameObject("UsernamePromptCanvas");
        Canvas canvas = canvasRoot.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvasRoot.AddComponent<CanvasScaler>();
        TrackedDeviceGraphicRaycaster raycasterInit = canvasRoot.AddComponent<TrackedDeviceGraphicRaycaster>();
        raycasterInit.ignoreReversedGraphics = false;

        canvasRect = canvasRoot.GetComponent<RectTransform>();

        if (cam != null)
        {
            canvasRoot.transform.SetParent(cam.transform, false);
            canvasRect.localPosition = new Vector3(0f, 0f, distanceFromCamera);
            canvasRect.localRotation = Quaternion.identity;
        }
        canvasRect.localScale = Vector3.one * 0.001f;

        GameObject panel = CreateUIObject("Panel", canvasRoot.transform);
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.85f);

        float cursorY = -20f;

        cursorY = CreateTitle(panel.transform, cursorY);
        cursorY = CreateInputField(panel.transform, cursorY);
        cursorY = CreateKeyboard(panel.transform, cursorY);
        cursorY = CreateConfirmButton(panel.transform, cursorY);

        canvasRect.sizeDelta = new Vector2(PanelWidth, -cursorY + 20f);

        inputField.Select();
        inputField.ActivateInputField();

        TrackedDeviceGraphicRaycaster raycaster = canvasRoot.GetComponent<TrackedDeviceGraphicRaycaster>();
        Debug.Log($"[UsernamePrompt] Pannello costruito. canvas.renderMode={canvas.renderMode}, "
            + $"worldPos={canvasRect.position}, TrackedDeviceGraphicRaycaster presente={raycaster != null}, "
            + $"EventSystem.current={(UnityEngine.EventSystems.EventSystem.current != null ? UnityEngine.EventSystems.EventSystem.current.name : "NESSUNO")}");
    }

    private float CreateTitle(Transform parent, float cursorY)
    {
        const float height = 50f;

        GameObject title = CreateUIObject("Title", parent);
        RectTransform titleRect = title.GetComponent<RectTransform>();
        SetTopAnchored(titleRect, cursorY, new Vector2(PanelWidth - 40f, height));

        TextMeshProUGUI titleText = title.AddComponent<TextMeshProUGUI>();
        titleText.text = "Enter your username";
        titleText.fontSize = 28;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.white;

        return cursorY - height - SectionGap;
    }

    private float CreateInputField(Transform parent, float cursorY)
    {
        const float height = 64f;

        GameObject inputGO = CreateUIObject("UsernameInput", parent);
        RectTransform inputRect = inputGO.GetComponent<RectTransform>();
        SetTopAnchored(inputRect, cursorY, new Vector2(440f, height));

        Image inputImage = inputGO.AddComponent<Image>();
        inputImage.color = Color.white;
        inputField = inputGO.AddComponent<TMP_InputField>();
        inputField.characterLimit = 24;

        GameObject textArea = CreateUIObject("Text Area", inputGO.transform);
        RectTransform textAreaRect = textArea.GetComponent<RectTransform>();
        textAreaRect.anchorMin = Vector2.zero;
        textAreaRect.anchorMax = Vector2.one;
        textAreaRect.offsetMin = new Vector2(12f, 6f);
        textAreaRect.offsetMax = new Vector2(-12f, -6f);
        textArea.AddComponent<RectMask2D>();

        GameObject placeholder = CreateUIObject("Placeholder", textArea.transform);
        RectTransform placeholderRect = placeholder.GetComponent<RectTransform>();
        placeholderRect.anchorMin = Vector2.zero;
        placeholderRect.anchorMax = Vector2.one;
        placeholderRect.offsetMin = Vector2.zero;
        placeholderRect.offsetMax = Vector2.zero;
        TextMeshProUGUI placeholderText = placeholder.AddComponent<TextMeshProUGUI>();
        placeholderText.text = "Username...";
        placeholderText.fontStyle = FontStyles.Italic;
        placeholderText.color = new Color(0f, 0f, 0f, 0.5f);
        placeholderText.alignment = TextAlignmentOptions.MidlineLeft;
        placeholderText.fontSize = 24;

        GameObject inputTextGO = CreateUIObject("Text", textArea.transform);
        RectTransform inputTextRect = inputTextGO.GetComponent<RectTransform>();
        inputTextRect.anchorMin = Vector2.zero;
        inputTextRect.anchorMax = Vector2.one;
        inputTextRect.offsetMin = Vector2.zero;
        inputTextRect.offsetMax = Vector2.zero;
        TextMeshProUGUI inputText = inputTextGO.AddComponent<TextMeshProUGUI>();
        inputText.color = Color.black;
        inputText.alignment = TextAlignmentOptions.MidlineLeft;
        inputText.fontSize = 24;

        inputField.textViewport = textAreaRect;
        inputField.textComponent = inputText;
        inputField.placeholder = placeholderText;
        inputField.fontAsset = inputText.font;

        return cursorY - height - SectionGap;
    }

    private float CreateKeyboard(Transform parent, float cursorY)
    {
        cursorY = CreateLetterRow(parent, Row1, cursorY);
        cursorY = CreateLetterRow(parent, Row2, cursorY);

        float keyWidth = RowKeyWidth(Row1.Length);
        float row3Width = Row3.Length * keyWidth + (Row3.Length - 1) * KeyGap + KeyGap + keyWidth * 1.6f;
        float startX = -row3Width / 2f + keyWidth / 2f;

        for (int i = 0; i < Row3.Length; i++)
        {
            float x = startX + i * (keyWidth + KeyGap);
            string key = Row3[i];
            CreateKey(parent, key, new Vector2(x, cursorY), new Vector2(keyWidth, KeyHeight), () => AppendChar(key));
        }

        float backspaceWidth = keyWidth * 1.6f;
        float backspaceX = startX + Row3.Length * (keyWidth + KeyGap) - KeyGap + KeyGap + backspaceWidth / 2f - keyWidth / 2f;
        CreateKey(parent, "<--", new Vector2(backspaceX, cursorY), new Vector2(backspaceWidth, KeyHeight), RemoveLastChar);

        cursorY -= KeyHeight + RowGap;

        float spaceWidth = RowKeyWidth(Row1.Length) * Row1.Length + (Row1.Length - 1) * KeyGap;
        CreateKey(parent, " ", new Vector2(0f, cursorY), new Vector2(spaceWidth, KeyHeight), () => AppendChar(" "), "Space");
        cursorY -= KeyHeight + SectionGap;

        return cursorY;
    }

    private float CreateLetterRow(Transform parent, string[] keys, float cursorY)
    {
        float keyWidth = RowKeyWidth(Row1.Length);
        float rowWidth = keys.Length * keyWidth + (keys.Length - 1) * KeyGap;
        float startX = -rowWidth / 2f + keyWidth / 2f;

        for (int i = 0; i < keys.Length; i++)
        {
            float x = startX + i * (keyWidth + KeyGap);
            string key = keys[i];
            CreateKey(parent, key, new Vector2(x, cursorY), new Vector2(keyWidth, KeyHeight), () => AppendChar(key));
        }

        return cursorY - KeyHeight - RowGap;
    }

    private static float RowKeyWidth(int widestRowKeyCount)
    {
        const float innerMargin = 40f;
        float available = PanelWidth - innerMargin;
        return (available - (widestRowKeyCount - 1) * KeyGap) / widestRowKeyCount;
    }

    private void CreateKey(Transform parent, string key, Vector2 anchoredPos, Vector2 size, UnityEngine.Events.UnityAction onClick, string label = null)
    {
        GameObject keyGO = CreateUIObject($"Key_{(key == " " ? "Space" : key)}", parent);
        RectTransform rect = keyGO.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta = size;

        Image bg = keyGO.AddComponent<Image>();
        bg.color = new Color(1f, 1f, 1f, 0.9f);
        Button btn = keyGO.AddComponent<Button>();
        btn.onClick.AddListener(onClick);

        GameObject textGO = CreateUIObject("Text", keyGO.transform);
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        TextMeshProUGUI txt = textGO.AddComponent<TextMeshProUGUI>();
        txt.text = label ?? key;
        txt.fontSize = 22;
        txt.alignment = TextAlignmentOptions.Center;
        txt.color = Color.black;
    }

    private float CreateConfirmButton(Transform parent, float cursorY)
    {
        const float height = 70f;

        GameObject buttonGO = CreateUIObject("ConfirmButton", parent);
        RectTransform buttonRect = buttonGO.GetComponent<RectTransform>();
        SetTopAnchored(buttonRect, cursorY, new Vector2(220f, height));

        Image buttonImage = buttonGO.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.6f, 0.2f, 1f);
        Button confirmButton = buttonGO.AddComponent<Button>();

        GameObject buttonTextGO = CreateUIObject("Text", buttonGO.transform);
        RectTransform buttonTextRect = buttonTextGO.GetComponent<RectTransform>();
        buttonTextRect.anchorMin = Vector2.zero;
        buttonTextRect.anchorMax = Vector2.one;
        buttonTextRect.offsetMin = Vector2.zero;
        buttonTextRect.offsetMax = Vector2.zero;
        TextMeshProUGUI buttonText = buttonTextGO.AddComponent<TextMeshProUGUI>();
        buttonText.text = "Confirm";
        buttonText.fontSize = 26;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.color = Color.white;

        confirmButton.onClick.AddListener(OnConfirm);
        inputField.onSubmit.AddListener(_ => OnConfirm());

        return cursorY - height;
    }

    private static void SetTopAnchored(RectTransform rect, float y, Vector2 size)
    {
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0f, y);
        rect.sizeDelta = size;
    }

    private static GameObject CreateUIObject(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go;
    }

    private void AppendChar(string c)
    {
        Debug.Log($"[UsernamePrompt] Tasto '{c}' cliccato.");

        if (inputField.characterLimit > 0 && inputField.text.Length >= inputField.characterLimit)
        {
            Debug.Log("[UsernamePrompt] Limite caratteri raggiunto, tasto ignorato.");
            return;
        }

        inputField.text += c.ToLowerInvariant();
        inputField.caretPosition = inputField.text.Length;
        inputField.ActivateInputField();
        Debug.Log($"[UsernamePrompt] Testo attuale: '{inputField.text}'");
    }

    private void RemoveLastChar()
    {
        Debug.Log("[UsernamePrompt] Backspace cliccato.");

        if (inputField.text.Length > 0)
            inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);

        inputField.caretPosition = inputField.text.Length;
        inputField.ActivateInputField();
        Debug.Log($"[UsernamePrompt] Testo attuale: '{inputField.text}'");
    }

    private void OnConfirm()
    {
        Debug.Log($"[UsernamePrompt] Conferma cliccata. Testo nel campo: '{inputField.text}'");

        string entered = inputField.text.Trim();
        string username = string.IsNullOrEmpty(entered) ? "Unknown" : entered;

        ActionLogger.Instance?.BeginSession(username);
        TimerManager.Instance?.BeginSession();

        DisableRayInteractors();
        EnableLocomotion();

        Destroy(canvasRoot);
        Destroy(this);
    }

    private void DisableLocomotion()
    {
        locomotionLocked = true;
        foreach (LocomotionProvider provider in providers)
            if (provider != null) provider.enabled = false;
    }

    private void EnableLocomotion()
    {
        locomotionLocked = false;
        foreach (LocomotionProvider provider in providers)
            if (provider != null) provider.enabled = true;
    }

    private void DisableRayInteractors()
    {
        foreach (XRRayInteractor ray in FindObjectsByType<XRRayInteractor>(FindObjectsSortMode.None))
            ray.gameObject.SetActive(false);
    }
}
