using Cainos.LucidEditor;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class FageBossSystem : MonoBehaviour
{
    public EnemySystem enemySystem;
    public EnemyHelthSystem enemyHelthSystem;
    public BossPatternSystem bossPatternSystem;
    public BossPatternSO bossPatternData;

    [Header("현실과 꿈 전환 관련")]
    public int patternCount;
    public int changeFageCount = 5;
    public bool isDreamFage = false;

    [Header("현실 관련 오브젝트")]
    public GameObject RealityBackGround;
    public GameObject RealityGround;

    [Header("꿈 관련 오브젝트")]
    public GameObject DreamBackGround;
    public GameObject DreamGround;

    private void Start()
    {
        enemySystem = FindAnyObjectByType<EnemySystem>();
        enemyHelthSystem = FindAnyObjectByType<EnemyHelthSystem>();
        bossPatternSystem = FindAnyObjectByType<BossPatternSystem>();

        SetFage(false);
        StartCoroutine(Opening());
    }
    IEnumerator Opening()
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

        SelectSkill(enemySystem.enemyName, currentPattern.BossPatternName, currentPattern.currentPage);
        AddPatternCount();
    }

    void SelectSkill(string BossName, string Pattern, PatternPage page)
    {
        switch (BossName, Pattern, page)
        {

            default:
                BossRandomPattern();
                break;
        }
    }

    public void AddPatternCount()
    {
        patternCount++;
        Debug.Log("현재 패턴 카운트 : " + patternCount);

        if (patternCount >= changeFageCount)
        {
            ChangeFage();
        }
    }

    public void ChangeFage()
    {
        patternCount = 0;

        isDreamFage = !isDreamFage;

        SetFage(isDreamFage);

        if (isDreamFage)
        {
            Debug.Log("꿈 페이지로 전환");
        }
        else
        {
            Debug.Log("현실 페이지로 전환");
        }
    }

    public void SetFage(bool dream)
    {
        RealityBackGround.SetActive(!dream);
        RealityGround.SetActive(!dream);

        DreamBackGround.SetActive(dream);
        DreamGround.SetActive(dream);
    }



}
