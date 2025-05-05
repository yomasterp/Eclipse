using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Flash Settings")]
    public float flashDuration = 60f;   // how long the flash lasts
    public float startIntensity = 10f;   // initial brightness
    public float maxRange = 50f;  // how far the light reaches
    public Color flashColor = Color.white;
    public GameObject deathParticles;

    // ← add this
    bool hasDied = false;

    public void TakeDamage(float damage)
    {
        // prevent multiple deaths/flashes
        if (hasDied) return;
        hasDied = true;

        // optional: disable collider to stop further triggers
        var col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // 1) spawn death particles
        if (deathParticles != null)
            Instantiate(deathParticles, transform.position, Quaternion.identity);

        // 2) run the single flash coroutine
        StartCoroutine(ExplodeFlashAndDie());
    }

    IEnumerator ExplodeFlashAndDie()
    {
        // 1) create the light
        var flashGO = new GameObject("GlobalDeathFlash");
        flashGO.transform.position = transform.position;
        var L = flashGO.AddComponent<Light>();
        L.type = LightType.Point;
        L.color = flashColor;
        L.intensity = startIntensity;
        L.range = maxRange;
        L.shadows = LightShadows.None;

        // 2) fade out over time
        float elapsed = 0f;
        while (elapsed < flashDuration)
        {
            float t = elapsed / flashDuration;
            L.intensity = Mathf.Lerp(startIntensity, 0f, t);
            L.range = Mathf.Lerp(maxRange, 0f, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 3) clean up the light
        Destroy(flashGO);

        // 4) finally destroy the enemy
        Destroy(gameObject);
    }
}
