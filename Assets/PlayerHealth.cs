using UnityEngine;
using UnityEngine.UI;           // <-- for Slider
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("UI")]
    [Tooltip("Assign your Game Over panel here")]
    public GameObject deathScreen;

    [Tooltip("Drag in your HealthBar slider here")]
    public Slider healthBar;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;

        if (deathScreen != null)
            deathScreen.SetActive(false);

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    /// <summary>
    /// Call this to damage the player
    /// </summary>
    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        if (healthBar != null)
            healthBar.value = currentHealth;

        Debug.Log($"{name} took {amount} damage. Remaining health: {currentHealth}");

        if (currentHealth <= 0f)
            Die();
    }

    private void Die()
    {
        isDead = true;
        Debug.Log($"{name} has died.");

        // Try to hand off to UIManager if available
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGameOver();
            return;
        }

        // Fallback: use your existing panel + pause
        if (deathScreen != null)
            deathScreen.SetActive(true);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Or, if you want to load a GameOver scene:
        // SceneManager.LoadScene("GameOver");
    }
}
