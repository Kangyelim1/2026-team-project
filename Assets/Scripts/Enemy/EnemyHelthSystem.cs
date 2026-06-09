using System.Collections;
using UnityEngine;

public class EnemyHelthSystem : MonoBehaviour
{
    public EnemySystem enemySystem;

    [Header("보스 체력")]
    public int maxBossHelth;
    public int minBossHelth;
    public int currentBossHelth;
    public bool isInvincibility;

    [Header("죽음 연출")]
    public float deathDelay = 0.8f;
    public float deathUpForce = 4f;
    public float deathSideForceMin = -2f;
    public float deathSideForceMax = 2f;
    public float deathTorqueMin = -120f;
    public float deathTorqueMax = 120f;

    private StageClearManager stageClearManager;
    private BossClearSystem bossClearSystem;

    public bool isDead = false;

    private void Start()
    {
        enemySystem = GetComponentInParent<EnemySystem>();
        stageClearManager = FindAnyObjectByType<StageClearManager>();
        bossClearSystem = FindAnyObjectByType<BossClearSystem>();

        if (enemySystem != null && enemySystem.enemyType == EnemyType.Boss)
            currentBossHelth = maxBossHelth;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Bullet"))
        {
            if (collision.gameObject.TryGetComponent(out BulletSystem bullet))
            {
                if (bullet.type == BulletType.PlayerBullet)
                    Helth(collision.transform.position);
            }
        }
    }

    void Helth(Vector2 hitPos)
    {
        if (isDead) return;

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

        if (enemySystem != null && enemySystem.enemyType == EnemyType.Boss)
        {
            if (!isInvincibility)
            {
                currentBossHelth -= 8;
                Debug.Log("보스 체력 감소");

                if (currentBossHelth <= 0)
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
        if (isDead) return;
        isDead = true;

        Debug.Log("몬스터 사망");

        EnemySystem es = GetComponentInParent<EnemySystem>();
        if (es != null) es.OnDead();

        EnemyChargeSystem charge = GetComponentInParent<EnemyChargeSystem>();
        if (charge != null) charge.OnDead();

        EnemyShieldSystem shield = GetComponentInParent<EnemyShieldSystem>();
        if (shield != null) shield.OnDead();

        if (stageClearManager != null)
            stageClearManager.EnemyDead();

        if (enemySystem != null && enemySystem.enemyType == EnemyType.Boss)
        {
            if (bossClearSystem != null)
                bossClearSystem.GameClear();
        }

        StartCoroutine(DieRoutine());
    }

    private IEnumerator DieRoutine()
    {
        if (enemySystem == null)
        {
            Destroy(gameObject);
            yield break;
        }

        Animator anim = enemySystem.GetComponent<Animator>();
        Rigidbody2D rb = enemySystem.GetComponent<Rigidbody2D>();
        Collider2D[] allCols = enemySystem.GetComponentsInChildren<Collider2D>(true);
        SpriteRenderer[] allSprites = enemySystem.GetComponentsInChildren<SpriteRenderer>(true);

        if (anim != null) anim.SetTrigger("Die");

        foreach (Collider2D col in allCols)
            col.enabled = false;

        if (enemySystem.BoomEffect != null)
            Instantiate(enemySystem.BoomEffect, enemySystem.transform.position, Quaternion.identity);

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.freezeRotation = false;
            rb.AddForce(new Vector2(Random.Range(deathSideForceMin, deathSideForceMax), deathUpForce), ForceMode2D.Impulse);
            rb.AddTorque(Random.Range(deathTorqueMin, deathTorqueMax));
        }

        foreach (SpriteRenderer sr in allSprites)
        {
            if (sr != null)
                sr.color = new Color(1f, 0.5f, 0.5f, 1f);
        }

        yield return new WaitForSeconds(deathDelay);
        Destroy(enemySystem.gameObject);
    }
}