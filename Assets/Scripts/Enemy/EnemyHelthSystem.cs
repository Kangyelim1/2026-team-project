using Unity.VisualScripting;
using UnityEngine;

public class EnemyHelthSystem : MonoBehaviour
{
    public EnemySystem enemySystem;
    public PlayerAttackSystem playerAttackSystem;

    private void Start()
    {
        enemySystem = FindAnyObjectByType<EnemySystem>();
        playerAttackSystem = FindAnyObjectByType<PlayerAttackSystem>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            if (collision.gameObject.TryGetComponent(out BulletSystem bullet))
            {
                if (bullet.type == BulletType.PlayerBullet)
                {
                    Die();
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (enemySystem.playerMoveSystem.isDashAttack && collision.gameObject.CompareTag("Player"))
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("몬스터 사망");
        Destroy(enemySystem.gameObject);
    }
}
