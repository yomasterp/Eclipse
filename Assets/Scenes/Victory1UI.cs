using UnityEngine;
using UnityEngine.SceneManagement;

public class Victory1ScreenUI : MonoBehaviour
{
    public void OnNextLevel()
    {
        SceneManager.LoadScene("Level2"); // Next level after Level1
    }

    public void OnReturnHome()
    {
        SceneManager.LoadScene("HomeScreen");
    }
}
