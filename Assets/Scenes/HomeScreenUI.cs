using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeScreenUI : MonoBehaviour
{
    public void OnStartGame()
    {
        // Load your main gameplay scene
        SceneManager.LoadScene("SampleScene"); // change to = actual scene name
    }

    public void OnQuit()
    {
        Application.Quit();
    }
}