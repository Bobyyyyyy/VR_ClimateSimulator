using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SessionSummaryHUD : MonoBehaviour
{
    [Tooltip("Distanza (metri) del pannello davanti alla camera")]
    public float distanceFromCamera = 1.2f;

    private const float PanelWidth = 700f;
    private const float RowHeight = 46f;
    private const float SectionGap = 20f;

    void Start()
    {
        if (ActionLogger.Instance != null)
            ActionLogger.Instance.OnSessionComplete += BuildUI;
    }

    void OnDestroy()
    {
        if (ActionLogger.Instance != null)
            ActionLogger.Instance.OnSessionComplete -= BuildUI;
    }

    private void BuildUI()
    {
        Camera cam = Camera.main;

        GameObject canvasRoot = new GameObject("SessionSummaryCanvas");
        Canvas canvas = canvasRoot.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvasRoot.AddComponent<CanvasScaler>();
        canvasRoot.AddComponent<GraphicRaycaster>();

        RectTransform canvasRect = canvasRoot.GetComponent<RectTransform>();
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
        panelImage.color = new Color(0f, 0f, 0f, 0.92f);

        float cursorY = -30f;

        cursorY = CreateLabel(panel.transform, cursorY, 44f, "Day completed", 32, FontStyles.Bold);
        cursorY -= SectionGap;

        string username = ActionLogger.Instance != null ? ActionLogger.Instance.Username : "Unknown";
        cursorY = CreateLabel(panel.transform, cursorY, 36f, $"User: {username}", 26, FontStyles.Normal);
        cursorY -= SectionGap;

        float totalSeconds = 0f;
        if (ActionLogger.Instance != null)
        {
            foreach (var entry in ActionLogger.Instance.TaskDurations)
            {
                cursorY = CreateLabel(panel.transform, cursorY, RowHeight,
                    $"{entry.TaskName}: {FormatTime(entry.Seconds)}", 24, FontStyles.Normal);
                totalSeconds += entry.Seconds;
            }
        }

        cursorY -= SectionGap;
        cursorY = CreateLabel(panel.transform, cursorY, 40f, $"Total time: {FormatTime(totalSeconds)}", 26, FontStyles.Bold);

        canvasRect.sizeDelta = new Vector2(PanelWidth, -cursorY + 30f);
    }

    private static string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60f);
        int secs = Mathf.FloorToInt(seconds % 60f);
        return $"{minutes:00}:{secs:00}";
    }

    private float CreateLabel(Transform parent, float cursorY, float height, string text, int fontSize, FontStyles style)
    {
        GameObject go = CreateUIObject("Label", parent);
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0f, cursorY);
        rect.sizeDelta = new Vector2(PanelWidth - 60f, height);

        TextMeshProUGUI txt = go.AddComponent<TextMeshProUGUI>();
        txt.text = text;
        txt.fontSize = fontSize;
        txt.fontStyle = style;
        txt.alignment = TextAlignmentOptions.Center;
        txt.color = Color.white;

        return cursorY - height;
    }

    private static GameObject CreateUIObject(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go;
    }
}
