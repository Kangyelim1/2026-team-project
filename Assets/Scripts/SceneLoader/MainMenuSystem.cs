using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuSystem : MonoBehaviour
{
    [Header("보스스테이지 테스트용")]
    public string bossStageName = "BossScene";

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Debug.Log("개발자 모드 - 보스 스테이지 이동");

            SceneManager.LoadScene(bossStageName);
        }
    }

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
