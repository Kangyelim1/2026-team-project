using DG.Tweening;
using System.Collections;
using UnityEngine;


public class EnemyBattleSystem : MonoBehaviour
{
    public EnemyAttackSystem _enemyAttackSystem;
    public NewBattleManager _battleManager;
    public QuestSystem _questSystem;

    private void Start()
    {
        _battleManager = FindAnyObjectByType<NewBattleManager>();
        _questSystem = FindAnyObjectByType<QuestSystem>();
    }

    public IEnumerator ShootDamage(ShootObjectSystem shoot)
    {
        _enemyAttackSystem = FindAnyObjectByType<EnemyAttackSystem>();

        if(shoot != null)
        {
            _enemyAttackSystem.enemySystem.HitEffect.gameObject.SetActive(true);

            if (_enemyAttackSystem.enemySystem.Enemy_CurrentHelth > 0)
            {
                int currentDamage = shoot.Soot_Damage + _battleManager._playerBattleSystem._playerSystem.player_Damage;
                _enemyAttackSystem.enemySystem.Enemy_CurrentHelth -= currentDamage;
                _enemyAttackSystem.transform.DOShakePosition(0.25f, 0.2f, 20, 90);
                _battleManager.MainCamera.DOShakePosition(0.25f, 0.2f, 20, 90);
                _enemyAttackSystem.enemySystem.Hit();
                _enemyAttackSystem.enemySystem.Enemy_CurrentHelth = Mathf.Clamp(_enemyAttackSystem.enemySystem.Enemy_CurrentHelth, 0, _enemyAttackSystem.enemySystem.Enemy_MaxHelth);
                _battleManager.CreateDamageText(_enemyAttackSystem.transform.position, currentDamage, AttackType.Attack);

                yield return new WaitForSeconds(1f);
                _enemyAttackSystem.enemySystem.HitEffect.gameObject.SetActive(false);

                if (_enemyAttackSystem.enemySystem.Enemy_CurrentHelth <= 0)
                {
                    Destroy( _enemyAttackSystem.enemySystem.gameObject);
                    StartCoroutine(_battleManager.EndGame(true));

                    if (_questSystem.currentQuestEnemyNPC == _enemyAttackSystem.enemySystem.Enemy_Name)
                        _questSystem.currnet_EnmeyDieCount++;  
                }
            }
        }
    }
}
