using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    //  이벤트 패널
    public GameObject eventPanel;

    void Start()
    {
        eventPanel.SetActive(false); // 시작할 때 패널 끄기
    }

    void Awake()
    {
        Instance = this;
    }

    // 신녀 이벤트
    public void ShowPriestEvent()
    {
        eventPanel.SetActive(true); //  패널 켜기
    }

    // 체력 회복
    public void HealPlayer()
    {
        int healAmount = 30;

        BattleManager.Instance.player.currentHP += healAmount;

        Debug.Log("체력 회복!");

        EndEvent();
    }

    // 스킬 강화
    public void UpgradeSkill()
    {
        Debug.Log("스킬 강화!");

        EndEvent();
    }

    void EndEvent()
    {
        eventPanel.SetActive(false); //  패널 끄기
        BattleManager.Instance.EndEvent();
    }
}
