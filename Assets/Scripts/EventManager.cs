using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        else
        {
            Debug.LogWarning("플레이어가 없어 회복할 수 없습니다.");
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

        if (BattleManager.Instance != null)
            BattleManager.Instance.EndEvent();
    }
}