using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossClearSystem : MonoBehaviour
{
    [Header("클리어 캔버스")]
    public GameObject clearCanvas;

    private void Start()
    {

        // 시작 시 패널 숨기기

        clearCanvas.SetActive(false);
    }

    
    // 게임 클리어 실행
    
    public void GameClear()
    {
        StartCoroutine(ClearCoroutine());
    }

    private IEnumerator ClearCoroutine()
    {

        // 승리 패널 표시

        clearCanvas.SetActive(true);

        Debug.Log("게임 클리어");

        
        yield return new WaitForSeconds(5f);

        
        SceneManager.LoadScene("MainMenu");
    }
}