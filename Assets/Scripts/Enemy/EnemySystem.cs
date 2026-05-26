using System.Collections;
using UnityEngine;

public class EnemySystem : MonoBehaviour
{
    public EnemySO enemySO;
    public string enemyName;
    public EnemyType enemyType;

    public Transform ShootPoint;
    public GameObject BulletPrefab;
    public GameObject BoomEffect;

    public PlayerMoveSystem playerMoveSystem;
    public PlayerHelthSystem playerHelthSystem;

    public float moveSpeed = 3f;
    public float stopDistance = 1.2f;
    public float shootDelay = 1f;
    public float bulletSpeed = 8f;
    public float chaseAfterExitTime = 3f;
    public float chaseDistance = 10f;

    private bool isAttack;
    private bool isDistonse;
    private bool isShoot;
    private bool isShortAttack;
    private bool isBoom;

    private Coroutine chaseStopCoroutine;

    private void Awake()
    {
        enemyName = enemySO.enemyName;
        enemyType = enemySO.enemyType;
    }

    private void Update()
    {

        if(playerHelthSystem == null)
            playerHelthSystem = FindAnyObjectByType<PlayerHelthSystem>();

        if (playerMoveSystem == null)
            playerMoveSystem = FindAnyObjectByType<PlayerMoveSystem>();
        else FlipToPlayer();

        if (isAttack)
            StartAttack();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (chaseStopCoroutine != null)
            {
                StopCoroutine(chaseStopCoroutine);
                chaseStopCoroutine = null;
            }

            isAttack = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        if (enemyType == EnemyType.Shortdistance || enemyType == EnemyType.Boom) return;

        if (chaseStopCoroutine != null) StopCoroutine(chaseStopCoroutine);

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
        switch (enemyType)
        {
            case EnemyType.Shortdistance:
                MoveToPlayer();
                break;

            case EnemyType.LongDistanc:
                LongDistanceAttack();
                break;

            case EnemyType.Boom:
                if (isBoom) return;
                MoveToPlayer();
                if (IsPlayerInAttackRange())
                    StartCoroutine(Boom());
                break;
        }
    }

    private void MoveToPlayer()
    {
        if (playerMoveSystem == null) return;

        float distance = Vector2.Distance(transform.position, playerMoveSystem.transform.position);

        if (distance <= chaseDistance)
        {
            if (distance > stopDistance)
            {
                Vector3 targetPos = new Vector3(playerMoveSystem.transform.position.x, transform.position.y, transform.position.z);

                Vector2 direction = (targetPos - transform.position).normalized;

                transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);

                isDistonse = false;
            }
            else
            {
                isDistonse = true;

                if (enemyType == EnemyType.Boom)
                {
                    if (!isBoom) StartCoroutine(Boom());
                }
                else
                {
                    if (!isShortAttack) StartCoroutine(ShortdistanceAttack());
                }
            }
        }
    }

    private IEnumerator ShortdistanceAttack()
    {
        isShortAttack = true;

        Debug.Log("적 일반 공격 애니매이션 실행");
        yield return new WaitForSeconds(1f);

        if (isDistonse && playerHelthSystem != null)
        {
            playerHelthSystem.Die();
        }

        yield return new WaitForSeconds(1f);
        isShortAttack = false;

    }

    private void LongDistanceAttack()
    {
        if (!isShoot)
            StartCoroutine(ShootBullet());
    }

    private IEnumerator ShootBullet()
    {
        isShoot = true;

        yield return new WaitForSeconds(shootDelay);

        if (BulletPrefab != null && ShootPoint != null && playerMoveSystem != null)
        {
            GameObject bullet = Instantiate(BulletPrefab, ShootPoint.position, Quaternion.identity);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                Vector2 direction = (playerMoveSystem.transform.position - ShootPoint.position).normalized;
                rb.linearVelocity = direction * bulletSpeed;
            }
        }

        isShoot = false;
    }

    private IEnumerator Boom()
    {
        isBoom = true;
        isAttack = false;

        Debug.Log("폭팔 카운트 시작 3초");
        yield return new WaitForSeconds(3f);

        if (isDistonse && playerHelthSystem != null)
        {
            Debug.Log("플레이어가 폭팔 범위 안에 있음");
            playerHelthSystem.Die();
        }
        else Debug.Log("플레이어가 폭팔 범위에서 벗어남");
            

        Destroy(gameObject);
    }

    private bool IsPlayerInAttackRange()
    {
        if (playerMoveSystem == null) return false;

        float distance = Vector2.Distance(transform.position, playerMoveSystem.transform.position);

        return distance <= stopDistance;
    }

    private void FlipToPlayer()
    {
        if (playerMoveSystem == null) return;

        if (playerMoveSystem.transform.position.x < transform.position.x)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (playerMoveSystem.transform.position.x > transform.position.x)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }
}