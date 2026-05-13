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

    public IEnumerator Toad()
    {
        _playerBattleSystem._playerSystem.player_Defense = 5;
        _playerBattleSystem._playerSystem.HillEffect.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        _playerBattleSystem._playerSystem.HillEffect.gameObject.SetActive(false);
        _playerBattleSystem.isTarget = false;
        _playerBattleSystem._battleManager.EndPlayerTurn();
    }
}
