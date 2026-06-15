using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySystem : MonoBehaviour
{
    public EnemySO enemySO;
    public EnemyHelthSystem enemyhelthSystem;
    public string enemyName;
    public EnemyType enemyType;
    public Transform ShootPoint;
    public GameObject BulletPrefab;
    public GameObject BoomEffect;
    public BossSystem bossPattern;
    public PlayerSystem playerSystem;
    public PlayerHelthSystem playerHelthSystem;
    public float moveSpeed = 3f;
    public float stopDistance = 1.2f;
    public float shootDelay = 1f;
    public float bulletSpeed = 8f;
    public float chaseAfterExitTime = 3f;
    public float chaseDistance = 10f;

    [Header("보스")]
    public int BossHelth;
    public bool isPattern;

    [Header("드론")]
    public float droneMoveSpeed = 4f;
    public float droneDetectDistance = 20f;
    public float droneBoomDistance;

    [Header("근접 공격 애니메이션")]
    public float attackRange = 1.5f;
    public float attackCooldown = 1.2f;
    public LayerMask playerLayer;

    private bool isAttack;
    private bool isDistonse;
    private bool isShoot;
    private bool isShortAttack;
    private bool isBoom;
    private bool isDead = false;
    private Coroutine chaseStopCoroutine;

    private Animator animator;
    private bool isAttacking = false;
    private float lastAttackTime = -999f;
    private bool isShooting = false;

    private void Awake()
    {
        enemyName = enemySO.enemyName;
        enemyType = enemySO.enemyType;
    }

    private SpriteRenderer spriteRenderer;
    
    private void Start()
    {
        if (enemyType == EnemyType.Boss)
            BossHelth = enemySO.BossHelth;

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (isDead) return;

        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = 1f;
            spriteRenderer.color = c;
        }

        if (playerHelthSystem == null)
                playerHelthSystem = FindAnyObjectByType<PlayerHelthSystem>();
        if (enemyhelthSystem == null)
            enemyhelthSystem = FindAnyObjectByType<EnemyHelthSystem>();
        if (playerSystem == null)
            playerSystem = FindAnyObjectByType<PlayerSystem>();
        else
        {
            if (enemyType != EnemyType.Charge)
                FlipToPlayer();

            if (enemyType == EnemyType.Boss)
            {
                if (bossPattern == null)
                    bossPattern = FindAnyObjectByType<BossSystem>();
            }

            if (enemyType == EnemyType.Drone)
                isAttack = true;

            if (!isAttack && playerSystem != null && enemyType != EnemyType.Charge)
            {
                float dist = Vector2.Distance(transform.position, playerSystem.transform.position);
                if (dist < chaseDistance)
                    isAttack = true;
            }

            if (isAttack)
                StartAttack();

            if (enemyType == EnemyType.Shortdistance && playerSystem != null)
            {
                float dist = Vector2.Distance(transform.position, playerSystem.transform.position);
                if (dist <= attackRange)
                {
                    if (!isAttacking && Time.time >= lastAttackTime + attackCooldown)
                        StartCoroutine(MeleeAttackRoutine());
                }
            }
        }
    }

    public void OnDead()
    {
        if (isDead) return;
        isDead = true;
        StopAllCoroutines();

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        isAttack = false;
        isShortAttack = false;
        isShoot = false;
        isBoom = false;
        isAttacking = false;
        isShooting = false;

        if (animator != null)
        {
            animator.SetBool("isAttacking", false);
            animator.SetBool("isShooting", false);
        }

        if (chaseStopCoroutine != null)
            StopCoroutine(chaseStopCoroutine);
        chaseStopCoroutine = null;

        Debug.Log("EnemySystem OnDead");
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isDead) return;
        if (collision.gameObject.CompareTag("Player"))
        {
            if (chaseStopCoroutine != null)
                StopCoroutine(chaseStopCoroutine);
            chaseStopCoroutine = null;
            isAttack = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isDead) return;
        if (!collision.CompareTag("Player")) return;
        if (enemyType == EnemyType.Shortdistance || enemyType == EnemyType.Drone) return;
        if (chaseStopCoroutine != null)
            StopCoroutine(chaseStopCoroutine);
        if (enemyType == EnemyType.Boss) return;
        chaseStopCoroutine = StartCoroutine(StopChaseAfterDelay());
    }

    private IEnumerator StopChaseAfterDelay()
    {
        yield return new WaitForSeconds(chaseAfterExitTime);
        isAttack = false;
        chaseStopCoroutine = null;
    }

    private void StartAttack()
    {
        if (isDead) return;
        switch (enemyType)
        {
            case EnemyType.Shortdistance:
                MoveToPlayer();
                break;
            case EnemyType.LongDistanc:
                LongDistanceAttack();
                break;
            case EnemyType.Boss:
                if (enemyhelthSystem.currentBossHelth < enemyhelthSystem.minBossHelth)
                    MoveToPlayer();
                break;
            case EnemyType.Drone:
                DroneAttack();
                break;
        }
    }

    private void MoveToPlayer()
    {
        if (isDead || playerSystem == null || isPattern) return;

        float distance = Vector2.Distance(transform.position, playerSystem.transform.position);
        if (distance < chaseDistance)
        {
            if (distance > stopDistance)
            {
                if (isAttacking) return;

                Vector3 targetPos = new Vector3(
                    playerSystem.transform.position.x,
                    transform.position.y,
                    transform.position.z);
                Vector2 direction = (targetPos - transform.position).normalized;
                transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
                isDistonse = false;
            }
            else
            {
                isDistonse = true;
                if (enemyType == EnemyType.Drone)
                {
                    if (!isBoom) StartCoroutine(Boom());
                }
                else if (enemyType == EnemyType.Shortdistance || enemyType == EnemyType.Boss)
                {
                    if (!isShortAttack) StartCoroutine(ShortdistanceAttack());
                }
            }
        }
    }

    private IEnumerator ShortdistanceAttack()
    {
        isShortAttack = true;
        yield return new WaitForSeconds(1f);
        if (isDead) { isShortAttack = false; yield break; }

        if (playerSystem != null)
        {
            float dist = Vector2.Distance(transform.position, playerSystem.transform.position);
            if (dist <= stopDistance)
            {
                PlayerHelthSystem ph = playerSystem.GetComponent<PlayerHelthSystem>();
                if (ph == null) ph = playerSystem.GetComponentInParent<PlayerHelthSystem>();
                if (ph == null) ph = FindAnyObjectByType<PlayerHelthSystem>();
                if (ph != null) ph.Die();
            }
        }

        yield return new WaitForSeconds(1f);
        if (isDead) { isShortAttack = false; yield break; }
        isShortAttack = false;
    }

    private IEnumerator MeleeAttackRoutine()
    {
        isAttacking = true;
        if (animator != null) animator.SetBool("isAttacking", true);
        lastAttackTime = Time.time;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(0.4f);

        if (!isDead && playerSystem != null)
        {
            float dist = Vector2.Distance(transform.position, playerSystem.transform.position);
            if (dist <= attackRange)
            {
                PlayerHelthSystem ph = playerSystem.GetComponent<PlayerHelthSystem>();
                if (ph == null) ph = playerSystem.GetComponentInParent<PlayerHelthSystem>();
                if (ph == null) ph = FindAnyObjectByType<PlayerHelthSystem>();
                if (ph != null) ph.Die();
            }
        }

        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
        if (animator != null) animator.SetBool("isAttacking", false);
    }

    private void LongDistanceAttack()
    {
        if (isDead) return;
        if (!isShoot) StartCoroutine(ShootBullet());
    }

    private IEnumerator ShootBullet()
    {
        isShoot = true;
        isShooting = true;
        if (animator != null) animator.SetBool("isShooting", true);

        yield return new WaitForSeconds(shootDelay);
        if (isDead)
        {
            isShoot = false;
            isShooting = false;
            if (animator != null) animator.SetBool("isShooting", false);
            yield break;
        }

        if (BulletPrefab != null && ShootPoint != null && playerSystem != null)
        {
            GameObject bullet = Instantiate(BulletPrefab, ShootPoint.position, Quaternion.identity);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 direction = (playerSystem.transform.position - ShootPoint.position).normalized;
                rb.linearVelocity = direction * bulletSpeed;
            }
        }

        yield return new WaitForSeconds(0.5f);
        isShoot = false;
        isShooting = false;
        if (animator != null) animator.SetBool("isShooting", false);
    }

    private IEnumerator Boom()
    {
        isBoom = true;
        isAttack = false;
        yield return new WaitForSeconds(3f);
        if (isDead) yield break;
        if (playerSystem == null) yield break;

        float distance = Vector2.Distance(transform.position, playerSystem.transform.position);
        if (distance <= stopDistance && playerHelthSystem != null)
            playerHelthSystem.Die();
        else
            enemyhelthSystem.Die();
    }

    private void DroneAttack()
    {
        if (isDead || playerSystem == null) return;

        float distance = Vector2.Distance(transform.position, playerSystem.transform.position);
        if (distance < droneDetectDistance)
        {
            Vector2 direction = (playerSystem.transform.position - transform.position).normalized;
            transform.position += (Vector3)(direction * droneMoveSpeed * Time.deltaTime);
            LongDistanceAttack();
        }
        if (distance < droneBoomDistance)
        {
            if (!isBoom) StartCoroutine(Boom());
        }
    }

    private void FlipToPlayer()
    {
        if (playerSystem == null) return;

        if (playerSystem.transform.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(
                Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(
                -Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z);
        }
    }
}