using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuSystem : MonoBehaviour
{

    public void StartGame()
    {
        SceneManager.LoadScene("Stage01");
    }
    public void OpenSetting()
    {
        SceneManager.LoadScene("Setting");
    }


    public void BackMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Quit()
    {
        Application.Quit();

    }
}
