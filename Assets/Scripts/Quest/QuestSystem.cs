using UnityEngine;
using TMPro;

public class QuestSystem : MonoBehaviour
{
    public static QuestSystem instance;
    public QuestDataSO questData;
    public StorySystem storySystem;
    public PlayerInteraction _playerInteraction;

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
    public string currentDestination;

    public bool finishQuest;
    public bool victory;

    public int currnet_EnmeyDieCount;

    public GameObject QuestCanavarse;

    [Header("플레이어 정보")]
    public int playerLevel = 1;
    public int playerExperience = 0;

    [Header("퀘스트 UI")]
    public TextMeshProUGUI cuttentQuestName;
    public TextMeshProUGUI questText;
    private int currentQuestIndex = 0;

    public QuestAndStoryDatabase _questAndStoryDatabase;

    public string currentEnemy;

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

        _playerInteraction = FindAnyObjectByType<PlayerInteraction>();
    }

    private void Start()
    {
        storySystem = GetComponent<StorySystem>();


        StartQuest();

        if (questData == null) return;

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
        finishQuest = false;
        questText.text = playerQuestTitle;
    }


    private void Update()
    {
      
        cuttentQuestName.text = $"{questData.name}";

        if (!finishQuest)
        {
            SuccessCheck();

            // 치트키: N을 누르면 현재 퀘스트 즉시 완료
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
        currentDestination = quest.Destination;

    }

    void SuccessCheck()
    {
        switch (playerQuestType)
        {
            case "story":
                if(storySystem.isFinishStory == true) SuccessQuest();
                break;

            case "move":
                if (currentEnemy == currentDestination)
                SuccessQuest();
                break;

            case "Battle":
                if(currnet_EnmeyDieCount == currentQuestEnemyCount)
                {
                    SuccessQuest();
                    currnet_EnmeyDieCount = 0;
                }
                break;
            case "finish":
                finishQuest = true;
                SuccessQuest();
                Debug.Log($"{playerQuestTitle} 종료");
                break;

        }
    }
    void SuccessQuest()
    {
        Debug.Log("퀘스트 완료 호출");
        if (isProcessingQuest) return;
        isProcessingQuest = true; // 중복 호출 방지 플래그 추가!

        if (!finishQuest)
        {
            currentQuestIndex += 1;
            Debug.Log($"다음 퀘스트 인덱스 : {currentQuestIndex}");

            if (currentQuestIndex >= questData.quests.Count)
            {
                Debug.Log("챕터 완료");
                finishQuest = true;
                storySystem.StoryUI.gameObject.SetActive(false);
                isProcessingQuest = false;
                return;
            }

            // 다음 퀘스트 보여주기
            QuestData nextQuest = questData.quests[currentQuestIndex];
            ShowQuest(nextQuest);

            questText.text = playerQuestTitle;
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

    void ResetQuestFlag()
    {
        isProcessingQuest = false;
    }

    // 처음 게임 켤 때 또는 챕터 시작 시 부르는 함수
    void StartQuest()
    {
        int firstQuest = currentQuestAndSotorys;

        if (_questAndStoryDatabase != null)
        {
            StoryDataSO StartStory = _questAndStoryDatabase.storyDataSOs[currentQuestAndSotorys];

            if (StartStory != null)
            {
                storySystem.StoryDataSO = StartStory;
                storySystem.current_StoryCount = 0;
                Debug.Log($"{StartStory.name} 스토리 적용");
                storySystem.isFinishStory = false;
            }
            else { Debug.LogWarning("활당된 퀘스트 미존재");}

            QuestDataSO StartQuestData = _questAndStoryDatabase.questDataSOs[currentQuestAndSotorys];
            if (StartQuestData != null)
            {
                questData = StartQuestData;
                QuestData ChangeFirstQuest = questData.quests[0];
                ShowQuest(ChangeFirstQuest);
                currentQuestIndex = 0;
                Debug.Log($"{StartQuestData.name} 으로 챕터 변경 완료");
                finishQuest = false;

                Debug.Log($"현재 플레이어 퀘스트 ID: {playerQuestID}, 이름: {playerQuestTitle}, 진행될 스토리 ID: {playerQuestStoryID}," +
                $"처치할 몬스터: {currentQuestEnemyNPC}, 처치할 몬스터 수: {currentQuestEnemyCount}");

                storySystem.QuestStory(playerQuestStoryID);
                questText.text = playerQuestTitle;
            }
            else { Debug.LogWarning("활당된 퀘스트 미존재"); }
        }

    }

    void ChangeQuest()
    {
        currentQuestAndSotorys ++;

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
            else { Debug.LogWarning("할당된 스토리 미존재"); }

            QuestDataSO nextQuest = _questAndStoryDatabase.questDataSOs[currentQuestAndSotorys];
            if (nextQuest != null)
            {
                questData = nextQuest;
                QuestData ChangeFirstQuest = questData.quests[0];
                ShowQuest(ChangeFirstQuest);
                currentQuestIndex = 0;
                Debug.Log($"{nextQuest.name} 으로 챕터 변경 완료");
                finishQuest = false;

                Debug.Log($"현재 플레이어 퀘스트 ID: {playerQuestID}, 이름: {playerQuestTitle}, 진행될 스토리 ID: {playerQuestStoryID}," +
                $"처치할 몬스터: {currentQuestEnemyNPC}, 처치할 몬스터 수: {currentQuestEnemyCount}");

                storySystem.QuestStory(playerQuestStoryID);
                questText.text = playerQuestTitle;
            }
            else { Debug.LogWarning("할당된 퀘스트 미존재"); }

            isProcessingQuest = false; // 챕터 넘어가면 플래그 해제
        }
    }
    // ============================================
    // ★ 추가된 부분: 길드 UI에서 퀘스트를 수락했을 때 호출됨
    // ============================================
    public void AcceptNewQuest(QuestData newQuest)
    {
        ShowQuest(newQuest);

        currnet_EnmeyDieCount = 0;
        finishQuest = false;
        playerquest_Is_success = false;

        if (_questAndStoryDatabase != null && _questAndStoryDatabase.storyDataSOs.Count > currentQuestAndSotorys)
        {
            storySystem.StoryDataSO = _questAndStoryDatabase.storyDataSOs[currentQuestAndSotorys];
        }

        if (playerQuestStoryID != 0 && storySystem != null)
        {
            if (storySystem.StoryDataSO != null)
            {
                storySystem.QuestStory(playerQuestStoryID);
            }
            else
            {
                Debug.LogWarning(" 스토리를 재생할 수 없습니다!");
            }
        }

        Debug.Log($"[퀘스트 수락 완료] 제목: {playerQuestTitle}, 타입: {playerQuestType}, 목표: {currentQuestEnemyNPC} {currentQuestEnemyCount}마리");
    }
}


