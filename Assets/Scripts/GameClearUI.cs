using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClearUI : MonoBehaviour
{
    public static GameClearUI Instance;

    public GameObject gameClearPanel;

    void Awake()
    {
        Instance = this;

        // 시작할 때 비활성화
        gameClearPanel.SetActive(false);
    }

    public void ShowGameClear()
    {
        Debug.Log("게임 클리어 패널 열기");
        gameClearPanel.SetActive(true);
    }
}
