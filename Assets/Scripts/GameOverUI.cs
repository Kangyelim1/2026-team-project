using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    public static GameOverUI Instance;

    // GameOver 패널
    public GameObject gameOverPanel;

    void Awake()
    {
        Instance = this;

        // 시작 시 패널 꺼두기
        gameOverPanel.SetActive(false);
    }

    // 게임 오버 패널 열기
    public void ShowGameOver()
    {
        Debug.Log("Game Over 패널 열기");

        gameOverPanel.SetActive(true);
    }

    // 게임 오버 패널 닫기 (선택사항)
    public void HideGameOver()
    {
        gameOverPanel.SetActive(false);
    }
}