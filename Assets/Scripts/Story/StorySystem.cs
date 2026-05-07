using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor;
using System;
using Unity.VisualScripting;

public class StorySystem : MonoBehaviour
{
    public StoryDataSO StoryDataSO;
    public QuestSystem questSystem;

    public int current_StoryID;
    public int current_StoryCount;
    public bool iscurrent_StoryImage;

    public string current_ImageName;
  

    public bool isStoryEndPoint;
    public bool isFinishStory;
    public string current_TargetAudio;
    public string current_TargetMusic;

    [Header("스토리 UI")]
    public GameObject storyImageGameObejct;
    public RawImage storyImageTextor;
    public GameObject StoryUI;
    public Button StoryButton;
    public TextMeshProUGUI characterNameText;
    public Image nameBox;
    public TextMeshProUGUI StoryDialogue;

    public AudioSource storyDubbingAudioSource;

    public bool isNotStoryTimedelay;
    public bool isStory;
    public bool isStorySkip;

    private float typingSpeed = 0.05f;
    private string characterName;
    private bool isStoryTIme;
    private bool isTyping;


    
    private void Start()
    {
        questSystem = GetComponent<QuestSystem>();
        StoryButton.onClick.AddListener(NextStory);
    }

    public void QuestStory(int StoryID)     
    {
        // QuestSystem에서 전달 받은 스토리 ID 조회
        if (StoryID == 0 && questSystem.finishQuest == false)
        {
            Debug.Log("전투 및 이동 퀘스트 진행중");
            return;
        }
        else
        {
            NewStoryData story = StoryDataSO.storys.Find(s => s.Story_ID == StoryID);

            if (story != null)
            {
                isFinishStory = false;
                isStoryEndPoint = false;
                isStory = true;
                StoryUI.gameObject.SetActive(true);
                ShowStory(story);
            }
            else
                Debug.Log("스토리 아이디를 못찾음");
            

            int currnetstoryIdCount = StoryDataSO.storys.FindIndex(sid => sid.Story_ID == current_StoryID);
            Debug.Log($"현재 스토리 아이디: {current_StoryID} 스토리 아이디의 인덱스: {currnetstoryIdCount}");
            current_StoryCount = currnetstoryIdCount;
            isStoryEndPoint = false;
        }
    }

    void NextStory()
    {
        // 다음 퀘스트 조회
        if (isStoryEndPoint == true || isStoryTIme || isFinishStory) return;

        if (isTyping)
        {
            isStorySkip = true;
        }
        else
        {
            Debug.Log($"이전 스토리 아이디: {current_StoryID}");

            current_StoryCount += 1;

            NewStoryData nextStory = StoryDataSO.storys[current_StoryCount];
            ShowStory(nextStory);
            Debug.Log($"현재 스토리 아이디: {current_StoryID}, 엔드 포인트 여부: {nextStory.EndPoint}");

            if (nextStory.EndPoint == true)
            {
                StoryUI.gameObject.SetActive(false);
                Debug.Log($"현재 퀘스트 스토리 종료");
                isFinishStory = true;
                isStoryEndPoint = true;
                isStory = false;
                return;
            }
        }
    }
    void ShowStory(NewStoryData story)
    {
        current_StoryID = story.Story_ID;
        characterName = story.Speaker;
        iscurrent_StoryImage = story.Is_Image;
        current_TargetAudio = story.TargetAudio;
        current_ImageName = story.TargetImageName;
        current_TargetMusic = story.TargetMusic;
        TyepingStory(characterName, story.Dialogue);
        CurrentStoryAsset();
    }

    void CurrentStoryAsset()
    {
        ShowImage();
    }

    void TyepingStory(string characterSpeaker, string Dialogue)
    {
        characterNameText.text = $"{characterSpeaker}";
        
        if (isStorySkip)
        {
            isTyping = false;
            StoryDialogue.text = "";
            StoryDialogue.text = Dialogue;
            isStorySkip = false;
        }
        else
            StartCoroutine(TypeDialogue(Dialogue));
    }

    IEnumerator TypeDialogue(string dialogue)          // 스토리 진행시 타이핑 효과
    {
        StoryDialogue.text = "";
     
        for (int i = 0; i < dialogue.Length + 1; i++)
        {
            if (isStorySkip)
            {
                isTyping = false;
                StoryDialogue.text = "";
                StoryDialogue.text = dialogue;
                yield break;
            }

            StoryDialogue.text = dialogue.Substring(0, i);
            yield return new WaitForSeconds(typingSpeed);

            if (i >= dialogue.Length) isTyping = false;
            else isTyping = true;
        }
    }

    void ShowImage()        // 스토리 이미지
    {
        if (iscurrent_StoryImage)
        {
            Texture2D texture = Resources.Load<Texture2D>(current_ImageName);

            if (texture != null)
            {
                RawImage rawImage = storyImageTextor.GetComponent<RawImage>();
                if (rawImage != null)
                {
                    rawImage.texture = texture;
                    storyImageGameObejct.SetActive(true);
                }
            }
        }
        else
        {
            storyImageGameObejct.SetActive(false);
        }
    }

}