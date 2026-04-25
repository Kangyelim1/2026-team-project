using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class StoryLine
{
    public int id;
    public int stage;
    public string characterName;
    public string text;
    public string leftPortraitPath;
    public string rightPortraitPath;
    public string backgroundPath;
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
    public Image backgroundImage;
    public Image leftPortraitImage;
    public Image rightPortraitImage;

    [Header("다음 씬 이름")]
    public string nextSceneName = "BattleScene";

    private List<StoryLine> currentStageLines = new List<StoryLine>();
    private int currentIndex = 0;
    private int currentStageNumber = 1;

    void Start()
    {
        currentStageNumber = PlayerPrefs.GetInt("CurrentStage", 1);
        Debug.Log($"현재 스토리 스테이지: {currentStageNumber}");

        LoadStoryJson();
        ShowCurrentLine();
    }

    void Update()
    {
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
                if (line.stage == currentStageNumber) currentStageLines.Add(line);
        }
    }

    void ShowCurrentLine()
    {
        if (currentIndex < currentStageLines.Count)
        {
            StoryLine currentLine = currentStageLines[currentIndex];
            if (speakerText != null) speakerText.text = currentLine.characterName.Replace("(시스템)", "시스템");
            if (dialogueText != null) dialogueText.text = currentLine.text.Replace("\\n", "\n");

            UpdateVisuals(currentLine);
        }
    }

    void UpdateVisuals(StoryLine currentLine)
    {
        if (backgroundImage != null)
        {
            string targetBg = "Bg_Yard";
            if (!string.IsNullOrEmpty(currentLine.backgroundPath)) targetBg = currentLine.backgroundPath.Trim();
            else
            {
                if (currentStageNumber == 2) targetBg = "Bg_Morning";
                else if (currentStageNumber == 3 || currentStageNumber == 4) targetBg = "Bg_Village";
                else if (currentStageNumber >= 5) targetBg = "Bg_NightBridge";
            }

            Sprite bgSprite = Resources.Load<Sprite>($"Backgrounds/{targetBg}");
            if (bgSprite != null)
            {
                backgroundImage.sprite = bgSprite;
                backgroundImage.color = new Color(1f, 1f, 1f, 1f);
            }
        }

        if (!string.IsNullOrEmpty(currentLine.leftPortraitPath) && leftPortraitImage != null)
        {
            leftPortraitImage.gameObject.SetActive(true);
            Sprite leftSprite = Resources.Load<Sprite>($"Portraits/{currentLine.leftPortraitPath.Trim()}");
            if (leftSprite != null) leftPortraitImage.sprite = leftSprite;
        }
        else if (leftPortraitImage != null) leftPortraitImage.gameObject.SetActive(false);

        if (!string.IsNullOrEmpty(currentLine.rightPortraitPath) && rightPortraitImage != null)
        {
            rightPortraitImage.gameObject.SetActive(true);
            Sprite rightSprite = Resources.Load<Sprite>($"Portraits/{currentLine.rightPortraitPath.Trim()}");
            if (rightSprite != null) rightPortraitImage.sprite = rightSprite;
        }
        else if (rightPortraitImage != null) rightPortraitImage.gameObject.SetActive(false);

        Color activeColor = new Color(1f, 1f, 1f, 1f);
        Color dimColor = new Color(0.4f, 0.4f, 0.4f, 1f);

        string speaker = currentLine.characterName.Trim();
        bool isSystem = string.IsNullOrEmpty(speaker) || speaker.Contains("시스템") || speaker.Contains("해설") || speaker.Contains("내면") || speaker.Contains("정체불명");

        if (isSystem)
        {
            if (leftPortraitImage != null) leftPortraitImage.color = dimColor;
            if (rightPortraitImage != null) rightPortraitImage.color = dimColor;
        }
        else if (speaker == "콩쥐" || speaker.Contains("해님") || speaker.Contains("달님"))
        {
            if (leftPortraitImage != null) leftPortraitImage.color = activeColor;
            if (rightPortraitImage != null) rightPortraitImage.color = dimColor;
        }
        else
        {
            if (leftPortraitImage != null) leftPortraitImage.color = dimColor;
            if (rightPortraitImage != null) rightPortraitImage.color = activeColor;
        }
    }

    public void NextLine()
    {
        currentIndex++;
        if (currentIndex >= currentStageLines.Count)
        {
            // ★ 수정: 5스테이지 대사가 끝나면 게임클리어가 아니라 찐막 보스전으로 이동!
            if (currentStageNumber >= 5)
            {
                Debug.Log("5스테이지 스토리 완료! 마지막 자신과의 전투 시작!");
                SceneManager.LoadScene(nextSceneName);
                return;
            }

            if (PlayerPrefs.GetInt("NeedEvent", 0) == 1)
            {
                PlayerPrefs.SetInt("NeedEvent", 0);
                PlayerPrefs.Save();
                if (EventManager.Instance != null) EventManager.Instance.ShowPriestEvent();
                return;
            }

            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            ShowCurrentLine();
        }
    }
}