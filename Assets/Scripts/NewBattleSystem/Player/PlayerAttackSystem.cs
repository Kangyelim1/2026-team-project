using JetBrains.Annotations;
using System.Collections;
using UnityEngine;

public class PlayerAttackSystem : MonoBehaviour
{
    public PlayerSystem playerSystem;
    public PlayerBattleSystem _playerBattleSystem;
    
    public int currentDamage;
    public Vector3 playerPoition;
    public float attackRange = 1.0f;

    private void Start()
    {
        playerSystem = Object.FindAnyObjectByType<PlayerSystem>();
        _playerBattleSystem = Object.FindAnyObjectByType<PlayerBattleSystem>();

        currentDamage = playerSystem.player_Damage;
        playerPoition = transform.position;
    }

    public IEnumerator isAttack()
    {
        Debug.Log("적 위치로 이동");
        while (Vector3.Distance(transform.position, _playerBattleSystem.TargetEnemy.transform.position) > attackRange)
        {
            Vector3 direction = (_playerBattleSystem.TargetEnemy.transform.position - transform.position).normalized;
            transform.position += direction * playerSystem.player_Speed * Time.deltaTime;

            yield return null;
        }

        Debug.Log("적 공격");
        //playerSystem.playerAnimator.SetBool("isAttack", true);
        yield return new WaitForSeconds(0.2f);
        //playerSystem.playerAnimator.SetBool("isAttack", false);

        _playerBattleSystem.TargetEnemy.TryGetComponent<EnemySystem>(out EnemySystem enemy);
        IsAttack(enemy);

        Debug.Log("원위치");

        while (Vector3.Distance(transform.position, playerPoition) > 0.1f)
        {
            Vector3 backDirection = (playerPoition - transform.position).normalized;
            transform.position += backDirection * playerSystem.player_Speed * Time.deltaTime;

            yield return null;
        }
        transform.position = playerPoition;
        Debug.Log("복귀 완료");
        yield return new WaitForSeconds(1);
        _playerBattleSystem._battleManager.EndPlayerTurn();
    }

    void IsAttack(EnemySystem target)
    {
        target.Enemy_CurrentHelth -= playerSystem.player_Damage;
        Debug.Log($"플레이어 체력 {playerSystem.player_Damage} 만큼 감소, 현제 체력: {target.Enemy_CurrentHelth}");
    }
}
