
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSystem : MonoBehaviour
{
    public EnemySystem enemySystem;
    public EnemyHelthSystem enemyHelthSystem;
    public BossPatternSystem bossPatternSystem;
    public BossPatternSO bossPatternData;

    private void Start()
    {
        enemySystem = FindAnyObjectByType<EnemySystem>();
        enemyHelthSystem = FindAnyObjectByType<EnemyHelthSystem>();
        bossPatternSystem = FindAnyObjectByType<BossPatternSystem>();

        StartCoroutine(BossRandomPatternTime());
    }

    IEnumerator BossRandomPatternTime()
    {
        yield return new WaitForSeconds(3f);
        BossRandomPattern();
    }

    public void BossRandomPattern()
    {
        Debug.Log("보스 패턴 뽑기");

        int randomIndex = Random.Range(0, bossPatternData.bossPatternDataList.Count);
        BossPatternData currentPattern = bossPatternData.bossPatternDataList[randomIndex];

        if(enemyHelthSystem.currentBossHelth <= enemyHelthSystem.minBossHelth)
        {
            while (true)
            {
                if (currentPattern.currentPage == PatternPage.Page02 || currentPattern.currentPage == PatternPage.EveryPage)
                    break;

                return;
            }
        }
        else
        {
            SelectSkill(enemySystem.enemyName, currentPattern.BossPatternName, currentPattern.currentPage);
        }
    }

    void SelectSkill(string BossName, string Pattern, PatternPage page)
    {
        switch(BossName, Pattern, page)
        {
            case ("보스", "레이저", PatternPage.EveryPage):
                StartCoroutine(bossPatternSystem.LaserAttack());
                break;
            case ("보스", "전기", PatternPage.Page01):
                StartCoroutine(bossPatternSystem.BossPattern02());
                break;
            case ("보스", "검은 물체", PatternPage.Page01):
                StartCoroutine(bossPatternSystem.CreateObjectPattern());
                break;
            case ("보스", "오브젝트 파괴", PatternPage.EveryPage):
                StartCoroutine(bossPatternSystem.DestoryObjectPattern());
                break;
            default:
                break;
        }
    }
}
