// GlintPickup.cs
using UnityEngine;

public class GlintPickup : MonoBehaviour
{
    [Tooltip("How many Glints this gives")]
    public int glintValue = 1;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // 1) Use the singleton directly so there’s only one source of truth
        if (GlintCollector.Instance != null)
        {
            GlintCollector.Instance.AddGlints(glintValue);
        }
        else
        {
            Debug.LogError("GlintCollector.Instance is null! Make sure a GlintCollector exists in the scene.");
        }


        // 2) Update the UI counter
        if (UIManager.Instance != null)
            UIManager.Instance.AddGlints(glintValue);

        // 3) Destroy the pickup
        Destroy(gameObject);
    }
}