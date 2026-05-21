using JetBrains.Annotations;
using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BossEnemyPatternSystem : MonoBehaviour
{
    public EnemyBattleSystem _enemyBattleSystem;
    public EnemyAttackSystem _enemyAttackSystem;
    public EnemySystem _enemySystem;
    public PlayerSystem _playerSystem;
    public NewBattleManager _battleManager;

    private void Start()
    {
        _enemyBattleSystem = Object.FindAnyObjectByType<EnemyBattleSystem>();
        _battleManager = Object.FindAnyObjectByType<NewBattleManager>();
    }

    private void Update()
    {
        if(_enemySystem == null) _enemySystem = Object.FindAnyObjectByType<EnemySystem>();
        if (_playerSystem == null) _playerSystem = Object.FindAnyObjectByType<PlayerSystem>();
        if (_enemyAttackSystem == null) _enemyAttackSystem = Object.FindAnyObjectByType<EnemyAttackSystem>();
    }

    public IEnumerator Wonnim01()   // 플레이어 턴을 바로 넘김
    {
        Debug.Log("관아의 위엄 진행");
        yield return new WaitForSeconds(1f);
        _enemyBattleSystem._battleManager.EndEnemyTurn();
        yield return new WaitForSeconds(0.1f);
        _enemyBattleSystem._battleManager.EndPlayerTurn();
        Debug.Log("플레이어 턴 넘김 패턴");
    }

    public IEnumerator Wonnim02()   // 강공격
    {
        Debug.Log("풍월 일섬 진행");

        _enemySystem.Boss_Image.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        _enemySystem.transform.position = _enemyAttackSystem.TargetPlayer.transform.position;
        _enemySystem.Boss_Image.gameObject.SetActive(true);
        Debug.Log("순간 이동 완료");
        yield return new WaitForSeconds(0.1f);
        int currentDamage = _enemySystem.Enemy_Damage + 50;
        _playerSystem.HitEffect.gameObject.SetActive(true);
        TakeDamage(currentDamage);
       yield return new WaitForSeconds(1f);
       if(_playerSystem != null) _playerSystem.HitEffect.gameObject.SetActive(false);
        _enemySystem.transform.position = _enemyAttackSystem.enemyStartPosition;
        yield return new WaitForSeconds(1f);
        _enemyBattleSystem._battleManager.EndEnemyTurn();
        
    }

    public IEnumerator Wonnim03()   // 4번의 배기 공격
    {
        Debug.Log("난무 진행");

        while (Vector3.Distance(_enemySystem.transform.position, _enemyAttackSystem.TargetPlayer.transform.position) > 0.1f)
        {
            Vector3 direction = (_enemyAttackSystem.TargetPlayer.transform.position - _enemySystem.transform.position).normalized;
            _enemySystem.transform.position += direction * _enemySystem.Enemy_Speed * Time.deltaTime;

            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        int currentDamage = _enemySystem.Enemy_Damage + 10;
        for (int i = 0; i < 4; i++)
        {
            _playerSystem.HitEffect.gameObject.SetActive(true);
            TakeDamage(currentDamage);
            yield return new WaitForSeconds(0.1f);
            if (_playerSystem != null) _playerSystem.HitEffect.gameObject.SetActive(false);

            yield return null;
        }

        while (Vector3.Distance(_enemySystem.transform.position, _enemyAttackSystem.enemyStartPosition) > 0.1f)
        {
            Vector3 backDirection =
                (_enemyAttackSystem.enemyStartPosition - _enemySystem.transform.position).normalized;

            _enemySystem.transform.position +=
                backDirection * _enemySystem.Enemy_Speed * Time.deltaTime;

            yield return null;
        }

        _enemySystem.transform.position = _enemyAttackSystem.enemyStartPosition;

        yield return new WaitForSeconds(1f);
        _enemyBattleSystem._battleManager.EndEnemyTurn();
    }

    public IEnumerator Wonnim04()   // 궁극기
    {
        Debug.Log("월하 집행 진행");
        yield return new WaitForSeconds(1f);
        _enemyBattleSystem._battleManager.EndEnemyTurn();
    }

    void TakeDamage(int Damage)
    {
        if(_playerSystem.player_CurrentHelth > 0)
        {
            Debug.Log(Damage);
            _playerSystem.player_CurrentHelth -= Damage;
            _playerSystem.player_CurrentHelth = Mathf.Clamp(_playerSystem.player_CurrentHelth, 0, Damage);
            if (_playerSystem.player_CurrentHelth <= 0)
            {
                Debug.Log("플레이어 사망");
                StartCoroutine(DiePlayer());
            }
        }
    }

    IEnumerator DiePlayer()
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(_playerSystem.gameObject);
        _battleManager.spawnedPlayer = null;
    }
}

