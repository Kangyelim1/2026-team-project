using System.Collections;
using UnityEngine;

public class ShootObjectSystem : MonoBehaviour
{
    public ShootObjectSO shootObjectSO;

    public string Shoot_Name;
    public int Soot_Speed;
    public int Soot_Damage;

    public PlayerBattleSystem _playerBattleSystem;

    private void Start()
    {
        Shoot_Name = shootObjectSO.ShootName;
        Soot_Speed = shootObjectSO.ShootSpeed;
        Soot_Damage = shootObjectSO.SootDamege;

        _playerBattleSystem = Object.FindAnyObjectByType<PlayerBattleSystem>();
    }

    private void Update()
    {
        if(_playerBattleSystem._playerAttackSystem.TargetEnemy != null)
        {
            Vector3 Derection = (_playerBattleSystem._playerAttackSystem.TargetEnemy.transform.position - transform.position).normalized;
            Vector3 Move = Derection * Soot_Speed * Time.deltaTime;
            transform.position += Move;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Enemy"))
        {
            if (Shoot_Name == "황소") StartCoroutine(bull());
            else Destroy(gameObject);
        }
    }

    IEnumerator bull()
    {
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
}
