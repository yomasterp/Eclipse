using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Flash Settings")]
    public float flashDuration = 60f;
    public float startIntensity = 10f;
    public float maxRange = 50f;
    public Color flashColor = Color.white;
    public GameObject deathParticles;

    [Header("Glint Drop")]
    public GameObject glintPrefab;
    public int glintDropAmount = 1;

    public static event Action OnEnemyKilled;
    private bool hasDied = false;

    public void TakeDamage(float damage)
    {
        if (hasDied) return;
        hasDied = true;

        // 1) stop navmesh movement
        var agent = GetComponent<NavMeshAgent>();
        if (agent != null) agent.enabled = false;

        // 2) stop AI update logic
        var ai = GetComponent<EnemyAI>();
        if (ai != null) ai.enabled = false;

        // 3) disable collider
        var col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // 4) hide all renderers immediately
        foreach (var rend in GetComponentsInChildren<Renderer>())
            rend.enabled = false;

        // 5) still play death particles
        if (deathParticles != null)
            Instantiate(deathParticles, transform.position, Quaternion.identity);

        // 6) continue with your flash & glint drop coroutine
        StartCoroutine(ExplodeFlashAndDie());
    }

    private IEnumerator ExplodeFlashAndDie()
    {
        // flash light
        var flashGO = new GameObject("GlobalDeathFlash");
        flashGO.transform.position = transform.position;
        var L = flashGO.AddComponent<Light>();
        L.type = LightType.Point;
        L.color = flashColor;
        L.intensity = startIntensity;
        L.range = maxRange;
        L.shadows = LightShadows.None;

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

        // spawn glints
        if (glintPrefab != null)
        {
            for (int i = 0; i < glintDropAmount; i++)
                Instantiate(glintPrefab, transform.position, Quaternion.identity);
        }

        // fire kill event and destroy
        OnEnemyKilled?.Invoke();
        Destroy(gameObject);
    }
}
