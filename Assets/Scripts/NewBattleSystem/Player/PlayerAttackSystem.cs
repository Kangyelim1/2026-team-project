using System.Collections;
using UnityEngine;

public class PlayerAttackSystem : MonoBehaviour
{
    public PlayerSystem playerSystem;
    public PlayerBattleSystem _playerBattleSystem;
    public QuestSystem _questSystem;
    public SkillAttackSystem _skillAttackSystem;
    public EnemySystem _enemySystem;
    public GameObject TargetEnemy;

    public int currentDamage;

    public Vector3 playerPoition;
    public float attackRange = 1.0f;

    [Header("발사체 관련")]
    public GameObject ShootPoiont;
    public GameObject ShootObject01;
    public GameObject ShootObject02;

    [Header("궁극기 Stemina 관련")]
    public int Skill04Stemina = 10;
    public int currentSkill04Stemina;

    public int Skill01Stemina = 1;
    public int Skill02Stemina = 2;
    public int Skill03Stemina = 3;

    private void Start()
    {
        playerSystem = Object.FindAnyObjectByType<PlayerSystem>();
        _playerBattleSystem = Object.FindAnyObjectByType<PlayerBattleSystem>();
        _questSystem = Object.FindAnyObjectByType<QuestSystem>();
        _skillAttackSystem = Object.FindAnyObjectByType<SkillAttackSystem>();
        _enemySystem = Object.FindAnyObjectByType<EnemySystem>();

        currentDamage = playerSystem.player_Damage;
        _playerBattleSystem.ButtonHose.gameObject.SetActive(false);
        _playerBattleSystem.isAttack = true;
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
        _playerBattleSystem.isAttack = false;
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
}
