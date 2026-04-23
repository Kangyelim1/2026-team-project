using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// JSON 데이터 구조에 맞게 틀을 짜줍니다.
[System.Serializable]
public class StoryLine
{
    public int id;
    public int stage;
    public string characterName;
    public string text;
    public string leftPortraitPath;  // 왼쪽(콩쥐) 초상화 이름
    public string rightPortraitPath; // 오른쪽(상대방) 초상화 이름
    public string backgroundPath;    // 배경 이름
}

[System.Serializable]
public class StoryDataWrapper
{
    public List<StoryLine> rows = new List<StoryLine>();
}

public class StoryManager : MonoBehaviour
{
    [Header("UI 연결 - 텍스트")]
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI dialogueText;

    [Header("UI 연결 - 이미지")]
    public Image backgroundImage;     // 배경 이미지
    public Image leftPortraitImage;   // 왼쪽 초상화
    public Image rightPortraitImage;  // 오른쪽 초상화

    [Header("다음 씬 이름")]
    public string nextSceneName = "BattleScene";

    private List<StoryLine> currentStageLines = new List<StoryLine>();
    private int currentIndex = 0;
    private int currentStageNumber = 1;

    void Start()
    {
        currentStageNumber = PlayerPrefs.GetInt("CurrentStage", 1);
        LoadStoryJson();
        ShowCurrentLine();
    }

    void Update()
    {
        // 클릭하면 다음 대사로 넘어갑니다.
        if (Input.GetMouseButtonDown(0)) NextLine();
    }

    void LoadStoryJson()
    {
        TextAsset storyFile = Resources.Load<TextAsset>("JsonFile/StoryData");
        if (storyFile == null) return;

        StoryDataWrapper wrapper = JsonUtility.FromJson<StoryDataWrapper>(storyFile.text);
        if (wrapper != null && wrapper.rows != null)
        {
            foreach (var line in wrapper.rows)
            {
                if (line.stage == currentStageNumber) currentStageLines.Add(line);
            }
        }
    }

    void ShowCurrentLine()
    {
        if (currentIndex < currentStageLines.Count)
        {
            StoryLine currentLine = currentStageLines[currentIndex];

            if (speakerText != null) speakerText.text = currentLine.characterName;
            if (dialogueText != null) dialogueText.text = currentLine.text;

            // 여기서 배경과 초상화를 갱신합니다!
            UpdateVisuals(currentLine);
        }
    }

    void UpdateVisuals(StoryLine currentLine)
    {
        // 1. 배경 이미지 로드 (Resources/Backgrounds 폴더에서)
        if (!string.IsNullOrEmpty(currentLine.backgroundPath) && backgroundImage != null)
        {
            Sprite bgSprite = Resources.Load<Sprite>($"Backgrounds/{currentLine.backgroundPath}");
            if (bgSprite != null) backgroundImage.sprite = bgSprite;
        }

        // 2. 왼쪽 초상화(콩쥐) 로드 (Resources/Portraits 폴더에서)
        if (!string.IsNullOrEmpty(currentLine.leftPortraitPath) && leftPortraitImage != null)
        {
            leftPortraitImage.gameObject.SetActive(true);
            Sprite leftSprite = Resources.Load<Sprite>($"Portraits/{currentLine.leftPortraitPath}");
            if (leftSprite != null) leftPortraitImage.sprite = leftSprite;
        }
        else if (leftPortraitImage != null) leftPortraitImage.gameObject.SetActive(false);

        // 3. 오른쪽 초상화(상대방) 로드
        if (!string.IsNullOrEmpty(currentLine.rightPortraitPath) && rightPortraitImage != null)
        {
            rightPortraitImage.gameObject.SetActive(true);
            Sprite rightSprite = Resources.Load<Sprite>($"Portraits/{currentLine.rightPortraitPath}");
            if (rightSprite != null) rightPortraitImage.sprite = rightSprite;
        }
        else if (rightPortraitImage != null) rightPortraitImage.gameObject.SetActive(false);

        // 4. 말하는 사람에 따라 명암(색상) 조절!
        Color activeColor = new Color(1f, 1f, 1f, 1f);     // 밝은 원래 색상
        Color dimColor = new Color(0.4f, 0.4f, 0.4f, 1f);  // 어두운 회색상

        if (currentLine.characterName.Contains("콩쥐"))
        {
            // 콩쥐가 말하면: 왼쪽 밝게, 오른쪽 어둡게
            if (leftPortraitImage != null) leftPortraitImage.color = activeColor;
            if (rightPortraitImage != null) rightPortraitImage.color = dimColor;
        }
        else if (currentLine.characterName.Contains("시스템"))
        {
            // 시스템/해설이면: 둘 다 어둡게
            if (leftPortraitImage != null) leftPortraitImage.color = dimColor;
            if (rightPortraitImage != null) rightPortraitImage.color = dimColor;
        }
        else
        {
            // 다른 캐릭터가 말하면: 오른쪽 밝게, 왼쪽 어둡게
            if (leftPortraitImage != null) leftPortraitImage.color = dimColor;
            if (rightPortraitImage != null) rightPortraitImage.color = activeColor;
        }
    }

    public void NextLine()
    {
        currentIndex++;

        if (currentIndex >= currentStageLines.Count)
        {
            // 이벤트 예약되어 있으면 이벤트 먼저
            if (PlayerPrefs.GetInt("NeedEvent", 0) == 1)
            {
                Debug.Log("스토리 끝 → 이벤트 실행");

                PlayerPrefs.SetInt("NeedEvent", 0);
                PlayerPrefs.Save();

                // 이벤트 패널 실행
                if (EventManager.Instance != null)
                {
                    EventManager.Instance.ShowPriestEvent();
                }

                return; // 여기서 멈춰야 함
            }

            // 이벤트 없으면 전투로
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            ShowCurrentLine();
        }
    }
}