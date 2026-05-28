using UnityEngine;

public class EnemyHelthSystem : MonoBehaviour
{
    public EnemySystem enemySystem;

    [Header("보스 체력")]
    public int maxBossHelth;
    public int MinBossHelth;
    public int currentBossHelth;

    private void Start()
    {
        enemySystem = GetComponentInParent<EnemySystem>();

        if (enemySystem.enemyType == EnemyType.Boss)
        {
            currentBossHelth = maxBossHelth;
        }
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
            currentBossHelth -= 8;
            Debug.Log("보스 체력 감소");

            if (currentBossHelth <= 0)
            {
                Die();
            }
        }
        else 
        {
            Debug.Log("일반로봇 사망");
            Die();
        } 
    }

    public void Die()
    {
        Debug.Log("몬스터 사망");
        Destroy(enemySystem.gameObject);
    }
}
