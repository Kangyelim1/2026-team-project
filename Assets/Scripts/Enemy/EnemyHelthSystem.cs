using Unity.VisualScripting;
using UnityEngine;

public class EnemyHelthSystem : MonoBehaviour
{
    public EnemySystem enemySystem;
    public PlayerAttackSystem playerAttackSystem;
    public PlayerSystem playerSystem;

    private void Start()
    {
        enemySystem = GetComponentInParent<EnemySystem>();
        playerAttackSystem = FindAnyObjectByType<PlayerAttackSystem>();
        playerSystem = FindAnyObjectByType<PlayerSystem>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            if (collision.gameObject.TryGetComponent(out BulletSystem bullet))
            {
                if (bullet.type == BulletType.PlayerBullet)
                {
                    Helth();
                }
            }
        }
    }


    void Helth()
    {
        if (enemySystem.enemyType == EnemyType.Boss)
        {
            Debug.Log("보스 체력 감소");
        }
        else Die();
    }

    public void Die()
    {
        Debug.Log("몬스터 사망");
        Destroy(enemySystem.gameObject);
    }
}
