using UnityEngine;
using UnityEngine.SceneManagement;  // only if you want to reload or change scenes

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("UI")]
    [Tooltip("Assign your Game Over panel here")]
    public GameObject deathScreen;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;

        if (deathScreen != null)
            deathScreen.SetActive(false);
    }

    /// <summary>
    /// Call this to damage the player
    /// </summary>
    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log($"{name} took {amount} damage. Remaining health: {currentHealth}");

        if (currentHealth <= 0f)
            Die();
    }

    private void Die()
    {
        isDead = true;
        Debug.Log($"{name} has died.");

        // 1) Show your death screen
        if (deathScreen != null)
            deathScreen.SetActive(true);

        // 2) Stop time (optional)
        Time.timeScale = 0f;

        // 3) (Optional) Unlock cursor so user can click UI buttons
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 4) (Optional) If you have a separate Game Over scene:
        // SceneManager.LoadScene("GameOver");
    }

    // ... (other methods unchanged)
}
