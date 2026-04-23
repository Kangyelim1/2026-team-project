using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// JSON 데이터 구조
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
        // 화면을 클릭하면 다음 대사로 넘어갑니다.
        if (Input.GetMouseButtonDown(0)) NextLine();
    }

    void LoadStoryJson()
    {
        TextAsset storyFile = Resources.Load<TextAsset>("JsonFile/StoryData");
        if (storyFile == null)
        {
            Debug.LogError("StoryData.json 파일을 찾을 수 없습니다!");
            return;
        }

        StoryDataWrapper wrapper = JsonUtility.FromJson<StoryDataWrapper>(storyFile.text);
        if (wrapper != null && wrapper.rows != null)
        {
            foreach (var line in wrapper.rows)
            {
                // 현재 스테이지 번호와 일치하는 대사만 가져옵니다.
                if (line.stage == currentStageNumber)
                    currentStageLines.Add(line);
            }
        }

        // 만약 해당 스테이지에 대사가 아예 없으면 멈추지 않고 바로 전투로 보냅니다! (오류 방지)
        if (currentStageLines.Count == 0)
        {
            Debug.LogWarning($"스테이지 {currentStageNumber}에 해당하는 대사가 없어 바로 다음 씬으로 넘어갑니다.");
            SceneManager.LoadScene(nextSceneName);
        }
    }

    void ShowCurrentLine()
    {
        if (currentIndex < currentStageLines.Count)
        {
            StoryLine currentLine = currentStageLines[currentIndex];

            // 이름 출력 (괄호 등을 깔끔하게 정리)
            if (speakerText != null)
                speakerText.text = currentLine.characterName.Replace("(시스템)", "시스템");

            // 대사 출력 (JSON에서 \n으로 쓴 줄바꿈이 유니티에서 진짜 줄바꿈이 되도록 치환해 줍니다!)
            if (dialogueText != null)
            {
                string parsedText = currentLine.text.Replace("\\n", "\n");
                dialogueText.text = parsedText;
            }

            UpdateVisuals(currentLine);
        }
    }

    void UpdateVisuals(StoryLine currentLine)
    {
        // =========================================================
        // 1. 배경 이미지 업데이트
        // =========================================================
        if (backgroundImage != null)
        {
            string targetBg = "Bg_Yard"; // 기본값 (마당)

            // JSON에 배경이 쓰여있다면 무조건 그걸 최우선으로 씁니다!
            if (!string.IsNullOrEmpty(currentLine.backgroundPath))
            {
                targetBg = currentLine.backgroundPath.Trim();
            }
            else
            {
                // JSON에 배경이 안 적혀있을 때를 대비한 2차 안전장치 (스테이지별 강제 고정)
                if (currentStageNumber == 2) targetBg = "Bg_Morning";
                else if (currentStageNumber == 3 || currentStageNumber == 4) targetBg = "Bg_Village";
                else if (currentStageNumber == 5) targetBg = "Bg_Night";
            }

            Sprite bgSprite = Resources.Load<Sprite>($"Backgrounds/{targetBg}");
            if (bgSprite != null)
            {
                backgroundImage.sprite = bgSprite;
                backgroundImage.color = new Color(1f, 1f, 1f, 1f);
            }
            else
            {
                Debug.LogWarning($"배경 이미지를 찾을 수 없습니다: Backgrounds/{targetBg}");
            }
        }

        // =========================================================
        // 2. 캐릭터 초상화 업데이트 (Trim()으로 보이지 않는 공백 제거)
        // =========================================================
        // 왼쪽 초상화(콩쥐 자리)
        if (!string.IsNullOrEmpty(currentLine.leftPortraitPath) && leftPortraitImage != null)
        {
            leftPortraitImage.gameObject.SetActive(true);
            Sprite leftSprite = Resources.Load<Sprite>($"Portraits/{currentLine.leftPortraitPath.Trim()}");
            if (leftSprite != null) leftPortraitImage.sprite = leftSprite;
        }
        else if (leftPortraitImage != null) leftPortraitImage.gameObject.SetActive(false); // 사진 없으면 숨김

        // 오른쪽 초상화(적 자리)
        if (!string.IsNullOrEmpty(currentLine.rightPortraitPath) && rightPortraitImage != null)
        {
            rightPortraitImage.gameObject.SetActive(true);
            Sprite rightSprite = Resources.Load<Sprite>($"Portraits/{currentLine.rightPortraitPath.Trim()}");
            if (rightSprite != null) rightPortraitImage.sprite = rightSprite;
        }
        else if (rightPortraitImage != null) rightPortraitImage.gameObject.SetActive(false); // 사진 없으면 숨김

        // =========================================================
        // 3. 화자에 따른 명암 조절 로직 (정교하게 업그레이드!)
        // =========================================================
        Color activeColor = new Color(1f, 1f, 1f, 1f);     // 말하는 사람 (밝게)
        Color dimColor = new Color(0.4f, 0.4f, 0.4f, 1f);  // 안 말하는 사람 (어둡게)

        string speaker = currentLine.characterName.Trim();

        // 내면, 시스템, 정체불명 등은 화면의 캐릭터가 직접 말하는 게 아니므로 둘 다 어둡게!
        bool isSystem = string.IsNullOrEmpty(speaker) || speaker.Contains("시스템") || speaker.Contains("해설") || speaker.Contains("내면") || speaker.Contains("정체불명");

        if (isSystem)
        {
            if (leftPortraitImage != null) leftPortraitImage.color = dimColor;
            if (rightPortraitImage != null) rightPortraitImage.color = dimColor;
        }
        // "콩쥐" 본인일 때만 왼쪽을 밝게! ("미래의 콩쥐"는 오른쪽 캐릭터이므로 예외 처리)
        else if (speaker == "콩쥐" || speaker.Contains("해님") || speaker.Contains("달님"))
        {
            if (leftPortraitImage != null) leftPortraitImage.color = activeColor;
            if (rightPortraitImage != null) rightPortraitImage.color = dimColor;
        }
        // 팥쥐, 계모, 원님, 마을사람, 미래의 콩쥐 등 상대방이 말할 때는 오른쪽을 밝게!
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