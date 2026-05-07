using JetBrains.Annotations;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class QuestSystem : MonoBehaviour
{
    public static QuestSystem instance;
    public QuestDataSO questData;
    public StorySystem storySystem;

    public int playerQuestID;
    public string playerQuestTitle;
    public string playerQuestDescription;
    public string playerQuestType;
    public int playerQuestStoryID;
    public string currentQuestEnemyNPC;
    public string playerQuestLocation_Asset;
    public string currentBGMAeest;
    public int currentQuestEnemyCount;
    public string currentRewards;

    public bool finishQuest;

    public int currnet_EnmeyDieCount;

    public GameObject QuestCanavarse;

    [Header("플레이어 정보")]
    public int playerLevel = 1;
    public int playerExperience = 0;

    [Header("퀘스트 UI")]
    //public TextMeshProUGUI cuttentQuestName;
    //public TextMeshProUGUI questText;

    private int currentQuestIndex = 0;

    public QuestAndStoryDatabase _questAndStoryDatabase;

    // 추후 저장 시스탬 구연후 사용
    private int lastQuestIndex;


    public bool playerquest_Is_success;
    public int currentQuestAndSotorys;
    private bool isProcessingQuest = false;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        storySystem = GetComponent<StorySystem>();

        if (playerQuestID == 0 && questData.quests.Count > 0)
        {
            // 신규 플레이
            QuestData firstQuest = questData.quests[0];
            ShowQuest(firstQuest);

        }
        else
        {
            QuestData quest = questData.quests.Find(q => q.Quest_ID == playerQuestID);
            if (quest != null)
            {
                currentQuestIndex = lastQuestIndex;
                ShowQuest(quest);
            }
        }
        Debug.Log($"현재 플레이어 퀘스트 ID: {playerQuestID}, 이름: {playerQuestTitle}, 진행될 스토리 ID: {playerQuestStoryID}," +
                    $"처치할 몬스터: {currentQuestEnemyNPC}, 처치할 몬스터 수: {currentQuestEnemyCount}");
        storySystem.QuestStory(playerQuestStoryID);
        //questText.text = playerquestName;
    }


    private void Update()
    {
        //if(playerquestPart != null)
        //    cuttentQuestName.text = $"{playerquestPart}";

        if (!finishQuest)
        {
            SuccessChack();

            if (Input.GetKeyDown(KeyCode.N))
            {
                SuccessQuest();
                Debug.Log("치트 사용으로 현재 퀘스트 완료 처리");
            }
        }
    }

    void ShowQuest(QuestData quest)
    {
        playerQuestID = quest.Quest_ID;
        playerQuestTitle = quest.Quest_Title;
        playerQuestType = quest.Quest_Type;
        playerQuestStoryID = quest.Quest_StoryID;
        currentQuestEnemyNPC = quest.Quest_EnemyNPC;
        currentQuestEnemyCount = quest.Qeust_EnemyCount;
        playerQuestLocation_Asset = quest.Location_Asset;
        currentBGMAeest = quest.BGM_Asset;
        currentRewards = quest.Rewards;

    }

    void SuccessChack()
    {
        switch (playerQuestType)
        {
            case "Story":
                if(storySystem.isFinishStory == true)
                {
                    SuccessQuest();
                }
                break;

            case "move":
                SuccessQuest();
                break;

            case "Battle":
                if(currnet_EnmeyDieCount == currentQuestEnemyCount)
                {
                    SuccessQuest();
                    currnet_EnmeyDieCount = 0;
                }
                break;
            case "Finish":
                finishQuest = true;
                SuccessQuest();
                Debug.Log($"{playerQuestTitle} 종료");
                break;

        }
    }
    void SuccessQuest()
    {
        if (isProcessingQuest) return;

        if (!finishQuest)
        {
            currentQuestIndex += 1;
            Debug.Log($"다음 퀘스트 인덱스 : {currentQuestIndex}");

            if (currentQuestIndex >= questData.quests.Count)
            {
                Debug.Log("챕터 완료");
                finishQuest = true;
                storySystem.StoryUI.gameObject.SetActive(false);
                return;
            }

            QuestData nextQuest = questData.quests[currentQuestIndex];
            ShowQuest(nextQuest);
            //questText.text = playerquestName;
            currnet_EnmeyDieCount = 0;
            Debug.Log($"현재 플레이어 퀘스트 ID: {playerQuestID}, 이름: {playerQuestTitle}, 진행될 스토리 ID: {playerQuestStoryID}," +
                $"처치할 몬스터: {currentQuestEnemyNPC}, 처치할 몬스터 수: {currentQuestEnemyCount}");
            storySystem.QuestStory(playerQuestStoryID);
            playerquest_Is_success = false;
            finishQuest = false;
            Invoke("ResetQuestFlag", 0.1f);
        }
        else
        {
            ChangeQuest();
        }
    }

    void ResetQuestFlag() { isProcessingQuest = false; }

    void ChangeQuest()
    {
        currentQuestAndSotorys += 1;

        int nextIndex = currentQuestAndSotorys;
        if (_questAndStoryDatabase != null && nextIndex < _questAndStoryDatabase.questDataSOs.Count)
        {
            StoryDataSO nextStory = _questAndStoryDatabase.storyDataSOs[currentQuestAndSotorys];
            if (nextStory != null)
            {
                storySystem.StoryDataSO = nextStory;
                storySystem.current_StoryCount = 0;
                Debug.Log($"{nextStory.name}으로 스토리 변경 완료");
                storySystem.isFinishStory = false;
            }

            QuestDataSO nextQuest = _questAndStoryDatabase.questDataSOs[currentQuestAndSotorys];
            if (nextQuest != null)
            {
                questData = nextQuest;
                QuestData ChangeFirstQuest = questData.quests[0];
                ShowQuest(ChangeFirstQuest);
                currentQuestIndex = 0;
                Debug.Log($"{nextQuest.name} 으로 쳅터 변경 완료");
                finishQuest = false;
                Debug.Log($"현재 플레이어 퀘스트 ID: {playerQuestID}, 이름: {playerQuestTitle}, 진행될 스토리 ID: {playerQuestStoryID}," +
                $"처치할 몬스터: {currentQuestEnemyNPC}, 처치할 몬스터 수: {currentQuestEnemyCount}");
                storySystem.QuestStory(playerQuestStoryID);
                //questText.text = playerquestName;
            }
        }
        else
        {
            finishQuest = true;
        }
    }
}

