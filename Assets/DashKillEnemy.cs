using UnityEngine;
using System.Collections;
using System;

public class EnemyHealth : MonoBehaviour
{
    [Header("Flash Settings")]
    public float flashDuration = 60f;
    public float startIntensity = 10f;
    public float maxRange = 50f;
    public Color flashColor = Color.white;
    public GameObject deathParticles;
    public static event Action OnEnemyKilled;

    [Header("Glint Drop")]
    public GameObject glintPrefab;    // assign your Glint prefab here
    public int glintDropAmount = 1;   // how many to spawn

    bool hasDied = false;

    public void TakeDamage(float damage)
    {
        if (hasDied) return;
        hasDied = true;

        // disable collider so you don’t retrigger
        var col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        if (deathParticles != null)
            Instantiate(deathParticles, transform.position, Quaternion.identity);

        StartCoroutine(ExplodeFlashAndDie());
    }


    IEnumerator ExplodeFlashAndDie()
    {
        // 1) flash light
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

        Destroy(flashGO);

        // 3) spawn Glint(s)
        if (glintPrefab != null)
        {
            for (int i = 0; i < glintDropAmount; i++)
            {
                Instantiate(glintPrefab, transform.position, Quaternion.identity);
            }
        }

        // 4) finally destroy the enemy
        OnEnemyKilled?.Invoke();
        Destroy(gameObject);
    }
}
