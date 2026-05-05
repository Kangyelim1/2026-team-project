using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyAttackSystem : MonoBehaviour
{
    [Header("적 이동 관련")]
    public float attackRange = 1.0f;
    public Vector3 EnemeyPoition;
    public GameObject TargetPlayer;

    public EnemySystem enemySystem;
    public NewBattleManager _battleManager;

    private void Start()
    {
        enemySystem = Object.FindAnyObjectByType<EnemySystem>();
        _battleManager = Object.FindAnyObjectByType<NewBattleManager>();

        EnemeyPoition = transform.position;
    }

    public void AutoSelectPlayer()
    {
        int targetPlayer = Mathf.Min(_battleManager.playerData.Count);
        targetPlayer -= 1;
        GameObject target = _battleManager.spawnedPlayers[targetPlayer];
        PlayerSystem playerSystem = target.GetComponent<PlayerSystem>();
        TargetPlayer = playerSystem.playerPrefab;

        Debug.Log($"타켓 플레이어 지정 완료{playerSystem.player_Name}");
        StartCoroutine(EnemyAttact());
    }

    IEnumerator EnemyAttact()
    {
        Debug.Log("플레이어 위치로 이동");
        while (Vector3.Distance(transform.position, TargetPlayer.transform.position) > attackRange)
        {
            Vector3 direction = (TargetPlayer.transform.position - transform.position).normalized;
            transform.position += direction * enemySystem.Enemy_Speed * Time.deltaTime;

            yield return null;
        }

        Debug.Log("플레이어 공격");
        TargetPlayer.TryGetComponent<PlayerSystem>(out PlayerSystem player);
        IsAttack(player);

        //player.playerAnimator.SetBool("isHit", true);
        yield return new WaitForSeconds(0.2f);
        //player.playerAnimator.SetBool("isHit", false);

        Debug.Log("원위치");
        while (Vector3.Distance(transform.position, EnemeyPoition) > 0.1f)
        {
            Vector3 backDirection = (EnemeyPoition - transform.position).normalized;
            transform.position += backDirection * enemySystem.Enemy_Speed * Time.deltaTime;
            yield return null;
        }
        transform.position = EnemeyPoition;
        Debug.Log("복귀 완료");
        yield return new WaitForSeconds(1);
        _battleManager.currentEnemyTurnIndex++;
        _battleManager.Turn();
    }

    void IsAttack(PlayerSystem target)
    {
        target.player_CurrentHelth -= enemySystem.Enemy_Damage;
        Debug.Log($"플레이어 체력 {enemySystem.Enemy_Damage} 만큼 감소, 현제 체력: {target.player_CurrentHelth}");
    }
}
