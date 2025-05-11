using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreenUI : MonoBehaviour
{
    public void OnRestartLevel()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene("SampleScene");
    }

    public void OnReturnHome()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("HomeScreen");
    }
}
