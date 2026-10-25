using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeItem : MonoBehaviour
{
    [SerializeField] private Toggle checkbox;
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private Color doneColor = new Color(0.5f, 0.5f, 0.5f);

    private string originalText;
    private Color originalColor;

    void Awake()
    {
        if (label != null)
        {
            originalText = label.text;
            originalColor = label.color;
        }
        if (checkbox != null)
        {
            checkbox.interactable = false;
            checkbox.isOn = false;
        }
    }

    public void SetDone(bool done)
    {
        if (checkbox != null) checkbox.isOn = done;
        if (label != null)
        {
            label.color = done ? doneColor : originalColor;
            label.text = done ? $"<s>{originalText}</s>" : originalText;
        }
    }
}