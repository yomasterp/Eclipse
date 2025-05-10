using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Glint UI")]
    public TMP_Text glintText;
    // public TMP_Text glintText;   // if using TextMeshPro

    [Header("Kill UI")]
    [Tooltip("Drag your KillText UI element here")]
    public TMP_Text killText;

    [Header("Health UI")]
    public Slider healthBar;

    [Header("Game Over")]
    public GameObject gameOverPanel;

    int _currentGlints = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // init UI
        glintText.text = "0";
        healthBar.value = healthBar.maxValue;
        gameOverPanel.SetActive(false);

        if (killText != null)
            killText.text = "Kills: 0/0";
    }

    public void AddGlints(int amount)
    {
        _currentGlints += amount;
        glintText.text = "Glints: " + _currentGlints.ToString();
    }

    public void UpdateHealth(int currentHealth)
    {
        healthBar.value = currentHealth;
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        // optionally: Time.timeScale = 0;

        // pause the game
        Time.timeScale = 0f;

        // UNLOCK & SHOW cursor so UI buttons are clickable
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void RestartLevel()
    {
        // 1) Un-pause the game
        Time.timeScale = 1f;
        // 2) Reload the active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Call this to update the on-screen kill counter
    /// </summary>
    public void UpdateKillCounter(int current, int required)
    {
        if (killText != null)
            killText.text = $"Kills: {current}/{required}";
    }


}
