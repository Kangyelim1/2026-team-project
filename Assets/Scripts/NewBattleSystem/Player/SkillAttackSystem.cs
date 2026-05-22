using System.Collections;
using UnityEngine;

public class SkillAttackSystem : MonoBehaviour
{
    public PlayerBattleSystem _playerBattleSystem;
    public PlayerAttackSystem _playerAttackSystem;
    public PlayerSystem _playerSystem;
    public NewBattleManager _battleManager;

    private void Start()
    {
        if (_playerBattleSystem == null) _playerBattleSystem = Object.FindAnyObjectByType<PlayerBattleSystem>();
        if (_battleManager == null) _battleManager = Object.FindAnyObjectByType<NewBattleManager>();
    }

    private void Update()
    {
        if (_playerAttackSystem == null) _playerAttackSystem = Object.FindAnyObjectByType<PlayerAttackSystem>();
        if (_playerSystem == null) _playerSystem = Object.FindAnyObjectByType<PlayerSystem>();
    }

    public IEnumerator Toad()       // 콩지, 두꺼비
    {
        _playerSystem.player_Defense = 5;
        _playerSystem.DefanseEffect.gameObject.SetActive(true);
        _battleManager.CreateDamageText(transform.position, _playerSystem.player_Defense, AttackType.Defense);
        yield return new WaitForSeconds(1f);
        _playerSystem.DefanseEffect.gameObject.SetActive(false);
        _playerBattleSystem.isTarget = false;
        _playerAttackSystem.currentSkill04Stemina += _playerAttackSystem.Skill02Stemina;
        _playerBattleSystem.isAttack = false;
        _playerBattleSystem._battleManager.EndPlayerTurn();
    }

    public IEnumerator aFlockOfBirds()  // 콩지, 새때
    {
        Instantiate(_playerAttackSystem.ShootObject01, _playerAttackSystem.ShootPoiont.transform.position, Quaternion.identity);

        yield return new WaitForSeconds(1f);
        _playerBattleSystem.isTarget = false;
        _playerAttackSystem.currentSkill04Stemina += _playerBattleSystem._playerAttackSystem.Skill03Stemina;
        _playerBattleSystem.isAttack = false;
        _playerBattleSystem._battleManager.EndPlayerTurn();
    }

    public IEnumerator Bull()  // 콩지, 황소
    {
        Instantiate(_playerAttackSystem.ShootObject02, _playerAttackSystem.ShootPoiont.transform.position, Quaternion.identity);

        yield return new WaitForSeconds(1f);
        _playerBattleSystem.isTarget = false;
        _playerBattleSystem.isAttack = false;
        _playerAttackSystem.currentSkill04Stemina = _playerBattleSystem._playerAttackSystem.currentSkill04Stemina = 0;
        _playerBattleSystem._battleManager.EndPlayerTurn();
    }
}
