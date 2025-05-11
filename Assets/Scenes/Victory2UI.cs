using UnityEngine;
using UnityEngine.SceneManagement;

public class Victory2ScreenUI : MonoBehaviour
{
    public void OnReturnHome()
    {
        SceneManager.LoadScene("HomeScreen");
    }
}