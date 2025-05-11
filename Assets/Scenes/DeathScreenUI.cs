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
        SceneManager.LoadScene("HomeScreen");
    }
}
