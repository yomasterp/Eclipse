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

        // 1) Add to the player’s GlintCollector (if you still need it)
        var collector = other.GetComponent<GlintCollector>();
        if (collector != null)
            collector.AddGlints(glintValue);

        // 2) Update the UI counter
        if (UIManager.Instance != null)
            UIManager.Instance.AddGlints(glintValue);

        // 3) Destroy the pickup object
        Destroy(gameObject);
    }
}
