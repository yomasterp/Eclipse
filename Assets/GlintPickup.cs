// GlintPickup.cs
using UnityEngine;

public class GlintPickup : MonoBehaviour
{
    [Tooltip("How many Glints this gives")]
    public int glintValue = 1;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Add to the player’s GlintCollector
        var collector = other.GetComponent<GlintCollector>();
        if (collector != null)
            collector.AddGlints(glintValue);

        Destroy(gameObject);
    }
}
