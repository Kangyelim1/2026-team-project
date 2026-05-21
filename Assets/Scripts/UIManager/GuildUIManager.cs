using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuildUIManager : MonoBehaviour
{
    // 싱글톤
    public static GuildUIManager Instance;

    [Header("패널")]
    public GameObject questPanel;
    public GameObject panel1; // 목록 창
    public GameObject panel2; // 상세 창

    [Header("Panel2 UI (상세 창)")]
    public TextMeshProUGUI questTitleText;
    public TextMeshProUGUI questDescriptionText;
    public Image rewardImage1;
    public Image rewardImage2;
    public Button acceptButton; // 수락 버튼

    [Header("퀘스트 버튼 자동 생성")]
    public Transform contentParent; // ScrollView Content
    public GameObject questButtonPrefab;
    public QuestList questList;

    // 현재 상호작용 중인 길드와 플레이어
    private Interactable currentGuild;
    private Transform player;
    public float closeDistance = 3f;

    // 선택된 퀘스트 기억용
    private QuestData currentSelectedQuest;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        // Player 태그를 가진 오브젝트 찾기
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        questList = Object.FindAnyObjectByType<QuestList>();

        // 시작 시 패널 모두 끄기
        if (questPanel != null) questPanel.SetActive(false);
        if (panel1 != null) panel1.SetActive(false);
        if (panel2 != null) panel2.SetActive(false);

        // 수락 버튼 기능 연결
        if (acceptButton != null)
        {
            acceptButton.onClick.RemoveAllListeners();
            acceptButton.onClick.AddListener(OnClickAcceptQuest);
        }

        // 퀘스트 버튼 생성 함수 호출
        CreateQuestButtons();
    }

    private void Update()
    {
        // 길드(NPC)와 거리가 멀어지면 UI 자동 닫기
        if (currentGuild != null && player != null)
        {
            float distance = Vector2.Distance(player.position, currentGuild.transform.position);

            if (distance > closeDistance)
            {
                CloseGuildUI();
            }
        }
    }

    // 길드 UI 열기 (상호작용 시 호출)
    public void OpenGuildUI(Interactable guild)
    {
        currentGuild = guild;

        if (questPanel != null) questPanel.SetActive(true);
        if (panel1 != null) panel1.SetActive(true);
        if (panel2 != null) panel2.SetActive(false);

        Debug.Log("길드 UI 열림");
    }

    // 길드 UI 닫기
    public void CloseGuildUI()
    {
        currentGuild = null;

        if (questPanel != null) questPanel.SetActive(false);

        Debug.Log("길드 UI 닫힘");
    }

    // 퀘스트 열기 (목록에서 버튼 클릭 시 호출됨)
    public void OpenQuestDetail(QuestData quest)
    {
        currentSelectedQuest = quest;

        if (panel1 != null) panel1.SetActive(false);
        if (panel2 != null) panel2.SetActive(true);

        // 퀘스트 정보 텍스트/이미지 갱신
        if (questTitleText != null) questTitleText.text = quest.Quest_Title;
        if (questDescriptionText != null) questDescriptionText.text = quest.Quest_Description;
        if (rewardImage1 != null) rewardImage1.sprite = quest.rewardIcon1;
        if (rewardImage2 != null) rewardImage2.sprite = quest.rewardIcon2;

        Debug.Log("퀘스트 상세창 열림");
    }

    // ★ 수락 버튼을 눌렀을 때 실행되는 함수
    public void OnClickAcceptQuest()
    {
        if (currentSelectedQuest == null) return;

        Debug.Log($"{currentSelectedQuest.Quest_Title} 수락됨!");

        // QuestSystem에 수락한 퀘스트 넘기기
        if (QuestSystem.instance != null)
        {
            QuestSystem.instance.AcceptNewQuest(currentSelectedQuest);
        }

        // 수락 후 퀘스트 목록 창으로 돌아가기
        BackToQuestList();
    }

    // 퀘스트창 닫기 (목록으로 돌아가기 버튼용)
    public void BackToQuestList()
    {
        if (panel1 != null) panel1.SetActive(true);
        if (panel2 != null) panel2.SetActive(false);
    }

    // 퀘스트 목록 버튼 자동 생성
    void CreateQuestButtons()
    {
        if (questList == null || questList.questList == null) return;

        Debug.Log("퀘스트 버튼 생성 시작");
        foreach (QuestDataSO questSO in questList.questList)
        {
            if (questSO != null && questSO.quests.Count > 0)
            {
                // 프리팹 생성
                GameObject buttonObj = Instantiate(questButtonPrefab, contentParent);
                QuestButton button = buttonObj.GetComponent<QuestButton>();

                if (button != null)
                {
                    QuestData currentQuest = questSO.quests[0];
                    button.Setup(currentQuest);
                }
            }
        }
    }
}