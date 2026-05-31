using UnityEngine;
using System.Collections;

public class EnemyShieldSystem : MonoBehaviour
{
    [Header("필수 연결")]
    public EnemySystem enemySystem;

    public enum ShieldState { Patrol, Track, Attack, Stun }
    public ShieldState currentState = ShieldState.Patrol;

    [Header("순찰 설정 (씬 뷰 초록색 선)")]
    public float patrolDistance = 5f;
    public float patrolSpeed = 2f;

    [Header("추적 설정")]
    public float trackSpeed = 3.5f;
    public float stopDistance = 1.2f; 

    [Header("시야 설정 (씬 뷰 파란색 박스)")]
    public Vector2 viewOffset = new Vector2(2f, 0f);
    public Vector2 viewSize = new Vector2(6f, 2.5f);
    public LayerMask playerLayer;

    [Header("공격 설정 (씬 뷰 빨간색 박스)")]
    public Vector2 attackOffset = new Vector2(0.8f, 0f);
    public Vector2 attackSize = new Vector2(1.2f, 1.5f);
    public float attackDelay = 0.4f;    
    public float attackCooldown = 1.0f; 

    [Header("방패 무적 설정 (씬 뷰 노란색 박스)")]
    public Vector2 shieldOffset = new Vector2(0.6f, 0f);
    public Vector2 shieldSize = new Vector2(0.8f, 2f);

    [Header("머리 피격 영역 (씬 뷰 보라색 박스)")]
    public Vector2 headOffset = new Vector2(0f, 0.8f);
    public Vector2 headSize = new Vector2(0.8f, 0.5f);

    [Header("스턴 설정")]
    public float stunTime = 0.5f;

    private Rigidbody2D rb;
    private Vector2 startPos;
    private int moveDir = 1;
    private bool isAttacking = false;
    private bool isCoroutineRunning = false;

    private PlayerSystem playerSystem;
    private PlayerHelthSystem playerHelthSystem;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (enemySystem == null)
            enemySystem = GetComponent<EnemySystem>();

        startPos = transform.position;
        moveDir = transform.localScale.x >= 0 ? 1 : -1;
    }

    void Update()
    {
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
        rb.linearVelocity = new Vector2(moveDir * patrolSpeed, rb.linearVelocity.y);

        if (transform.position.x >= startPos.x + patrolDistance && moveDir == 1)
            Flip();
        else if (transform.position.x <= startPos.x - patrolDistance && moveDir == -1)
            Flip();
    }

    void LookForPlayer()
    {
        if (isCoroutineRunning) return;

        Vector2 center = (Vector2)transform.position
                         + new Vector2(viewOffset.x * moveDir, viewOffset.y);
        Collider2D hit = Physics2D.OverlapBox(center, viewSize, 0f, playerLayer);

        if (hit != null)
        {
            Debug.Log("방패병: 플레이어 발견! 추적 시작");
            currentState = ShieldState.Track;
        }
    }

    void TrackPlayer()
    {
        if (playerSystem == null) return;
        if (isAttacking) return;

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
            Debug.Log("방패병: 플레이어를 놓침. 순찰 복귀");
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

        Debug.Log("방패병: 공격 모션 시작");
        yield return new WaitForSeconds(attackDelay);

        Vector2 attackCenter = (Vector2)transform.position
                               + new Vector2(attackOffset.x * moveDir, attackOffset.y);
        Collider2D hit = Physics2D.OverlapBox(attackCenter, attackSize, 0f, playerLayer);

        if (hit != null && hit.CompareTag("Player"))
        {
            Debug.Log("방패병: 플레이어 공격 적중!");
            if (playerHelthSystem != null)
                playerHelthSystem.Die();
        }

        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
        isCoroutineRunning = false;
    }

    void FacePlayer()
    {
        if (playerSystem == null) return;

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
    }

    public bool TryTakeDamage(Vector2 bulletHitPos)
    {
        Vector2 headCenter = (Vector2)transform.position
                             + new Vector2(headOffset.x * moveDir, headOffset.y);
        bool hitHead = IsInsideBox(bulletHitPos, headCenter, headSize);

        if (hitHead)
        {
            Debug.Log("방패병: 머리 피격! 사망");
            return true; 
        }

        Vector2 shieldCenter = (Vector2)transform.position
                               + new Vector2(shieldOffset.x * moveDir, shieldOffset.y);
        bool hitShield = IsInsideBox(bulletHitPos, shieldCenter, shieldSize);

        if (hitShield)
        {
            Debug.Log("방패병: 방패 막기! 무적");
            StartCoroutine(ShieldBlockStun()); 
            return false;
        }

        Debug.Log("방패병: 뒤쪽/몸통 피격! 사망");
        return true; 
    }

    IEnumerator ShieldBlockStun()
    {
        currentState = ShieldState.Stun;
        rb.linearVelocity = Vector2.zero;
        Debug.Log("방패병: 방패 막기 성공! 잠깐 경직");

        yield return new WaitForSeconds(stunTime);

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