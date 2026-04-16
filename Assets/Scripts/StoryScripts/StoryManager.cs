using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class StoryLine
{
    public int id;
    public int stage;
    public string characterName;
    public string text;
    public string portraitPath;
}

[System.Serializable]
public class StoryDataWrapper
{
    public List<StoryLine> rows = new List<StoryLine>();
}

public class StoryManager : MonoBehaviour
{
    [Header("UI 연결")]
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI dialogueText;

    // ★ [핵심 변경] 이미지를 왼쪽(주인공)과 오른쪽(상대방) 두 개로 나눕니다.
    [Header("캐릭터 일러스트 UI")]
    public Image leftPortraitImage;  // 왼쪽 일러스트 (주로 콩쥐)
    public Image rightPortraitImage; // 오른쪽 일러스트 (주로 계모, 팥쥐 등)

    [Header("다음 씬 이름")]
    public string nextSceneName = "BattleScene";

    private List<StoryLine> currentStageLines = new List<StoryLine>();
    private int currentIndex = 0;
    private int currentStageNumber = 1;

    void Start()
    {
        currentStageNumber = PlayerPrefs.GetInt("CurrentStage", 1);
        Debug.Log($"[StoryManager] 현재 스테이지는 {currentStageNumber} 입니다.");

        LoadStoryJson();

        // 처음 시작할 때 양쪽 이미지를 모두 투명하게(안 보이게) 초기화합니다.
        if (leftPortraitImage != null) leftPortraitImage.color = new Color(1, 1, 1, 0);
        if (rightPortraitImage != null) rightPortraitImage.color = new Color(1, 1, 1, 0);

        ShowCurrentLine();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            NextLine();
        }
    }

    void LoadStoryJson()
    {
        TextAsset storyFile = Resources.Load<TextAsset>("JsonFile/StoryData");

        if (storyFile == null)
        {
            Debug.LogError("Resources/JsonFile/StoryData.json 파일을 찾을 수 없습니다!");
            return;
        }

        StoryDataWrapper wrapper = JsonUtility.FromJson<StoryDataWrapper>(storyFile.text);
        if (wrapper != null)
        {
            foreach (var line in wrapper.rows)
            {
                if (line.stage == currentStageNumber)
                {
                    currentStageLines.Add(line);
                }
            }
        }
    }

    void ShowCurrentLine()
    {
        if (currentIndex < currentStageLines.Count)
        {
            StoryLine currentLine = currentStageLines[currentIndex];

            // 1. 대사 및 이름 텍스트 반영
            if (speakerText != null) speakerText.text = currentLine.characterName;
            if (dialogueText != null) dialogueText.text = currentLine.text;

            // 2. 일러스트 연출 (누가 말하느냐에 따라 왼쪽/오른쪽 이미지 갱신)
            UpdatePortraits(currentLine);
        }
    }

    // ★ [핵심 추가] 말하는 사람에 따라 왼쪽/오른쪽 이미지를 띄워주고 색상을 조절하는 함수
    void UpdatePortraits(StoryLine currentLine)
    {
        // 시스템 대사이거나 지문이라서 이미지 경로가 비어있다면, 양쪽 다 살짝 어둡게 처리 (또는 유지)
        if (string.IsNullOrEmpty(currentLine.portraitPath))
        {
            if (leftPortraitImage != null && leftPortraitImage.sprite != null) leftPortraitImage.color = new Color(0.5f, 0.5f, 0.5f, 1);
            if (rightPortraitImage != null && rightPortraitImage.sprite != null) rightPortraitImage.color = new Color(0.5f, 0.5f, 0.5f, 1);
            return;
        }

        // Resources 폴더에서 새로운 이미지를 가져옵니다.
        Sprite loadedSprite = Resources.Load<Sprite>("Portraits/" + currentLine.portraitPath);

        if (loadedSprite == null)
        {
            Debug.LogWarning($"[이미지 오류] Portraits/{currentLine.portraitPath} 경로에 이미지가 없습니다.");
            return;
        }

        // 콩쥐(또는 내면/미래의 콩쥐)가 말할 때는 '왼쪽'에 이미지를 띄우고 밝게! 오른쪽은 어둡게!
        if (currentLine.characterName.Contains("콩쥐"))
        {
            if (leftPortraitImage != null)
            {
                leftPortraitImage.sprite = loadedSprite;
                leftPortraitImage.color = new Color(1, 1, 1, 1); // 밝게 (말하는 사람)
            }
            if (rightPortraitImage != null && rightPortraitImage.sprite != null)
            {
                rightPortraitImage.color = new Color(0.5f, 0.5f, 0.5f, 1); // 어둡게 (안 말하는 사람)
            }
        }
        // 계모, 팥쥐, 원님 등 다른 사람이 말할 때는 '오른쪽'에 이미지를 띄우고 밝게! 왼쪽은 어둡게!
        else
        {
            if (rightPortraitImage != null)
            {
                rightPortraitImage.sprite = loadedSprite;
                rightPortraitImage.color = new Color(1, 1, 1, 1); // 밝게 (말하는 사람)
            }
            if (leftPortraitImage != null && leftPortraitImage.sprite != null)
            {
                leftPortraitImage.color = new Color(0.5f, 0.5f, 0.5f, 1); // 어둡게 (안 말하는 사람)
            }
        }
    }

    public void NextLine()
    {
        currentIndex++;

        if (currentIndex >= currentStageLines.Count)
        {
            Debug.Log($"스테이지 {currentStageNumber} 대사가 모두 끝났습니다. 전투 씬으로 갑니다.");
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            ShowCurrentLine();
        }
    }
}