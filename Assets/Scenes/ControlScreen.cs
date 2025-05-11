using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlsScreenUI : MonoBehaviour
{
    public void OnHome()
    {
        SceneManager.LoadScene("HomeScreen");
    }

}
