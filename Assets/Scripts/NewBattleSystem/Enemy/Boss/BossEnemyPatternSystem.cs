using DG.Tweening;
using System.Collections;
using UnityEngine;

public class BossEnemyPatternSystem : MonoBehaviour
{
    [Header("외부 스크립트")]
    public EnemyBattleSystem _enemyBattleSystem;
    public EnemyAttackSystem _enemyAttackSystem;
    public EnemySystem _enemySystem;
    public PlayerSystem _playerSystem;
    public NewBattleManager _battleManager;
    public ScreenHitEffect _screenHitEffect;
    public BossUISystem _bossUISystem;

    [Header("원님 패턴 공격력")]
    [Tooltip("순간 이동후 강공격(풍월 일섬)")]
    public int WonnimPattern02;
    [Tooltip("4번 연속 공격(난무)")]
    public int WonnimPattern03;
    [Tooltip("궁극기(월하 집행)")]
    public int WonnimPattern04; 

    private void Start()
    {
        _enemyBattleSystem = Object.FindAnyObjectByType<EnemyBattleSystem>();
        _battleManager = Object.FindAnyObjectByType<NewBattleManager>();
        _screenHitEffect = Object.FindAnyObjectByType<ScreenHitEffect>();
        _bossUISystem = Object.FindAnyObjectByType<BossUISystem>();
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
        _playerSystem.DebuffEffect.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        _playerSystem.DebuffEffect.gameObject.SetActive(false);
        Debug.Log("플레이어 턴 넘김 패턴");
        yield return new WaitForSeconds(0.1f);
        _enemyBattleSystem._battleManager.EndPlayerTurn();
        
    }

    public IEnumerator Wonnim02()   // 강공격
    {
        Debug.Log("풍월 일섬 진행");

        SpriteRenderer bossSprite = _enemySystem.Boss_Image.GetComponent<SpriteRenderer>();

        Vector3 startPos = _enemyAttackSystem.enemyStartPosition;
        Vector3 targetPos = _enemyAttackSystem.TargetPlayer.transform.position + new Vector3(1.2f, 0f, 0f);
        Vector3 originalScale = new Vector3(0.4f, 0.4f, 1f);

        yield return bossSprite.DOFade(0f, 0.15f).WaitForCompletion();

        _enemySystem.transform.position = targetPos;
        _enemySystem.transform.localScale = originalScale * 1.2f;

        yield return bossSprite.DOFade(1f, 0.15f).WaitForCompletion();
        yield return _enemySystem.transform.DOScale(originalScale, 0.12f).WaitForCompletion();

        Debug.Log("순간 이동 완료");

        yield return new WaitForSeconds(0.1f);

        _enemySystem.AttackEffect.gameObject.SetActive(true);

        int currentDamage = _enemySystem.Enemy_Damage + WonnimPattern02;

        _playerSystem.HitEffect.gameObject.SetActive(true);

        TakeDamage(currentDamage);

        yield return new WaitForSeconds(1f);

        if (_playerSystem != null)
            _playerSystem.HitEffect.gameObject.SetActive(false);

        _enemySystem.AttackEffect.gameObject.SetActive(false);

        yield return bossSprite.DOFade(0f, 0.12f).WaitForCompletion();

        _enemySystem.transform.position = startPos;

        yield return bossSprite.DOFade(1f, 0.12f).WaitForCompletion();

        yield return new WaitForSeconds(0.3f);

        _enemyBattleSystem._battleManager.EndEnemyTurn();
    }

    public IEnumerator Wonnim03()   // 4번 배기 공격
    {
        Debug.Log("난무 진행");

        while (Vector3.Distance(_enemySystem.transform.position, _enemyAttackSystem.TargetPlayer.transform.position) > 0.1f)
        {
            Vector3 direction = (_enemyAttackSystem.TargetPlayer.transform.position - _enemySystem.transform.position).normalized;
            _enemySystem.transform.position += direction * _enemySystem.Enemy_Speed * Time.deltaTime;

            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        int currentDamage = _enemySystem.Enemy_Damage + WonnimPattern03;
        for (int i = 0; i < 4; i++)
        {
            _enemySystem.AttackEffect.gameObject.SetActive(true);
            _playerSystem.HitEffect.gameObject.SetActive(true);
            TakeDamage(currentDamage);
            yield return new WaitForSeconds(0.1f);
            if (_playerSystem != null) _playerSystem.HitEffect.gameObject.SetActive(false);
            _enemySystem.AttackEffect.gameObject.SetActive(false);
            Debug.Log(currentDamage);

            yield return null;
        }

        while (Vector3.Distance(_enemySystem.transform.position, _enemyAttackSystem.enemyStartPosition) > 0.1f)
        {
            Vector3 backDirection = (_enemyAttackSystem.enemyStartPosition - _enemySystem.transform.position).normalized;

            _enemySystem.transform.position += backDirection * _enemySystem.Enemy_Speed * Time.deltaTime;

            yield return null;
        }

        _enemySystem.transform.position = _enemyAttackSystem.enemyStartPosition;

        yield return new WaitForSeconds(1f);
        _enemyBattleSystem._battleManager.EndEnemyTurn();
    }

    public IEnumerator Wonnim04()   // 궁극기
    {
        _bossUISystem.Skill04Image.gameObject.SetActive(true);
        _bossUISystem.Skill04Image.color = new Color(_bossUISystem.Skill04Image.color.r, _bossUISystem.Skill04Image.color.g, _bossUISystem.Skill04Image.color.b,0f);

        yield return _bossUISystem.Skill04Image.DOFade(1f, 0.5f).WaitForCompletion();
        yield return new WaitForSeconds(1f);
        yield return _bossUISystem.Skill04Image.DOFade(0f, 0.5f).WaitForCompletion();

        _bossUISystem.Skill04Image.gameObject.SetActive(false);

        
        int currentDamage = _enemySystem.Enemy_Damage + WonnimPattern04;
        _enemySystem.Skill04Effect.gameObject.SetActive(true);
        _playerSystem.HitEffect.gameObject.SetActive(true);
        TakeDamage(currentDamage);
        Debug.Log("월하 집행 진행");
        yield return new WaitForSeconds(1f);
        _playerSystem.HitEffect.gameObject.SetActive(false);
        _enemySystem.Skill04Effect.gameObject.SetActive(false);
        _enemyBattleSystem._battleManager.EndEnemyTurn();
    }

    void TakeDamage(int Damage)
    {
        if(_playerSystem.player_CurrentHelth > 0)
        {
            Debug.Log(Damage);
            _playerSystem.transform.DOShakePosition(0.25f, 0.2f, 20, 90);
            _battleManager.MainCamera.transform.DOShakePosition(0.25f, 0.2f, 20, 90);
            _playerSystem.Hit();
            _screenHitEffect.PlayerHitFlash();
            _playerSystem.player_CurrentHelth -= Damage;
            _playerSystem.player_CurrentHelth = Mathf.Clamp(_playerSystem.player_CurrentHelth, 0, _playerSystem.player_MaxHelth);
            _battleManager.CreateDamageText(_playerSystem.transform.position, Damage, AttackType.Hit);
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

