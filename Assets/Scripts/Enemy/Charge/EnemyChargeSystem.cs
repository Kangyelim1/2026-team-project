using UnityEngine;
using System.Collections;

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

        if (enemySystem == null)
            enemySystem = GetComponent<EnemySystem>();

        if (enemyHelth == null)
            enemyHelth = GetComponentInChildren<EnemyHelthSystem>();

        startPos = transform.position;
        moveDir = transform.localScale.x >= 0 ? 1 : -1;

        Debug.Log("=== ChargeEnemy Init ===");
        Debug.Log("EnemySystem: " + (enemySystem != null ? "OK" : "Missing!"));
        Debug.Log("EnemyType: " + (enemySystem != null ? enemySystem.enemyType.ToString() : "Missing!"));
        Debug.Log("PlayerLayer: " + playerLayer.value);
    }

    void Update()
    {
        if (isDead) return;

        if (enemySystem == null)
        {
            Debug.LogError("EnemySystem is missing!");
            return;
        }

        if (enemySystem.enemyType != EnemyType.Charge && enemySystem.enemyType != EnemyType.Boss)
        {
            Debug.LogError("EnemyType is not Charge! Current: " + enemySystem.enemyType);
            return;
        }

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

        Debug.Log("EnemyChargeSystem: Dead - all actions stopped");
    }

    void Patrol()
    {
        if (isDead) return;
        if (enemySystem.enemyType == EnemyType.Boss) return;

        rb.linearVelocity = new Vector2(moveDir * patrolSpeed, rb.linearVelocity.y);

        if (transform.position.x >= startPos.x + patrolDistance && moveDir == 1)
            Flip();
        else if (transform.position.x <= startPos.x - patrolDistance && moveDir == -1)
            Flip();
    }

    void ReturnToStart()
    {
        if (isDead) return;

        float dist = startPos.x - transform.position.x;

        if (Mathf.Abs(dist) <= 0.15f)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            currentState = ChargeState.Patrol;
            return;
        }

        int dirToStart = dist > 0 ? 1 : -1;
        if (moveDir != dirToStart) Flip();

        rb.linearVelocity = new Vector2(moveDir * patrolSpeed, rb.linearVelocity.y);
    }

    void LookForPlayer()
    {
        if (isDead || isCoroutineRunning) return;

        Vector2 boxCenter = (Vector2)transform.position
                            + new Vector2(viewOffset.x * moveDir, viewOffset.y);

        Collider2D hit = Physics2D.OverlapBox(boxCenter, viewSize, 0f, playerLayer);

        if (hit != null)
        {
            Debug.Log("Player detected! Start charge: " + hit.gameObject.name);
            StartCoroutine(ChargeSequence());
        }
        else
        {
            if (Time.frameCount % 300 == 0)
                Debug.Log("No player in range (PlayerLayer=" + playerLayer.value + ")");
        }
    }

    public IEnumerator ChargeSequence()
    {
        isCoroutineRunning = true;
        currentState = ChargeState.Ready;
        rb.linearVelocity = Vector2.zero;
        Debug.Log("Charge ready! Charging after " + chargeReadyTime + "s");

        yield return new WaitForSeconds(chargeReadyTime);

        if (isDead)
        {
            isCoroutineRunning = false;
            yield break;
        }

        currentState = ChargeState.Charge;
        isInvincible = true;
        Debug.Log("Charge start! Invincible ON");

        float targetX = transform.position.x + (moveDir * chargeDistance);

        while (true)
        {
            if (isDead) yield break;

            rb.linearVelocity = new Vector2(moveDir * chargeSpeed, rb.linearVelocity.y);

            bool reachedTarget = (moveDir == 1 && transform.position.x >= targetX)
                              || (moveDir == -1 && transform.position.x <= targetX);

            if (reachedTarget)
            {
                Debug.Log("Reached target -> Stun");
                break;
            }

            yield return null;
        }

        yield return StartCoroutine(StunSequence());
    }
    //어떤 겜을 만들어야 하누 답이 없구만
    IEnumerator StunSequence()
    {
        currentState = ChargeState.Stun;
        isInvincible = false;
        rb.linearVelocity = Vector2.zero;
        Debug.Log("Stun! Invincible OFF. Return after " + stunTime + "s");

        yield return new WaitForSeconds(stunTime);

        if (!isDead)
        {
            currentState = ChargeState.Return;
            isCoroutineRunning = false;
            Debug.Log("Return start");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;
        if (currentState != ChargeState.Charge) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player hit!");

            PlayerHelthSystem playerHelth = collision.gameObject.GetComponent<PlayerHelthSystem>();
            if (playerHelth == null)
                playerHelth = collision.gameObject.GetComponentInChildren<PlayerHelthSystem>();

            if (playerHelth != null)
                playerHelth.Die();

            StopAllCoroutines();
            StartCoroutine(StunSequence());
        }
        else
        {
            Debug.Log("Wall collision! Stun");
            StopAllCoroutines();
            StartCoroutine(StunSequence());
        }
    }

    void Flip()
    {
        moveDir *= -1;
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * moveDir;
        transform.localScale = scale;
    }

    private void OnDrawGizmos()
    {
        Vector2 origin = Application.isPlaying ? startPos : (Vector2)transform.position;
        float dir = transform.localScale.x >= 0 ? 1 : -1;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(origin + Vector2.left * patrolDistance,
                        origin + Vector2.right * patrolDistance);

        Vector2 boxCenter = (Vector2)transform.position
                            + new Vector2(viewOffset.x * dir, viewOffset.y);
        Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
        Gizmos.DrawCube(boxCenter, viewSize);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCenter, viewSize);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine((Vector2)transform.position,
                        (Vector2)transform.position + new Vector2(dir * chargeDistance, 0f));
    }
}