using UnityEngine;
using System.Collections;

public class EnemyChargeSystem : MonoBehaviour
{
    [Header("필수 연결")]
    public EnemySystem enemySystem;
    public EnemyHelthSystem enemyHelth;

    public enum ChargeState { Patrol, Ready, Charge, Stun, Return }
    public ChargeState currentState = ChargeState.Patrol;

    [Header("순찰 설정 (씬 뷰 초록색 선)")]
    public float patrolDistance = 5f;
    public float patrolSpeed = 2f;

    [Header("시야 설정 (씬 뷰 빨간색 박스)")]
    public Vector2 viewOffset = new Vector2(2f, 0f);
    public Vector2 viewSize = new Vector2(5f, 2f);
    public LayerMask playerLayer;

    [Header("돌진 설정 (씬 뷰 파란색 선)")]
    public float chargeReadyTime = 0.5f;
    public float chargeDistance = 7f;
    public float chargeSpeed = 22f;

    [Header("스턴 설정")]
    public float stunTime = 1f;

    [Header("상태 확인")]
    public bool isInvincible = false;

    private Rigidbody2D rb;
    private Vector2 startPos;
    private int moveDir = 1;
    private bool isCoroutineRunning = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (enemySystem == null)
            enemySystem = GetComponent<EnemySystem>();

        if (enemyHelth == null)
            enemyHelth = GetComponentInChildren<EnemyHelthSystem>();

        startPos = transform.position;
        moveDir = transform.localScale.x >= 0 ? 1 : -1;

        Debug.Log("=== ChargeEnemy 초기화 ===");
        Debug.Log("EnemySystem 연결: " + (enemySystem != null ? "OK" : "없음!"));
        Debug.Log("EnemyType: " + (enemySystem != null ? enemySystem.enemyType.ToString() : "없음!"));
        Debug.Log("PlayerLayer 값: " + playerLayer.value);
    }

    void Update()
    {
        if (enemySystem == null)
        {
            Debug.LogError("EnemySystem이 없습니다!");
            return;
        }

        if (enemySystem.enemyType != EnemyType.Charge && enemySystem.enemyType != EnemyType.Boss)
        {
            Debug.LogError("EnemyType이 Charge가 아닙니다! 현재: " + enemySystem.enemyType);
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

    void Patrol()
    {
        if (enemySystem.enemyType == EnemyType.Boss) return;

        rb.linearVelocity = new Vector2(moveDir * patrolSpeed, rb.linearVelocity.y);

        if (transform.position.x >= startPos.x + patrolDistance && moveDir == 1)
            Flip();
        else if (transform.position.x <= startPos.x - patrolDistance && moveDir == -1)
            Flip();
    }

    void ReturnToStart()
    {
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
        if (isCoroutineRunning) return;

        Vector2 boxCenter = (Vector2)transform.position
                            + new Vector2(viewOffset.x * moveDir, viewOffset.y);

        Collider2D hit = Physics2D.OverlapBox(boxCenter, viewSize, 0f, playerLayer);

        if (hit != null)
        {
            Debug.Log("플레이어 발견! 돌진 시작: " + hit.gameObject.name);
            StartCoroutine(ChargeSequence());
        }
        else
        {
            if (Time.frameCount % 300 == 0)
                Debug.Log("시야 안에 아무도 없음 (PlayerLayer=" + playerLayer.value + ")");
        }
    }

    public IEnumerator ChargeSequence()
    {
        isCoroutineRunning = true;

        currentState = ChargeState.Ready;
        rb.linearVelocity = Vector2.zero;
        Debug.Log("차징 시작! 0.5초 후 돌진");

        yield return new WaitForSeconds(chargeReadyTime);

        currentState = ChargeState.Charge;
        isInvincible = true;
        Debug.Log("돌진 시작! 무적 ON");

        float targetX = transform.position.x + (moveDir * chargeDistance);

        while (true)
        {
            rb.linearVelocity = new Vector2(moveDir * chargeSpeed, rb.linearVelocity.y);

            bool reachedTarget = (moveDir == 1 && transform.position.x >= targetX)
                              || (moveDir == -1 && transform.position.x <= targetX);

            if (reachedTarget)
            {
                Debug.Log("목표 지점 도달 → 스턴");
                break;
            }

            yield return null;
        }

        yield return StartCoroutine(StunSequence());
    }

    IEnumerator StunSequence()
    {
        currentState = ChargeState.Stun;
        isInvincible = false;
        rb.linearVelocity = Vector2.zero;
        Debug.Log("스턴! 무적 OFF. " + stunTime + "초 후 귀환");

        yield return new WaitForSeconds(stunTime);

        currentState = ChargeState.Return;
        isCoroutineRunning = false;
        Debug.Log("귀환 시작");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (currentState != ChargeState.Charge) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("플레이어 명중!");

            PlayerHelthSystem playerHelth =
                collision.gameObject.GetComponent<PlayerHelthSystem>();

            if (playerHelth == null)
                playerHelth = collision.gameObject.GetComponentInChildren<PlayerHelthSystem>();

            if (playerHelth != null)
                playerHelth.Die();

            StopAllCoroutines();
            StartCoroutine(StunSequence());
        }
        else if (!collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("벽 충돌! 스턴");
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
        Gizmos.DrawLine(
            origin + Vector2.left * patrolDistance,
            origin + Vector2.right * patrolDistance
        );

        Vector2 boxCenter = (Vector2)transform.position
                            + new Vector2(viewOffset.x * dir, viewOffset.y);
        Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
        Gizmos.DrawCube(boxCenter, viewSize);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCenter, viewSize);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(
            (Vector2)transform.position,
            (Vector2)transform.position + new Vector2(dir * chargeDistance, 0f)
        );
    }
}