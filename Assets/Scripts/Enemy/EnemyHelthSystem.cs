using UnityEngine;

public class EnemyHelthSystem : MonoBehaviour
{
    public EnemySystem enemySystem;

    [Header("보스 체력")]
    public int maxBossHelth;
    public int minBossHelth;
    public int currentBossHelth;

    private StageClearManager stageClearManager;
    private BossClearSystem bossClearSystem;

    private void Start()
    {
        enemySystem = GetComponentInParent<EnemySystem>();

        stageClearManager = FindAnyObjectByType<StageClearManager>();
        bossClearSystem = FindAnyObjectByType<BossClearSystem>();

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
                    Helth(collision.transform.position);
                }
            }
        }
    }

    void Helth(Vector2 hitPos)
    {
        EnemyChargeSystem chargeSystem = GetComponentInParent<EnemyChargeSystem>();
        if (chargeSystem != null && chargeSystem.isInvincible)
        {
            Debug.Log("돌진 중 무적! 피해 무시");
            return;
        }

        EnemyShieldSystem shieldSystem = GetComponentInParent<EnemyShieldSystem>();
        if (shieldSystem != null)
        {
            bool canDamage = shieldSystem.TryTakeDamage(hitPos);
            if (!canDamage) return;
        }

        if (enemySystem.enemyType == EnemyType.Boss)
        {
            currentBossHelth -= 8;
            Debug.Log("보스 체력 감소");
            if (currentBossHelth <= 0) Die();
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

        if (stageClearManager != null)
        {
            stageClearManager.EnemyDead();
        }

        if (enemySystem.enemyType == EnemyType.Boss)
        {
            if (bossClearSystem != null)
            {
                bossClearSystem.GameClear();
            }
        }

        Destroy(enemySystem.gameObject);
    }
}