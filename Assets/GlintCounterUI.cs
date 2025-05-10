// GlintCounterUI.cs
using UnityEngine;
using TMPro;

public class GlintCounterUI : MonoBehaviour
{
    public TMP_Text glintText;
    private GlintCollector collector;

    void Start()
    {
        collector = GlintCollector.Instance;
        if (collector == null)
        {
            Debug.LogError("No GlintCollector in scene!");
            enabled = false;
            return;
        }
        collector.onGlintCountChanged += UpdateUI;
        UpdateUI(collector.glintCount);
    }

    void UpdateUI(int count)
    {
        glintText.text = $"Glints: {count}";
    }

    void OnDestroy()
    {
        if (collector != null)
            collector.onGlintCountChanged -= UpdateUI;
    }
}
