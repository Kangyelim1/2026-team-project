using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuildUIManager : MonoBehaviour
{
    // 싱글톤
    public static GuildUIManager Instance;

    [Header("패널")]
    public GameObject questPanel;
    public GameObject panel1;
    public GameObject panel2;

    [Header("Panel2 UI")]
    public TMP_Text questTitleText;
    public TMP_Text questDescriptionText;
    public Image rewardImage1;
    public Image rewardImage2;


    [Header("퀘스트 버튼 자동 생성")]
    public Transform contentParent; // ScrollView Content
    public GameObject questButtonPrefab;

    public QuestDataSO questDataSO;

    // 현재 상호작용 중인 길드
    private Interactable currentGuild;

    // 플레이어
    private Transform player;

    // 길드 UI 자동 종료 거리
    public float closeDistance = 3f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Player 찾기
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // 시작 시 전부 끄기
        questPanel.SetActive(false);
        panel1.SetActive(false);
        panel2.SetActive(false);

        // 퀘스트 버튼 생성
        //CreateQuestButtons();
    }

    private void Update()
    {
        // 길드와 멀어지면 UI 닫기
        if (currentGuild != null)
        {
            float distance = Vector2.Distance(
                player.position,
                currentGuild.transform.position
            );

            if (distance > closeDistance)
            {
                CloseGuildUI();
            }
        }
    }

    // 길드 UI 열기
    public void OpenGuildUI(Interactable guild)
    {
        currentGuild = guild;

        questPanel.SetActive(true);

        panel1.SetActive(true);
        panel2.SetActive(false);

        Debug.Log("길드 UI 열림");
    }

    // 길드 UI 닫기
    public void CloseGuildUI()
    {
        currentGuild = null;

        questPanel.SetActive(false);

        Debug.Log("길드 UI 닫힘");
    }

    // 퀘스트 열기
    public void OpenQuestDetail(QuestData quest)
    {
        panel1.SetActive(false);
        panel2.SetActive(true);

        // 퀘스트 정보 표시
        questTitleText.text = quest.Quest_Title;
        questDescriptionText.text = quest.Quest_Description;
        rewardImage1.sprite = quest.rewardIcon1;
        rewardImage2.sprite = quest.rewardIcon2;

        Debug.Log("퀘스트 상세창 열림");
    }

    // 퀘스트창 닫기
    public void BackToQuestList()
    {
        panel1.SetActive(true);
        panel2.SetActive(false);
    }

    void CreateQuestButtons()
    {
        Debug.Log("퀘스트 버튼 생성 시작");
        foreach (QuestData quest in questDataSO.quests)
        {
            Debug.Log("생성할 퀘스트: " + quest.Quest_Title);

            // 버튼 생성
            GameObject buttonObj = Instantiate(
                questButtonPrefab,
                contentParent
            );

            // QuestButton 스크립트 가져오기
            QuestButton button = buttonObj.GetComponent<QuestButton>();

            // 퀘스트 데이터 연결
            button.Setup(quest);
        }
    }
}