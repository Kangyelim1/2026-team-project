using TMPro;
using UnityEngine;

public class QuestButton : MonoBehaviour
{
    // 현재 버튼이 가진 퀘스트 데이터
    private QuestData currentQuest;

    // 버튼 텍스트
    public TextMeshProUGUI questTitleText;

    // 퀘스트 설정
    public void Setup(QuestData quest)
    {
        currentQuest = quest;

        //버튼 이름 표시
        questTitleText.text = $"{quest.Quest_ID}";
    }

    // 버튼 클릭 시
    public void OpenQuest()
    {
        GuildUIManager.Instance.OpenQuestDetail(currentQuest);
    }
}