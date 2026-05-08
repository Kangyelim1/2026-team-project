using System.Collections;
using UnityEngine;

public class EnemyAttackSystem : MonoBehaviour
{
    [Header("적 이동 관련")]
    public float attackRange = 1.0f;
    public Vector3 enemyStartPosition;
    public GameObject TargetPlayer;

    public EnemySystem enemySystem;
    public NewBattleManager _battleManager;

    private void Start()
    {
        enemySystem = GetComponent<EnemySystem>();
        _battleManager = Object.FindAnyObjectByType<NewBattleManager>();

        StartCoroutine(StartBattle());
    }

    IEnumerator StartBattle()
    {
        while (Vector3.Distance(transform.position, _battleManager._playerBattleSystem.EnemyOpPoint.transform.position) > attackRange)
        {
            Vector3 direction = (_battleManager._playerBattleSystem.EnemyOpPoint.transform.position - transform.position).normalized;
            transform.position += direction * enemySystem.Enemy_Speed * Time.deltaTime;

            yield return null;

        }

        yield return new WaitForSeconds(0.7f);
        enemyStartPosition = transform.position;
    }

    public void AutoSelectPlayer()
    {
        if (_battleManager == null)
        {
            Debug.LogError("BattleManager를 찾지 못했습니다.");
            return;
        }

        if (_battleManager.spawnedPlayer == null)
        {
            Debug.Log("공격할 플레이어가 없습니다.");
            _battleManager.EndGame(false);
            return;
        }

        TargetPlayer = _battleManager.spawnedPlayer;

        PlayerSystem playerSystem = TargetPlayer.GetComponent<PlayerSystem>();

        if (playerSystem == null)
        {
            Debug.LogError("타겟 플레이어에 PlayerSystem이 없습니다.");
            return;
        }

        Debug.Log($"타겟 플레이어 지정 완료: {playerSystem.player_Name}");

        StartCoroutine(EnemyAttack());
    }

    private IEnumerator EnemyAttack()
    {
        Debug.Log("플레이어 위치로 이동");

        while (TargetPlayer != null &&
               Vector3.Distance(transform.position, TargetPlayer.transform.position) > attackRange)
        {
            Vector3 direction = (TargetPlayer.transform.position - transform.position).normalized;
            transform.position += direction * enemySystem.Enemy_Speed * Time.deltaTime;

            yield return null;
        }

        if (TargetPlayer == null)
        {
            Debug.Log("플레이어가 사라져 공격을 취소합니다.");
            _battleManager.EndEnemyTurn();
            yield break;
        }

        Debug.Log("플레이어 공격");

        PlayerSystem player = TargetPlayer.GetComponent<PlayerSystem>();

        if (player != null)
        {
            IsAttack(player);
        }

        yield return new WaitForSeconds(0.2f);

        Debug.Log("원위치");

        while (Vector3.Distance(transform.position, enemyStartPosition) > 0.1f)
        {
            Vector3 backDirection = (enemyStartPosition - transform.position).normalized;
            transform.position += backDirection * enemySystem.Enemy_Speed * Time.deltaTime;

            yield return null;
        }

        transform.position = enemyStartPosition;

        Debug.Log("복귀 완료");

        yield return new WaitForSeconds(1f);

        _battleManager.EndEnemyTurn();
    }

    private void IsAttack(PlayerSystem target)
    {
        if (target.player_CurrentHelth > 0)
        {
            target.player_CurrentHelth -= enemySystem.Enemy_Damage;

            Debug.Log(
                $"플레이어 체력 {enemySystem.Enemy_Damage} 만큼 감소, 현재 체력: {target.player_CurrentHelth}"
            );

            if (target.player_CurrentHelth <= 0)
            {
                Debug.Log("플레이어 사망");
                Destroy(target.gameObject);
                _battleManager.spawnedPlayer = null;
            }
        }
    }
}