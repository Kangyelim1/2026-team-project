using JetBrains.Annotations;
using System.Collections;
using UnityEngine;

public class PlayerAttackSystem : MonoBehaviour
{
    public PlayerSystem playerSystem;
    public PlayerBattleSystem _playerBattleSystem;
    public QuestSystem _questSystem;
    
    public int currentDamage;
    public Vector3 playerPoition;
    public float attackRange = 1.0f;

    private void Start()
    {
        playerSystem = Object.FindAnyObjectByType<PlayerSystem>();
        _playerBattleSystem = Object.FindAnyObjectByType<PlayerBattleSystem>();
        _questSystem = Object.FindAnyObjectByType<QuestSystem>();

        currentDamage = playerSystem.player_Damage;
        _playerBattleSystem.ButtonHose.gameObject.SetActive(false);
        
        StartCoroutine(StartBattle());
    }

    IEnumerator StartBattle()
    {
        while (Vector3.Distance(transform.position, _playerBattleSystem.playerOpPoint.transform.position) > attackRange)
        {
            Vector3 direction = (_playerBattleSystem.playerOpPoint.transform.position - transform.position).normalized;
            transform.position += direction * playerSystem.player_Speed * Time.deltaTime;

            yield return null;

        }

        yield return new WaitForSeconds(0.7f);
        playerPoition = transform.position;
        _playerBattleSystem.ButtonHose.gameObject.SetActive(true);
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

        if (enemy != null)
            DealDamage(enemy);
        else
            Debug.LogWarning("[PlayerAttackSystem] 타겟에 EnemySystem 컴포넌트가 없습니다.");

        Debug.Log("원위치로 복귀");

        // 원래 위치로 복귀
        while (Vector3.Distance(transform.position, playerPoition) > 0.1f)
        {
            Vector3 backDirection = (playerPoition - transform.position).normalized;
            transform.position += backDirection * playerSystem.player_Speed * Time.deltaTime;
            yield return null;
        }

        transform.position = playerPoition;
        Debug.Log("복귀 완료");

        yield return new WaitForSeconds(1f);

        // [수정] isTarget 초기화 — 다음 턴에 타겟 재선택 가능하도록
        _playerBattleSystem.isTarget = false;

        _playerBattleSystem._battleManager.EndPlayerTurn();

        /*IsAttack(enemy);

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
        _playerBattleSystem._battleManager.EndPlayerTurn();*/
    }

    void DealDamage(EnemySystem target)
    {
        
        target.Enemy_CurrentHelth -= playerSystem.player_Damage;
        Debug.Log($"[공격] {target.Enmey_Name}에게 {playerSystem.player_Damage} 데미지. 남은 체력: {target.Enemy_CurrentHelth}");

        
        if (target.Enemy_CurrentHelth <= 0)
        {
            Debug.Log($"[사망] {target.Enmey_Name} 처치!");
            Destroy(target.gameObject);
            
            _playerBattleSystem._battleManager.EndGame(true);


            if(target.Enmey_Name == _questSystem.currentQuestEnemyNPC)
            {
                _questSystem.currnet_EnmeyDieCount++;
                return;
            }
        }
    }

    void IsAttack(EnemySystem target)
    {
        if(target.Enemy_CurrentHelth > 0)
        {
            target.Enemy_CurrentHelth -= playerSystem.player_Damage;
            Debug.Log($"플레이어 체력 {playerSystem.player_Damage} 만큼 감소, 현제 체력: {target.Enemy_CurrentHelth}");
        }
        else
        {
            Debug.Log($"{target.Enmey_Name} 사망");
            _playerBattleSystem._battleManager.EndGame(true);
            Destroy(target.gameObject);
        }
       
    }
}
