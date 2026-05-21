using System.Collections;
using UnityEngine;

public class BossEnemyPatternSystem : MonoBehaviour
{
    public EnemyBattleSystem _enemyBattleSystem;

    private void Start()
    {
        _enemyBattleSystem = Object.FindAnyObjectByType<EnemyBattleSystem>();
    }

    public IEnumerator Wonnim01()
    {
        Debug.Log("원님 패턴 01 진행");
        yield return new WaitForSeconds(1f);
        _enemyBattleSystem._battleManager.EndEnemyTurn();
    }

    public IEnumerator Wonnim02()
    {
        Debug.Log("원님 패턴 02 진행");
        yield return new WaitForSeconds(1f);
        _enemyBattleSystem._battleManager.EndEnemyTurn();
    }

    public IEnumerator Wonnim03()
    {
        Debug.Log("원님 패턴 03 진행");
        yield return new WaitForSeconds(1f);
        _enemyBattleSystem._battleManager.EndEnemyTurn();
    }
}
