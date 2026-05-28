
using System.Collections.Generic;
using UnityEngine;

public class BossSystem : MonoBehaviour
{
    public EnemySystem enemySystem;
    public BossPatternSystem bossPatternSystem;

    public List<string> PatternList = new List<string>();

    private void Start()
    {
        enemySystem = FindAnyObjectByType<EnemySystem>();
        bossPatternSystem = FindAnyObjectByType<BossPatternSystem>();

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
            case "보스" when Pattern == "패턴01":
                StartCoroutine(bossPatternSystem.LaserAttack());
                break;
            case "보스" when Pattern == "패턴02":
                StartCoroutine(bossPatternSystem.BossPattern02());
                break;
            default:
                break;
        }
    }
}
