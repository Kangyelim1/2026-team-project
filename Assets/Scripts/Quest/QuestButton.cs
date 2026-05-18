using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestButton : MonoBehaviour
{
    public Button myButton;

    private void Start()
    {
        myButton.onClick.AddListener(OpenQuest);
    }

    // 현재 버튼이 가진 퀘스트 데이터
    private QuestData currentQuest;

    // 버튼 텍스트
    public TextMeshProUGUI questTitleText;

    // 퀘스트 설정
    public void Setup(QuestData quest)
    {
        currentQuest = quest;

        //버튼 이름 표시
        questTitleText.text = $"{quest.Quest_Title}";
    }

    // 버튼 클릭 시
    public void OpenQuest()
    {
        Debug.Log("클릭");
        GuildUIManager.Instance.OpenQuestDetail(currentQuest);
    }
}