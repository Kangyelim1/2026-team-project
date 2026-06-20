
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSystem : MonoBehaviour
{
    public EnemySystem enemySystem;
    public EnemyHelthSystem enemyHelthSystem;
    public BossPatternSystem bossPatternSystem;
    public BossPatternSO bossPatternData;

    public GameObject page01Object;
    public GameObject page02Object;
    public Animator bossAnimator;

    public bool isPage02;

    private void Start()
    {
        enemySystem = FindAnyObjectByType<EnemySystem>();
        enemyHelthSystem = FindAnyObjectByType<EnemyHelthSystem>();
        bossPatternSystem = FindAnyObjectByType<BossPatternSystem>();
        bossAnimator = GetComponentInChildren<Animator>(true);
        StartCoroutine(BossRandomPatternTime());
    }

    IEnumerator BossRandomPatternTime()
    {
        yield return new WaitForSeconds(1.5f);
        BossRandomPattern();
    }

    public void BossRandomPattern()
    {
        if (enemySystem.playerSystem == null) return;
        Debug.Log("보스 패턴 뽑기");

        int randomIndex = Random.Range(0, bossPatternData.bossPatternDataList.Count);
        BossPatternData currentPattern = bossPatternData.bossPatternDataList[randomIndex];

        if (enemyHelthSystem.currentBossHelth <= enemyHelthSystem.minBossHelth)
        {
            ChanagePage();
            if (currentPattern.currentPage == PatternPage.Page02 || currentPattern.currentPage == PatternPage.EveryPage)
            {
                Debug.Log(currentPattern.BossPatternName);
                SelectSkill(enemySystem.enemyName, currentPattern.BossPatternName, currentPattern.currentPage);
            }
            else
            {
                BossRandomPattern();
            }
        }
        else
        {
            if (currentPattern.currentPage == PatternPage.Page01 || currentPattern.currentPage == PatternPage.EveryPage)
            {
                Debug.Log(currentPattern.BossPatternName);
                SelectSkill(enemySystem.enemyName, currentPattern.BossPatternName, currentPattern.currentPage);
            }
            else
            {
                BossRandomPattern();
            }
        }
    }

    void SelectSkill(string BossName, string Pattern, PatternPage page)
    {
        switch(BossName, Pattern, page)
        {
            case ("보스", "레이저", PatternPage.EveryPage):
                Debug.Log("레이저 패턴");
                StartCoroutine(bossPatternSystem.LaserAttack());
                break;
            case ("보스", "전기", PatternPage.Page01):
                Debug.Log("전기 패턴");
                StartCoroutine(bossPatternSystem.BossPattern02());
                break;
            case ("보스", "검은 물체", PatternPage.Page01):
                Debug.Log("검은 물체 패턴");
                StartCoroutine(bossPatternSystem.CreateObjectPattern());
                break;
            case ("보스", "오브젝트 파괴", PatternPage.EveryPage):
                Debug.Log("오브젝트 파괴 패턴");
                StartCoroutine(bossPatternSystem.DestoryObjectPattern());
                break;
            case ("보스", "미사일", PatternPage.Page02):
                Debug.Log("미사일 패턴");          
                StartCoroutine(bossPatternSystem.Missile());
                break;
            case ("보스", "돌진", PatternPage.Page02):
                StartCoroutine(bossPatternSystem.Rush());
                Debug.Log("돌진 패턴");
                break;
            case ("보스", "드론", PatternPage.Page02):
                StartCoroutine(bossPatternSystem.SuicideDrone());
                Debug.Log("자폭드론 소환 패턴");
                break;
            default:
                BossRandomPattern();
                break;
        }
    }

    void ChanagePage()
    {
        if (isPage02) return;
        page01Object.SetActive(false);
        page02Object.SetActive(true);

        isPage02 = true;
    }
}
