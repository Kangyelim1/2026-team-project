using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    [Header("이벤트 패널")]
    public GameObject eventPanel;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (eventPanel != null)
            eventPanel.SetActive(false);
    }

    public void ShowPriestEvent()
    {
        if (eventPanel != null)
            eventPanel.SetActive(true);

        Debug.Log("신녀 이벤트 시작");
    }

    public void HealPlayer()
    {
        int healAmount = 30;

        if (BattleManager.Instance != null && BattleManager.Instance.player != null)
        {
            BattleManager.Instance.player.currentHP += healAmount;

            if (BattleManager.Instance.player.currentHP > BattleManager.Instance.player.maxHP)
                BattleManager.Instance.player.currentHP = BattleManager.Instance.player.maxHP;

            Debug.Log($"체력 회복! +{healAmount}");
        }

        EndEvent();
    }

    public void UpgradeSkill()
    {
        Debug.Log("스킬 강화!");
        EndEvent();
    }

    void EndEvent()
    {
        if (eventPanel != null)
            eventPanel.SetActive(false);

        Debug.Log("이벤트 끝 → 스토리 씬으로 이동!");

        // ★ 버그 수정: 이벤트가 끝나면 전투를 다시 세팅하는 게 아니라,
        // 스테이지를 올리고 '스토리 씬(StoryScene)'으로 확실하게 보내줍니다!
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.GoToNextStoryAfterBoss();
        }
    }
}