using UnityEngine;
using System.Collections;

public class EnemyShieldSystem : MonoBehaviour
{
    // =====================================================
    // 외부 참조
    // =====================================================
    [Header("필수 연결")]
    public EnemySystem enemySystem;

    // =====================================================
    // 상태 머신
    // =====================================================
    public enum ShieldState { Patrol, Track, Attack, Stun }
    public ShieldState currentState = ShieldState.Patrol;

    // =====================================================
    // 순찰
    // =====================================================
    [Header("순찰 설정 (씬 뷰 초록색 선)")]
    public float patrolDistance = 5f;
    public float patrolSpeed = 2f;

    // =====================================================
    // 추적
    // =====================================================
    [Header("추적 설정")]
    public float trackSpeed = 3.5f;
    public float stopDistance = 1.2f;   // 이 거리 안에 들어오면 공격

    // =====================================================
    // 시야
    // =====================================================
    [Header("시야 설정 (씬 뷰 파란색 박스)")]
    public Vector2 viewOffset = new Vector2(2f, 0f);
    public Vector2 viewSize = new Vector2(6f, 2.5f);
    public LayerMask playerLayer;

    // =====================================================
    // 공격
    // =====================================================
    [Header("공격 설정 (씬 뷰 빨간색 박스)")]
    public Vector2 attackOffset = new Vector2(0.8f, 0f);
    public Vector2 attackSize = new Vector2(1.2f, 1.5f);
    public float attackDelay = 0.4f;    // 공격 모션 후 실제 판정까지 딜레이
    public float attackCooldown = 1.0f; // 다음 공격까지 대기 시간

    // =====================================================
    // 피격 무효화 영역
    // =====================================================
    [Header("방패 무적 설정 (씬 뷰 노란색 박스)")]
    public Vector2 shieldOffset = new Vector2(0.6f, 0f);
    public Vector2 shieldSize = new Vector2(0.8f, 2f);

    // =====================================================
    // 머리 피격 판정
    // =====================================================
    [Header("머리 피격 영역 (씬 뷰 보라색 박스)")]
    public Vector2 headOffset = new Vector2(0f, 0.8f);
    public Vector2 headSize = new Vector2(0.8f, 0.5f);

    // =====================================================
    // 넉백 & 스턴
    // =====================================================
    [Header("스턴 설정")]
    public float stunTime = 0.5f;

    // =====================================================
    // 내부 변수
    // =====================================================
    private Rigidbody2D rb;
    private Vector2 startPos;
    private int moveDir = 1;
    private bool isAttacking = false;
    private bool isCoroutineRunning = false;

    private PlayerSystem playerSystem;
    private PlayerHelthSystem playerHelthSystem;

    // =====================================================
    // 초기화
    // =====================================================
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
        // 플레이어 자동 탐색
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

    // =====================================================
    // 순찰
    // =====================================================
    void Patrol()
    {
        rb.linearVelocity = new Vector2(moveDir * patrolSpeed, rb.linearVelocity.y);

        if (transform.position.x >= startPos.x + patrolDistance && moveDir == 1)
            Flip();
        else if (transform.position.x <= startPos.x - patrolDistance && moveDir == -1)
            Flip();
    }

    // =====================================================
    // 플레이어 시야 감지
    // =====================================================
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

    // =====================================================
    // 플레이어 추적
    // =====================================================
    void TrackPlayer()
    {
        if (playerSystem == null) return;
        if (isAttacking) return;

        // 플레이어 방향으로 바라보기
        FacePlayer();

        float dist = Vector2.Distance(transform.position, playerSystem.transform.position);

        // 공격 사거리 안에 들어오면 공격
        if (dist <= stopDistance)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            if (!isCoroutineRunning)
                StartCoroutine(AttackRoutine());
            return;
        }

        // 시야를 벗어나면 순찰로 복귀
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

        // 플레이어를 향해 이동
        rb.linearVelocity = new Vector2(moveDir * trackSpeed, rb.linearVelocity.y);
    }

    // =====================================================
    // 공격
    // =====================================================
    IEnumerator AttackRoutine()
    {
        isCoroutineRunning = true;
        isAttacking = true;

        Debug.Log("방패병: 공격 모션 시작");
        yield return new WaitForSeconds(attackDelay);

        // 공격 판정
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

    // =====================================================
    // 플레이어 쪽 바라보기
    // =====================================================
    void FacePlayer()
    {
        if (playerSystem == null) return;

        float dirX = playerSystem.transform.position.x - transform.position.x;
        int newDir = dirX > 0 ? 1 : -1;

        if (newDir != moveDir)
            Flip();
    }

    // =====================================================
    // 방향 전환
    // =====================================================
    void Flip()
    {
        moveDir *= -1;
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * moveDir;
        transform.localScale = scale;
    }

    // =====================================================
    // ★ 핵심: 피격 판정 (방패병 전용)
    // 정면 방패 → 무적
    // 머리 or 뒤쪽 → 사망
    // =====================================================
    public bool TryTakeDamage(Vector2 bulletHitPos)
    {
        // 1. 머리를 맞았는지 확인
        Vector2 headCenter = (Vector2)transform.position
                             + new Vector2(headOffset.x * moveDir, headOffset.y);
        bool hitHead = IsInsideBox(bulletHitPos, headCenter, headSize);

        if (hitHead)
        {
            Debug.Log("방패병: 머리 피격! 사망");
            return true; // 데미지 허용
        }

        // 2. 방패(정면)에 맞았는지 확인
        Vector2 shieldCenter = (Vector2)transform.position
                               + new Vector2(shieldOffset.x * moveDir, shieldOffset.y);
        bool hitShield = IsInsideBox(bulletHitPos, shieldCenter, shieldSize);

        if (hitShield)
        {
            Debug.Log("방패병: 방패 막기! 무적");
            StartCoroutine(ShieldBlockStun()); // 방패로 막으면 잠깐 스턴
            return false; // 데미지 거부
        }

        // 3. 뒤쪽 or 몸통을 맞음
        Debug.Log("방패병: 뒤쪽/몸통 피격! 사망");
        return true; // 데미지 허용
    }

    IEnumerator ShieldBlockStun()
    {
        currentState = ShieldState.Stun;
        rb.linearVelocity = Vector2.zero;
        Debug.Log("방패병: 방패 막기 성공! 잠깐 경직");

        yield return new WaitForSeconds(stunTime);

        currentState = ShieldState.Track;
    }

    // =====================================================
    // 유틸: 특정 위치가 박스 안에 있는지 체크
    // =====================================================
    bool IsInsideBox(Vector2 point, Vector2 boxCenter, Vector2 boxSize)
    {
        return point.x >= boxCenter.x - boxSize.x / 2f &&
               point.x <= boxCenter.x + boxSize.x / 2f &&
               point.y >= boxCenter.y - boxSize.y / 2f &&
               point.y <= boxCenter.y + boxSize.y / 2f;
    }

    // =====================================================
    // 씬 뷰 시각화
    // =====================================================
    private void OnDrawGizmos()
    {
        float dir = Application.isPlaying ? moveDir : (transform.localScale.x >= 0 ? 1 : -1);
        Vector2 pos = transform.position;
        Vector2 sPos = Application.isPlaying ? startPos : pos;

        // 초록색: 순찰 거리
        Gizmos.color = Color.green;
        Gizmos.DrawLine(sPos + Vector2.left * patrolDistance,
                        sPos + Vector2.right * patrolDistance);

        // 파란색: 시야 박스
        Vector2 viewCenter = pos + new Vector2(viewOffset.x * dir, viewOffset.y);
        Gizmos.color = new Color(0f, 0.5f, 1f, 0.2f);
        Gizmos.DrawCube(viewCenter, viewSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(viewCenter, viewSize);

        // 빨간색: 공격 판정
        Vector2 attackCenter = pos + new Vector2(attackOffset.x * dir, attackOffset.y);
        Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
        Gizmos.DrawCube(attackCenter, attackSize);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackCenter, attackSize);

        // 노란색: 방패 무적 영역
        Vector2 shieldCenter = pos + new Vector2(shieldOffset.x * dir, shieldOffset.y);
        Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
        Gizmos.DrawCube(shieldCenter, shieldSize);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(shieldCenter, shieldSize);

        // 보라색: 머리 피격 영역
        Vector2 headCenter = pos + new Vector2(headOffset.x * dir, headOffset.y);
        Gizmos.color = new Color(0.8f, 0f, 1f, 0.3f);
        Gizmos.DrawCube(headCenter, headSize);
        Gizmos.color = new Color(0.8f, 0f, 1f);
        Gizmos.DrawWireCube(headCenter, headSize);
    }
}