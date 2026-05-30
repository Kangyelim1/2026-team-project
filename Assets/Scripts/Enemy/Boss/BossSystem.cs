
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSystem : MonoBehaviour
{
    public EnemySystem enemySystem;
    public EnemyHelthSystem enemyHelthSystem;
    public BossPatternSystem bossPatternSystem;

    public List<string> PatternList = new List<string>();

    private void Start()
    {
        enemySystem = FindAnyObjectByType<EnemySystem>();
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
        Debug.Log("패턴 뽑기");

        int randomIndex = Random.Range(0, PatternList.Count);
        string currentPattern = PatternList[randomIndex];

        SelectSkill(enemySystem.enemyName, currentPattern);
    }

    void SelectSkill(string BossName, string Pattern)
    {
        switch(BossName)
        {
            case "보스" when Pattern == "레이져":
                StartCoroutine(bossPatternSystem.LaserAttack());
                break;
            case "보스" when Pattern == "전기":
                StartCoroutine(bossPatternSystem.BossPattern02());
                break;
            case "보스" when Pattern == "검은 물체":
                StartCoroutine(bossPatternSystem.CreateObjectPattern());
                break;
            case "보스" when Pattern == "오브젝트 파괴":
                StartCoroutine(bossPatternSystem.DestoryObjectPattern());
                break;
            default:
                break;
        }
    }
}
