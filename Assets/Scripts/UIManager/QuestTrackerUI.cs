using UnityEngine;
using TMPro;

public class QuestTrackerUI : MonoBehaviour
{
    [Header("UI 연결")]
    public GameObject trackerPanel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI progressText;

    void Update()
    {
        if (QuestSystem.instance == null) return;

        if (QuestSystem.instance.finishQuest || string.IsNullOrEmpty(QuestSystem.instance.playerQuestTitle))
        {
            if (trackerPanel.activeSelf) trackerPanel.SetActive(false);
            return;
        }

        if (!trackerPanel.activeSelf) trackerPanel.SetActive(true);
        titleText.text = QuestSystem.instance.playerQuestTitle;

        if (QuestSystem.instance.playerQuestType == "Battle")
        {
            progressText.text = $"{QuestSystem.instance.currentQuestEnemyNPC} 처치 : {QuestSystem.instance.currnet_EnmeyDieCount} / {QuestSystem.instance.currentQuestEnemyCount}";
        }
        else if (QuestSystem.instance.playerQuestType == "story")
        {
            progressText.text = "NPC와 대화하여 스토리 진행";
        }
        else if (QuestSystem.instance.playerQuestType == "move")
        {
            progressText.text = "목표 지점으로 이동하기";
        }
        else
        {
            progressText.text = "진행 중...";
        }
    }
}