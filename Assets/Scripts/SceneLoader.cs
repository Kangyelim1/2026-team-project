using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    // 문자열로 씬 이름 불러오기
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // 인덱스로 씬 불러오기 (선택)
    public void LoadSceneByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    // 게임 종료 버튼용 (옵션)
    public void QuitGame()
    {
        Debug.Log("게임 종료");
        Application.Quit();
    }
}
