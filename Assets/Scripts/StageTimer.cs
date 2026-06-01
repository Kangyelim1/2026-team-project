using TMPro;
using UnityEngine;

public class StageTimer : MonoBehaviour
{
    [Header("제한 시간")]
    public float limitTime = 40f;

    [Header("시간 표시 UI")]
    public TextMeshProUGUI timerText;

    
    private GameManger gameManger;

    private bool isTimeOver;

    private void Start()
    {
       
        gameManger = FindAnyObjectByType<GameManger>();
    }

    private void Update()
    {
        if (isTimeOver)
            return;

        
        limitTime -= Time.deltaTime;


        int minutes = Mathf.FloorToInt(limitTime / 60);
        int seconds = Mathf.FloorToInt(limitTime % 60);

        timerText.text = $"{minutes}:{seconds:00}";


        if (limitTime <= 0)
        {
            limitTime = 0;
            isTimeOver = true;

            Debug.Log("시간 초과");

            
            gameManger.isDiePlayer = true;
        }
    }
}