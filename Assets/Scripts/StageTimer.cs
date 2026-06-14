using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class StageTimer : MonoBehaviour
{
    [Header("제한 시간")]
    public float limitTime = 40f;

    [Header("게임 시작 UI")]
    public RectTransform warningUI;
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI warningText;


    [Header("시간 표시 UI")]
    public TextMeshProUGUI timerText;

    private GameManger gameManger;

    // 시간이 끝났는지 확인
    private bool isTimeOver;

    private bool isStartGame;

  
    [HideInInspector]
    public bool isTimerStop = false;

    private void Start()
    {
        gameManger = FindAnyObjectByType<GameManger>();
        timerText.gameObject.SetActive(false);
        // 게임 시작 시 현재 시간 표시
        UpdateTimerText();

        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        Debug.Log("게임 준비");

        // 시작하자마자 문구 출력
        yield return StartCoroutine(WarningMessage("GameStart"));

        // 문구가 끝나면 타이머 표시
        timerText.gameObject.SetActive(true);

        isStartGame = true;
    }

    public IEnumerator WarningMessage(string message)
    {
        warningText.text = message;

        warningUI.rotation = Quaternion.Euler(0, 0, 15);
        warningUI.anchoredPosition = new Vector2(-1800, -300);

        canvasGroup.alpha = 0;
        warningUI.gameObject.SetActive(true);

        Sequence seq = DOTween.Sequence();

        seq.Append(canvasGroup.DOFade(1f, 0.15f));

        seq.Append(warningUI.DOAnchorPos(new Vector2(0, 0), 0.75f).SetEase(Ease.OutQuad));

        seq.AppendInterval(0.5f);

        seq.Append(warningUI.DOAnchorPos(new Vector2(1800, 300), 0.75f).SetEase(Ease.InQuad));

        seq.Append(canvasGroup.DOFade(0f, 0.15f));

        yield return seq.WaitForCompletion();

        warningUI.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!isStartGame || isTimeOver || isTimerStop)
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