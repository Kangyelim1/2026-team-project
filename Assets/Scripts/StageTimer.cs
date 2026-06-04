using TMPro;
using UnityEngine;

public class StageTimer : MonoBehaviour
{
    [Header("제한 시간")]
    public float limitTime = 40f;

    [Header("시간 표시 UI")]
    public TextMeshProUGUI timerText;

    private GameManger gameManger;

    // 시간이 끝났는지 확인
    private bool isTimeOver;

  
    [HideInInspector]
    public bool isTimerStop = false;

    private void Start()
    {
        gameManger = FindAnyObjectByType<GameManger>();

        // 게임 시작 시 현재 시간 표시
        UpdateTimerText();
    }

    private void Update()
    {
        // 이미 시간초과 상태면 종료
        if (isTimeOver)
            return;

        // 타이머 정지 상태면 시간 감소 안함
        if (isTimerStop)
            return;

        // 시간 감소
        limitTime -= Time.deltaTime;

        // 0 이하로 내려가는 것 방지
        if (limitTime < 0)
            limitTime = 0;

        // UI 갱신
        UpdateTimerText();

        // 시간 종료
        if (limitTime <= 0)
        {
            isTimeOver = true;

            Debug.Log("시간 초과");

            if (gameManger != null)
            {
                gameManger.isDiePlayer = true;
            }
        }
    }

    
    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(limitTime / 60);
        int seconds = Mathf.FloorToInt(limitTime % 60);

        timerText.text = $"{minutes}:{seconds:00}";
    }

    // 외부에서 호출해서 타이머 정지
    public void StopTimer()
    {
        isTimerStop = true;
    }

    // 외부에서 호출해서 타이머 재시작
    public void ResumeTimer()
    {
        isTimerStop = false;
    }
}