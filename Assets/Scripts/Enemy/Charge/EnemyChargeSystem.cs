using System.Collections;
using UnityEngine;

public class EnemyChargeSystem : MonoBehaviour
{
    [Header("Required References")]
    public EnemySystem enemySystem;
    public EnemyHelthSystem enemyHelth;

    public enum ChargeState { Patrol, Ready, Charge, Stun, Return, Dead }
    public ChargeState currentState = ChargeState.Patrol;

    [Header("Patrol Settings")]
    public float patrolDistance = 5f;
    public float patrolSpeed = 2f;

    [Header("Detection Settings")]
    public Vector2 viewOffset = new Vector2(2f, 0f);
    public Vector2 viewSize = new Vector2(5f, 2f);
    public LayerMask playerLayer;

    [Header("Charge Settings")]
    public float chargeReadyTime = 0.5f;
    public float chargeDistance = 7f;
    public float chargeSpeed = 22f;

    [Header("Stun Settings")]
    public float stunTime = 1f;

    [Header("State Check")]
    public bool isInvincible = false;

    private Rigidbody2D rb;
    private Vector2 startPos;
    private int moveDir = 1;
    private bool isCoroutineRunning = false;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (enemySystem == null) enemySystem = GetComponent<EnemySystem>();
        if (enemyHelth == null) enemyHelth = GetComponentInChildren<EnemyHelthSystem>();

        startPos = transform.position;

        moveDir = transform.localScale.x >= 0 ? 1 : -1;
    }

    void Update()
    {
        if (isDead) return;
        if (enemySystem == null) return;
        if (enemySystem.enemyType != EnemyType.Charge && enemySystem.enemyType != EnemyType.Boss) return;

        switch (currentState)
        {
            case ChargeState.Patrol:
                Patrol();
                LookForPlayer();
                break;
            case ChargeState.Return:
                ReturnToStart();
                LookForPlayer();
                break;
        }
    }

    public void OnDead()
    {
        if (isDead) return;
        isDead = true;
        currentState = ChargeState.Dead;
        StopAllCoroutines();
        if (rb != null) rb.linearVelocity = Vector2.zero;
        isInvincible = false;
        isCoroutineRunning = false;
    }

    void Patrol()
    {
        if (isDead) return;
        if (enemySystem.enemyType == EnemyType.Boss) return;

        rb.linearVelocity = new Vector2(moveDir * patrolSpeed, rb.linearVelocity.y);

        if (transform.position.x >= startPos.x + patrolDistance)
        {
            moveDir = -1;
            ApplyFlip();
        }
        else if (transform.position.x <= startPos.x - patrolDistance)
        {
            moveDir = 1;
            ApplyFlip();
        }
    }

    void ReturnToStart()
    {
        if (isDead) return;

        float dist = startPos.x - transform.position.x;
        if (Mathf.Abs(dist) < 0.15f)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            currentState = ChargeState.Patrol;
            return;
        }

        int dirToStart = dist > 0 ? 1 : -1;
        if (dirToStart != moveDir)
        {
            moveDir = dirToStart;
            ApplyFlip();
        }
        rb.linearVelocity = new Vector2(moveDir * patrolSpeed, rb.linearVelocity.y);
    }

    void LookForPlayer()
    {
        if (isDead || isCoroutineRunning) return;

        Vector2 boxCenter = (Vector2)transform.position + new Vector2(viewOffset.x * moveDir, viewOffset.y);
        Collider2D hit = Physics2D.OverlapBox(boxCenter, viewSize, 0f, playerLayer);

        if (hit != null)
        {
            float dirToPlayer = hit.transform.position.x - transform.position.x;
            int newDir = dirToPlayer >= 0 ? 1 : -1;
            if (newDir != moveDir)
            {
                moveDir = newDir;
                ApplyFlip();
            }
            StartCoroutine(ChargeSequence());
        }
    }

    public IEnumerator ChargeSequence()
    {
        isCoroutineRunning = true;
        currentState = ChargeState.Ready;
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(chargeReadyTime);
        if (isDead) { isCoroutineRunning = false; yield break; }

        currentState = ChargeState.Charge;
        isInvincible = true;

        float targetX = transform.position.x + moveDir * chargeDistance;

        while (true)
        {
            if (isDead) yield break;
            rb.linearVelocity = new Vector2(moveDir * chargeSpeed, rb.linearVelocity.y);

            bool reachedTarget = moveDir == 1
                ? transform.position.x >= targetX
                : transform.position.x <= targetX;

            if (reachedTarget) break;
            yield return null;
        }

        yield return StartCoroutine(StunSequence());
    }

    IEnumerator StunSequence()
    {
        currentState = ChargeState.Stun;
        isInvincible = false;
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(stunTime);

        if (!isDead)
        {
            currentState = ChargeState.Return;
            isCoroutineRunning = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return;
        if (currentState != ChargeState.Charge) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHelthSystem playerHelth = collision.gameObject.GetComponent<PlayerHelthSystem>();
            if (playerHelth == null)
                playerHelth = collision.gameObject.GetComponentInChildren<PlayerHelthSystem>();
            if (playerHelth == null)
                playerHelth = collision.gameObject.GetComponentInParent<PlayerHelthSystem>();

            if (playerHelth != null) playerHelth.Die();

            StopAllCoroutines();
            StartCoroutine(StunSequence());
        }
        else if (!collision.isTrigger) // º® Ãæµ¹
        {
            StopAllCoroutines();
            StartCoroutine(StunSequence());
        }
    }

    void ApplyFlip()
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * moveDir;
        transform.localScale = scale;
    }

    private void OnDrawGizmos()
    {
        Vector2 origin = Application.isPlaying ? startPos : (Vector2)transform.position;
        float dir = Application.isPlaying ? moveDir : (transform.localScale.x >= 0 ? 1 : -1);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(origin + Vector2.left * patrolDistance,
                        origin + Vector2.right * patrolDistance);

        Vector2 boxCenter = (Vector2)transform.position + new Vector2(viewOffset.x * dir, viewOffset.y);
        Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
        Gizmos.DrawCube(boxCenter, viewSize);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCenter, viewSize);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine((Vector2)transform.position,
            (Vector2)transform.position + new Vector2(dir * chargeDistance, 0f));
    }
}