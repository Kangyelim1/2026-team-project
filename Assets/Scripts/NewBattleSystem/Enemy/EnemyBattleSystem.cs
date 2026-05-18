using System.Collections;
using UnityEngine;


public class EnemyBattleSystem : MonoBehaviour
{
    public EnemyAttackSystem _enemyAttackSystem;
    public NewBattleManager _battleManager;

    private void Start()
    {
        _battleManager = FindAnyObjectByType<NewBattleManager>();
    }

    private void Update()
    {
        if (_battleManager.isGameEnd) _enemyAttackSystem = null;
    }

    public IEnumerator ShootDamage(ShootObjectSystem shoot)
    {
        _enemyAttackSystem = FindAnyObjectByType<EnemyAttackSystem>();
        Debug.Log("코루틴 진입 성공");

        if(shoot != null)
        {
            Debug.Log("통과");
            _enemyAttackSystem.enemySystem.HitEffect.gameObject.SetActive(true);

            if (_enemyAttackSystem.enemySystem.Enemy_CurrentHelth > 0)
            {
                int currentDamage = shoot.Soot_Damage + _battleManager._playerBattleSystem._playerSystem.player_Damage;
                _enemyAttackSystem.enemySystem.Enemy_CurrentHelth -= currentDamage;
                _battleManager.CreateDamageText(_enemyAttackSystem.transform.position, currentDamage, AttackType.Attack);

                yield return new WaitForSeconds(1f);
                _enemyAttackSystem.enemySystem.HitEffect.gameObject.SetActive(false);

                if (_enemyAttackSystem.enemySystem.Enemy_CurrentHelth <= 0)
                {
                    Destroy( _enemyAttackSystem.enemySystem.gameObject);
                    StartCoroutine(_battleManager.EndGame(true));
                }

            }
        }
        else
        {
            Debug.LogWarning("발사체를 못 가져옴");
        }
      
    }
}
