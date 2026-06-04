using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class KeypadMission : MonoBehaviour
{
    [Header("UI")]
    public GameObject missionCanvas;

    [Header("시간")]
    public float missionTime = 20f;

    public TextMeshProUGUI timerText;

    [Header("씬 이동")]
    public string nextSceneName;

    [Header("숫자 버튼들")]
    public RectTransform[] buttonTransforms;

    [Header("숫자 버튼")]
    public Button[] numberButtons;


    private int currentNumber = 1;

    private bool isMission;

    private float currentTime;

    public void StartMission()
    {
        missionCanvas.SetActive(true);

        currentNumber = 1;

        // 추가
        currentTime = missionTime;

        isMission = true;

        foreach (Button btn in numberButtons)
        {
            btn.GetComponent<Image>().color = Color.white;
        }

        ShuffleButtons();

        Debug.Log("숫자 미션 시작");
    }

    private void Update()
    {
        if (!isMission)
            return;

        currentTime -= Time.deltaTime;

        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);

        timerText.text = $"{minutes}:{seconds:00}";

        if (currentTime <= 0)
        {
            currentTime = 0;
            MissionFail();
        }
    }

    
    public void PressNumber(int number)
    {
        Debug.Log("누른 숫자 : " + number);

        if (number == currentNumber)
        {
            // 눌린 버튼 색 변경
            Image buttonImage =
                numberButtons[number - 1].GetComponent<Image>();

            buttonImage.color = Color.darkRed;

            currentNumber++;

            Debug.Log("정답");

            if (currentNumber > 9)
            {
                MissionSuccess();
            }
        }
        else
        {

            // 틀렸으면 버튼 색 전부 초기화
            foreach (Button btn in numberButtons)
            {
                btn.GetComponent<Image>().color = Color.white;
            }

            currentNumber = 1;

            Debug.Log("틀림! 처음부터 다시");
        }
    }

    void MissionSuccess()
    {
        Debug.Log("미션 성공");

        SceneManager.LoadScene(nextSceneName);
    }

    void MissionFail()
    {
        Debug.Log("시간 초과");

        isMission = false;

        missionCanvas.SetActive(false);
    }

    void ShuffleButtons()
    {
        for (int i = 0; i < buttonTransforms.Length; i++)
        {
            int randomIndex =
                Random.Range(i, buttonTransforms.Length);

            // 수정된 부분
            RectTransform temp = buttonTransforms[i];

            buttonTransforms[i] = buttonTransforms[randomIndex];

            buttonTransforms[randomIndex] = temp;
        }

        for (int i = 0; i < buttonTransforms.Length; i++)
        {
            buttonTransforms[i].SetSiblingIndex(i);
        }
    }
}