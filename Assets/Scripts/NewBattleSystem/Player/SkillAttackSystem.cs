using System.Collections;
using UnityEngine;

public class SkillAttackSystem : MonoBehaviour
{
    public PlayerBattleSystem _playerBattleSystem;

    private void Start()
    {
        _playerBattleSystem = Object.FindAnyObjectByType<PlayerBattleSystem>();
        _playerBattleSystem._playerSystem.HillEffect.gameObject.SetActive(false);
    }

    public IEnumerator Toad()       // 콩지, 두꺼비
    {
        _playerBattleSystem._playerSystem.player_Defense = 5;
        _playerBattleSystem._playerSystem.HillEffect.gameObject.SetActive(true);
        _playerBattleSystem._battleManager.CreateDamageText(transform.position, _playerBattleSystem._playerSystem.player_Defense, AttackType.Defense);
        yield return new WaitForSeconds(1f);
        _playerBattleSystem._playerSystem.HillEffect.gameObject.SetActive(false);
        _playerBattleSystem.isTarget = false;
        _playerBattleSystem._playerAttackSystem.currentSkill04Stemina += _playerBattleSystem._playerAttackSystem.Skill02Stemina;
        _playerBattleSystem._battleManager.EndPlayerTurn();
    }

    public IEnumerator aFlockOfBirds()  // 콩지, 새때
    {
        Instantiate(_playerBattleSystem._playerAttackSystem.ShootObject01,
            _playerBattleSystem._playerAttackSystem.ShootPoiont.transform.position, Quaternion.identity);

        yield return new WaitForSeconds(1f);
        _playerBattleSystem.isTarget = false;
        _playerBattleSystem._playerAttackSystem.currentSkill04Stemina += _playerBattleSystem._playerAttackSystem.Skill03Stemina;
        _playerBattleSystem._battleManager.EndPlayerTurn();
    }

    public IEnumerator Bull()  // 콩지, 황소
    {
        Instantiate(_playerBattleSystem._playerAttackSystem.ShootObject02, 
            _playerBattleSystem._playerAttackSystem.ShootPoiont.transform.position, Quaternion.identity);

        yield return new WaitForSeconds(1f);
        _playerBattleSystem.isTarget = false;
        _playerBattleSystem._battleManager.EndPlayerTurn();
        _playerBattleSystem._playerAttackSystem.currentSkill04Stemina += _playerBattleSystem._playerAttackSystem.currentSkill04Stemina = 0;
    }
}
