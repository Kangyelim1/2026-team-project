using JetBrains.Annotations;
using System.Collections;
using UnityEngine;

public class PlayerAttackSystem : MonoBehaviour
{
    public PlayerSystem playerSystem;
    public PlayerBattleSystem _playerBattleSystem;
    public QuestSystem _questSystem;
    public SkillAttackSystem _skillAttackSystem;
    
    public int currentDamage;
    public int Skill04Stemina;
    public int currentSkill04Stemina;
    public Vector3 playerPoition;
    public float attackRange = 1.0f;

    private void Start()
    {
        playerSystem = Object.FindAnyObjectByType<PlayerSystem>();
        _playerBattleSystem = Object.FindAnyObjectByType<PlayerBattleSystem>();
        _questSystem = Object.FindAnyObjectByType<QuestSystem>();
        _skillAttackSystem = Object.FindAnyObjectByType<SkillAttackSystem>();

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

    public IEnumerator isNomalAttack()
    {
        while (Vector3.Distance(transform.position, _playerBattleSystem.TargetEnemy.transform.position) > attackRange)
        {
            Vector3 direction = (_playerBattleSystem.TargetEnemy.transform.position - transform.position).normalized;
            transform.position += direction * playerSystem.player_Speed * Time.deltaTime;

            yield return null;
        }

        _playerBattleSystem.TargetEnemy.TryGetComponent<EnemySystem>(out EnemySystem enemy);

        if (enemy == null) yield break;

        Debug.Log("근거리 일반 공격");
        //playerSystem.playerAnimator.SetBool("isAttack", true);
        enemy.HitEffect.gameObject.SetActive(true);
        DealDamage(enemy);
        yield return new WaitForSeconds(1f);
        //playerSystem.playerAnimator.SetBool("isAttack", false);
        enemy.HitEffect.gameObject.SetActive(false);

        while (Vector3.Distance(transform.position, playerPoition) > 0.1f)
        {
            Vector3 backDirection = (playerPoition - transform.position).normalized;
            transform.position += backDirection * playerSystem.player_Speed * Time.deltaTime;
            yield return null;
        }

        transform.position = playerPoition;
        Debug.Log("복귀 완료");

        yield return new WaitForSeconds(1f);

        _playerBattleSystem.isTarget = false;
        _playerBattleSystem._battleManager.EndPlayerTurn();

    }

    public void SkillAttack(string playerName, string AttackType)
    {
        switch(playerName, AttackType)
        {
            case("콩지", "두꺼비"):
                StartCoroutine(_skillAttackSystem.Toad());
                Debug.Log("방어력 증가");
                _playerBattleSystem.isTarget = false;
                
                break;
            case ("콩지", "새 때"):
                StartCoroutine(_skillAttackSystem.aFlockOfBirds());
                Debug.Log("새 때 공격 진행");
                _playerBattleSystem.isTarget = false;
            
                break;
            case ("콩지", "황소"):
                StartCoroutine(_skillAttackSystem.Bull());
                Debug.Log("항소 공격 진행");
                _playerBattleSystem.isTarget = false;
             
                break;
            default:
                Debug.Log("공격 타입 미존재");
                break;
        }
    }

    void DealDamage(EnemySystem target)
    {
        
        target.Enemy_CurrentHelth -= playerSystem.player_Damage;
        Debug.Log($"{target.Enemy_Name}에게 {playerSystem.player_Damage} 데미지. 남은 체력: {target.Enemy_CurrentHelth}");
        _playerBattleSystem._battleManager.CreateDamageText(target.transform.position, currentDamage, AttackType.Attack);
        currentSkill04Stemina += 1;

        if (target.Enemy_CurrentHelth <= 0)
        {
            Debug.Log($"{target.Enemy_Name} 처치!");
            Destroy(target.gameObject);
            
            _playerBattleSystem._battleManager.EndGame(true);


            if(target.Enemy_Name == _questSystem.currentQuestEnemyNPC)
            {
                _questSystem.currnet_EnmeyDieCount++;
                return;
            }
        }
    }
}
