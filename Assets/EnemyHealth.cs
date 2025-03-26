using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    public float flashDuration = 0.5f;
    public float flashIntensity = 10f;
    public float flashRange = 5f;
    public Color flashColor = Color.red;
    public GameObject deathParticles;

    public void TakeDamage(float damage)
    {
        // Create flash light
        GameObject flash = new GameObject("DeathFlash");
        flash.transform.position = transform.position;

        Light flashLight = flash.AddComponent<Light>();
        flashLight.type = LightType.Point;
        flashLight.color = flashColor;
        flashLight.intensity = flashIntensity;
        flashLight.range = flashRange;

        StartCoroutine(FadeLight(flashLight));

        // Spawn death particles if assigned
        if (deathParticles != null)
        {
            Instantiate(deathParticles, transform.position, Quaternion.identity);
        }

        // Destroy enemy
        Destroy(gameObject);
    }

    IEnumerator FadeLight(Light light)
    {
        float startIntensity = light.intensity;
        float elapsed = 0f;

        while (elapsed < flashDuration)
        {
            light.intensity = Mathf.Lerp(startIntensity, 0f, elapsed / flashDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(light.gameObject);
    }
}