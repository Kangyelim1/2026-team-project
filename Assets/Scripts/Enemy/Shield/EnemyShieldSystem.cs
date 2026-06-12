using UnityEngine;
using System.Collections;

public class EnemyShieldSystem : MonoBehaviour
{
    [Header("Required References")]
    public EnemySystem enemySystem;

    public enum ShieldState { Patrol, Track, Attack, Stun, Dead }
    public ShieldState currentState = ShieldState.Patrol;

    [Header("Patrol Settings")]
    public float patrolDistance = 5f;
    public float patrolSpeed = 2f;

    [Header("Track Settings")]
    public float trackSpeed = 3.5f;
    public float stopDistance = 1.2f;

    [Header("Detection Settings")]
    public Vector2 viewOffset = new Vector2(2f, 0f);
    public Vector2 viewSize = new Vector2(6f, 2.5f);
    public LayerMask playerLayer;

    [Header("Attack Settings")]
    public Vector2 attackOffset = new Vector2(0.8f, 0f);
    public Vector2 attackSize = new Vector2(1.2f, 1.5f);
    public float attackDelay = 0.4f;
    public float attackCooldown = 1.0f;

    [Header("Shield Settings")]
    public Vector2 shieldOffset = new Vector2(0.6f, 0f);
    public Vector2 shieldSize = new Vector2(0.8f, 2f);

    [Header("Head Hitbox Settings")]
    public Vector2 headOffset = new Vector2(0f, 0.8f);
    public Vector2 headSize = new Vector2(0.8f, 0.5f);

    [Header("Stun Settings")]
    public float stunTime = 0.5f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 startPos;
    private int moveDir = 1;
    public int MoveDir => moveDir;
    private bool isAttacking = false;
    private bool isCoroutineRunning = false;
    private bool isDead = false;

    private PlayerSystem playerSystem;
    private PlayerHelthSystem playerHelthSystem;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (enemySystem == null)
            enemySystem = GetComponent<EnemySystem>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        startPos = transform.position;
        moveDir = transform.localScale.x >= 0 ? 1 : -1;

        ApplyFlip();
    }

    public void OnDead()
    {
        if (isDead) return;
        isDead = true;

        currentState = ShieldState.Dead;
        StopAllCoroutines();

        if (rb != null) rb.linearVelocity = Vector2.zero;

        isAttacking = false;
        isCoroutineRunning = false;

        Debug.Log("EnemyShieldSystem: Dead - all actions stopped");
    }

    void Update()
    {
        if (isDead) return;     

        if (playerSystem == null)
            playerSystem = FindAnyObjectByType<PlayerSystem>();

        if (playerHelthSystem == null)
            playerHelthSystem = FindAnyObjectByType<PlayerHelthSystem>();

        if (enemySystem == null || enemySystem.enemyType != EnemyType.Shield) return;

        switch (currentState)
        {
            case ShieldState.Patrol:
                Patrol();
                LookForPlayer();
                break;

            case ShieldState.Track:
                TrackPlayer();
                break;
        }
    }

    void Patrol()
    {
        if (isDead) return;

        rb.linearVelocity = new Vector2(moveDir * patrolSpeed, rb.linearVelocity.y);

        if (transform.position.x >= startPos.x + patrolDistance && moveDir == 1)
            Flip();
        else if (transform.position.x <= startPos.x - patrolDistance && moveDir == -1)
            Flip();
    }

    void LookForPlayer()
    {
        if (isDead || isCoroutineRunning) return;

        Vector2 center = (Vector2)transform.position
                         + new Vector2(viewOffset.x * moveDir, viewOffset.y);
        Collider2D hit = Physics2D.OverlapBox(center, viewSize, 0f, playerLayer);

        if (hit != null)
        {
            Debug.Log("Shield: Player detected! Start tracking");
            currentState = ShieldState.Track;
        }
    }

    void TrackPlayer()
    {
        if (isDead || playerSystem == null || isAttacking) return;

        FacePlayer();

        float dist = Vector2.Distance(transform.position, playerSystem.transform.position);

        if (dist <= stopDistance)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            if (!isCoroutineRunning)
                StartCoroutine(AttackRoutine());
            return;
        }

        Vector2 center = (Vector2)transform.position
                         + new Vector2(viewOffset.x * moveDir, viewOffset.y);
        Collider2D hit = Physics2D.OverlapBox(center, viewSize, 0f, playerLayer);

        if (hit == null)
        {
            Debug.Log("Shield: Lost player. Return to patrol");
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            currentState = ShieldState.Patrol;
            return;
        }

        rb.linearVelocity = new Vector2(moveDir * trackSpeed, rb.linearVelocity.y);
    }

    IEnumerator AttackRoutine()
    {
        isCoroutineRunning = true;
        isAttacking = true;

        Debug.Log("Shield: Attack start");
        yield return new WaitForSeconds(attackDelay);

        if (isDead)
        {
            isAttacking = false;
            isCoroutineRunning = false;
            yield break;
        }

        Vector2 attackCenter = (Vector2)transform.position
                               + new Vector2(attackOffset.x * moveDir, attackOffset.y);
        Collider2D hit = Physics2D.OverlapBox(attackCenter, attackSize, 0f, playerLayer);

        if (hit != null)
        {
            PlayerHelthSystem ph = hit.GetComponent<PlayerHelthSystem>();
            if (ph == null) ph = hit.GetComponentInParent<PlayerHelthSystem>();

            if (ph != null)
            {
                Debug.Log("Shield: Player hit!");
                ph.Die();
            }
        }

        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
        isCoroutineRunning = false;
    }

    void FacePlayer()
    {
        if (isDead || playerSystem == null) return;

        float dirX = playerSystem.transform.position.x - transform.position.x;
        int newDir = dirX > 0 ? 1 : -1;

        if (newDir != moveDir)
            Flip();
    }

    void Flip()
    {
        moveDir *= -1;
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * moveDir;
        transform.localScale = scale;
        ApplyFlip();
    }

    void ApplyFlip()
    {
        if (spriteRenderer != null)
            spriteRenderer.flipX = (moveDir == -1);
    }

    public bool TryTakeDamage(Vector2 bulletHitPos)
    {
        if (isDead) return false;

        Vector2 headCenter = (Vector2)transform.position
                             + new Vector2(headOffset.x * moveDir, headOffset.y);
        bool hitHead = IsInsideBox(bulletHitPos, headCenter, headSize);

        if (hitHead)
        {
            Debug.Log("Shield: Head hit! Dead");
            return true;
        }

        Vector2 shieldCenter = (Vector2)transform.position
                               + new Vector2(shieldOffset.x * moveDir, shieldOffset.y);
        bool hitShield = IsInsideBox(bulletHitPos, shieldCenter, shieldSize);

        if (hitShield)
        {
            Debug.Log("Shield: Blocked by shield! Invincible");
            StartCoroutine(ShieldBlockStun());
            return false;
        }

        Debug.Log("Shield: Back/body hit! Dead");
        return true;
    }

    IEnumerator ShieldBlockStun()
    {
        currentState = ShieldState.Stun;
        rb.linearVelocity = Vector2.zero;
        Debug.Log("Shield: Block success! Brief stagger");

        yield return new WaitForSeconds(stunTime);

        if (!isDead)
            currentState = ShieldState.Track;
    }

    bool IsInsideBox(Vector2 point, Vector2 boxCenter, Vector2 boxSize)
    {
        return point.x >= boxCenter.x - boxSize.x / 2f &&
               point.x <= boxCenter.x + boxSize.x / 2f &&
               point.y >= boxCenter.y - boxSize.y / 2f &&
               point.y <= boxCenter.y + boxSize.y / 2f;
    }

    private void OnDrawGizmos()
    {
        float dir = Application.isPlaying ? moveDir : (transform.localScale.x >= 0 ? 1 : -1);
        Vector2 pos = transform.position;
        Vector2 sPos = Application.isPlaying ? startPos : pos;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(sPos + Vector2.left * patrolDistance,
                        sPos + Vector2.right * patrolDistance);

        Vector2 viewCenter = pos + new Vector2(viewOffset.x * dir, viewOffset.y);
        Gizmos.color = new Color(0f, 0.5f, 1f, 0.2f);
        Gizmos.DrawCube(viewCenter, viewSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(viewCenter, viewSize);

        Vector2 attackCenter = pos + new Vector2(attackOffset.x * dir, attackOffset.y);
        Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
        Gizmos.DrawCube(attackCenter, attackSize);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackCenter, attackSize);

        Vector2 shieldCenter = pos + new Vector2(shieldOffset.x * dir, shieldOffset.y);
        Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
        Gizmos.DrawCube(shieldCenter, shieldSize);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(shieldCenter, shieldSize);

        Vector2 headCenter = pos + new Vector2(headOffset.x * dir, headOffset.y);
        Gizmos.color = new Color(0.8f, 0f, 1f, 0.3f);
        Gizmos.DrawCube(headCenter, headSize);
        Gizmos.color = new Color(0.8f, 0f, 1f);
        Gizmos.DrawWireCube(headCenter, headSize);
    }
}