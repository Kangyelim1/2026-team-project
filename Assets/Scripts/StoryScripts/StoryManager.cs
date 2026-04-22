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
    [Header("UI")]
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI dialogueText;

    [Header("Portrait UI")]
    public Image leftPortraitImage;
    public Image rightPortraitImage;

    [Header("Settings")]
    public string nextSceneName = "BattleScene";

    private List<StoryLine> currentStageLines = new List<StoryLine>();
    private int currentIndex = 0;
    private int currentStageNumber = 1;

    void Start()
    {
        currentStageNumber = PlayerPrefs.GetInt("CurrentStage", 1);
        Debug.Log($"StoryManager Start: Stage {currentStageNumber}");

        LoadStoryJson();

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

            if (speakerText != null) speakerText.text = currentLine.characterName;
            if (dialogueText != null) dialogueText.text = currentLine.text;

            UpdatePortraits(currentLine);
        }
    }

    void UpdatePortraits(StoryLine currentLine)
    {
        if (string.IsNullOrEmpty(currentLine.portraitPath))
        {
            if (leftPortraitImage != null && leftPortraitImage.sprite != null) leftPortraitImage.color = new Color(0.5f, 0.5f, 0.5f, 1);
            if (rightPortraitImage != null && rightPortraitImage.sprite != null) rightPortraitImage.color = new Color(0.5f, 0.5f, 0.5f, 1);
            return;
        }

        Sprite loadedSprite = Resources.Load<Sprite>("Portraits/" + currentLine.portraitPath);
        if (loadedSprite == null)
        {
            Debug.LogWarning($"Portrait 이미지 없음: {currentLine.portraitPath}");
            return;
        }

        if (currentLine.characterName.Contains("콩쥐") || currentLine.characterName.Contains("플레이어"))
        {
            if (leftPortraitImage != null)
            {
                leftPortraitImage.sprite = loadedSprite;
                leftPortraitImage.color = new Color(1, 1, 1, 1);
            }
            if (rightPortraitImage != null && rightPortraitImage.sprite != null)
            {
                rightPortraitImage.color = new Color(0.5f, 0.5f, 0.5f, 1);
            }
        }
        else
        {
            if (rightPortraitImage != null)
            {
                rightPortraitImage.sprite = loadedSprite;
                rightPortraitImage.color = new Color(1, 1, 1, 1);
            }
            if (leftPortraitImage != null && leftPortraitImage.sprite != null)
            {
                leftPortraitImage.color = new Color(0.5f, 0.5f, 0.5f, 1);
            }
        }
    }

    public void NextLine()
    {
        currentIndex++;
        if (currentIndex >= currentStageLines.Count)
        {
            Debug.Log($"스테이지 {currentStageNumber} 대사 종료. 전투 씬 전환.");

            // ★[추가된 로직] 전투 씬에 진입하기 전에 현재 스토리에 알맞은 '적 ID'를 저장합니다.
            PlayerPrefs.SetInt("TargetEnemyID", currentStageNumber);
            PlayerPrefs.Save();

            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            ShowCurrentLine();
        }
    }
}