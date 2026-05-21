using System.Collections;
using UnityEngine;

public class EnemyAttackSystem : MonoBehaviour
{
    [Header("적 이동 관련")]
    public float attackRange = 1.0f;
    public Vector3 enemyStartPosition;
    public GameObject TargetPlayer;

    [Header("외부 스크립트")]
    public EnemySystem enemySystem;
    public NewBattleManager _battleManager;
    public EnemyBattleSystem _enemyBattleSystem;
    public QuestSystem _questSystem;

    [Header("보스 전용")]
    public BossEnemyPatternSystem _bossEnemyPatternSystem;

    private void Start()
    {
        enemySystem = GetComponent<EnemySystem>();
        _battleManager = Object.FindAnyObjectByType<NewBattleManager>();
        _enemyBattleSystem = Object.FindAnyObjectByType<EnemyBattleSystem>();
        _questSystem = Object.FindAnyObjectByType<QuestSystem>();
 
        StartCoroutine(StartBattle());

        if(enemySystem.Enemy_Type == EnemyType.Boss)
            _bossEnemyPatternSystem = Object.FindAnyObjectByType<BossEnemyPatternSystem>();
    }

    IEnumerator StartBattle()
    {
        yield return new WaitForSeconds(0.1f);


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

        if (enemySystem.Enemy_CurrentHelth <= 0)
        {
            StartCoroutine(_battleManager.EndGame(true));
        }
        else
        {
            EnemyAttack();
        }
    }

    private void EnemyAttack()
    {
        if(enemySystem.Enemy_Type == EnemyType.Boss)
        {
            BossEnemyAttack();
        }
        else
        {
            StartCoroutine(NomalEnemyAttack());
        }
    }

    private IEnumerator NomalEnemyAttack()
    {
        Debug.Log("플레이어 위치로 이동");

        while (TargetPlayer != null && Vector3.Distance(transform.position, TargetPlayer.transform.position) > attackRange)
        {
            Vector3 direction = (TargetPlayer.transform.position - transform.position).normalized;
            transform.position += direction * enemySystem.Enemy_Speed * Time.deltaTime;

            yield return null;
        }

        if (TargetPlayer == null)
        {
            _battleManager.EndEnemyTurn();
            yield break;
        }

        Debug.Log("플레이어 공격");

        PlayerSystem player = TargetPlayer.GetComponent<PlayerSystem>();

        player.HitEffect.SetActive(true);
        if (player != null) IsAttack(player);
        yield return new WaitForSeconds(1f);

        if(player != null) player.HitEffect.SetActive(false);

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

    void BossEnemyAttack()
    {
        int randomPattern = Random.Range(0, enemySystem.bossPatternNameLIst.Count);

        string patternName = enemySystem.bossPatternNameLIst[randomPattern];

        BossPattern(enemySystem.Enemy_Name, patternName);
    }

    public void BossPattern(string BossName, string AttackType)
    {
        switch(BossName, AttackType)
        {
            case ("원님", "일반 공격"):
                StartCoroutine(NomalEnemyAttack());
                break;
            case ("원님", "관아의 위엄"):
                StartCoroutine(_bossEnemyPatternSystem.Wonnim01());
                break;
            case ("원님", "풍월 일섬"):
               StartCoroutine(_bossEnemyPatternSystem.Wonnim02());
                break;
            case ("원님", "난무"):
                StartCoroutine(_bossEnemyPatternSystem.Wonnim03());
                break;
            case ("원님", "월하 집행"):
                StartCoroutine(_bossEnemyPatternSystem.Wonnim04());
                break;
            default:
                Debug.LogWarning("패턴 미존재");
                break;
        }
    }

    private void IsAttack(PlayerSystem target)
    {
        if (target.player_CurrentHelth > 0)
        {
            int currentDamage = enemySystem.Enemy_Damage - _battleManager._playerBattleSystem._playerSystem.player_Defense;
            target.player_CurrentHelth -= currentDamage;
            target.player_CurrentHelth = Mathf.Clamp(target.player_CurrentHelth, 0, target.player_MaxHelth);
            _battleManager.CreateDamageText(target.transform.position, currentDamage, AttackType.Hit);

            Debug.Log($"플레이어 체력 {currentDamage} 만큼 감소, 현재 체력: {target.player_CurrentHelth}");

            if (target.player_CurrentHelth <= 0)
            {
                Debug.Log("플레이어 사망");
                Destroy(target.gameObject);
                _battleManager.spawnedPlayer = null;
            }
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            collision.gameObject.TryGetComponent<ShootObjectSystem>(out ShootObjectSystem shoot);

           StartCoroutine(_enemyBattleSystem.ShootDamage(shoot));
        }
    }
}