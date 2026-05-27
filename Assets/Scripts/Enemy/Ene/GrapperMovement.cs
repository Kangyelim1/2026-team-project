using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(CapsuleCollider2D))]
public class GrapperMovement : MonoBehaviour
{
    [Header("적 이름")]
    public string enemyType = "Grapper";

    [Header("이동")]
    public float moveSpeed = 4f;
    public float moveRadius = 3f;
    public float trunDuration = 0.3f;
    public float fastMoveDuration = 2f;
    public float fastSpeedMultiplier = 1.5f;

    [Header("벽 감지")]
    public Transform wallCheckPos;
    public float layerCheckRadius = 0.05f;

    [Header("시야 범위")]
    public Vector2 viewOffset = new Vector2(0.5f, 0f);
    public Vector2 viewSize = new Vector2(5f, 3f);

    [Header("공격 판정")]
    public Vector2 hitboxOffset = new Vector2(0.5f, 0f);
    public Vector2 hitboxSize = new Vector2(1f, 1f);
    public LayerMask playerLayer;

    [Header("무적 사운드")]
    public AudioClip invincibleHitSound;

    [Header("죽음")]
    public float deathDuration = 1.5f;
    public float fallingOutPower = 12f;

    [Header("레이어")]
    public LayerMask afterDeathLayer;
    public LayerMask obstacleMask;

    public enum State { Move, Turn, Attack, FastMove }
    private State currentState;

    private bool isGoingRight = true;
    private bool isDead = false;
    public bool isAttacking = false;
    private int facingSign = 1;
    private float fastMoveTimer = 0f;

    private Vector3 movePosRight;
    private Vector3 movePosLeft;
    private Vector3 targetPos;

    private Coroutine turnCoroutine;
    private Coroutine attackCoroutine;

    private Rigidbody2D rb;
    private Animator anim;
    private CapsuleCollider2D capsuleCol;
    private AudioSource audioSource;

    private GameObject playerObject;
    private PlayerHelthSystem playerHelthSystem;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        capsuleCol = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        PlayerSystem player = FindAnyObjectByType<PlayerSystem>();
        if (player != null)
        {
            playerObject = player.gameObject;
            playerHelthSystem = FindAnyObjectByType<PlayerHelthSystem>();
        }

        if (moveRadius < 0) moveRadius *= -1f;

        movePosRight = movePosLeft = transform.position;
        movePosRight.x += moveRadius;
        movePosLeft.x -= moveRadius;

        targetPos = movePosRight;
        isGoingRight = true;
        isDead = false;

        SetState(State.Move);
    }

    private void Update()
    {
        if (isDead) return;
        if (playerObject == null) return;

        switch (currentState)
        {
            case State.Move:
                MoveHandler(moveSpeed);
                CheckPlayerInView();
                break;

            case State.FastMove:
                FastMoveHandler();
                break;
        }

        if (isAttacking) AttackCheck();

        UpdateAnimation();
    }

    private void SetState(State target)
    {
        currentState = target;
        rb.linearVelocity = Vector2.zero;

        switch (target)
        {
            case State.Move:
                isAttacking = false;
                break;

            case State.Turn:
                isAttacking = false;
                if (turnCoroutine != null) StopCoroutine(turnCoroutine);
                turnCoroutine = StartCoroutine(WaitToTurn());
                break;

            case State.Attack:
                isAttacking = false;
                if (attackCoroutine != null) StopCoroutine(attackCoroutine);
                attackCoroutine = StartCoroutine(AttackStep());
                break;

            case State.FastMove:
                isAttacking = false;
                fastMoveTimer = 0f;
                break;
        }
    }

    private void MoveHandler(float speed)
    {
        if (wallCheckPos != null)
        {
            bool hitWall = Physics2D.OverlapCircle(wallCheckPos.position, layerCheckRadius, obstacleMask);
            if (hitWall)
            {
                SetState(State.Turn);
                return;
            }
        }

        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

        if (Vector3.Distance(transform.position, targetPos) < 0.05f)
            SetState(State.Turn);
    }

    private void FastMoveHandler()
    {
        fastMoveTimer += Time.deltaTime;
        MoveHandler(moveSpeed * fastSpeedMultiplier);

        if (fastMoveTimer >= fastMoveDuration)
        {
            SetState(IsPlayerInView() ? State.Attack : State.Move);
        }
    }

    private IEnumerator WaitToTurn()
    {
        anim.SetTrigger("turn");
        yield return new WaitForSeconds(trunDuration);

        isGoingRight = !isGoingRight;
        targetPos = isGoingRight ? movePosRight : movePosLeft;
        Flip();

        turnCoroutine = null;
        SetState(State.Move);
    }

    private IEnumerator AttackStep()
    {
        anim.SetTrigger("startAttack");
        isAttacking = true;

        yield return new WaitForSeconds(3f);

        isAttacking = false;
        anim.SetTrigger("endAttack");
        attackCoroutine = null;

        SetState(State.FastMove);
    }

    private void CheckPlayerInView()
    {
        if (IsPlayerInView())
            SetState(State.Attack);
    }

    private void AttackCheck()
    {
        float offsetX = hitboxOffset.x * facingSign;
        Vector2 worldCenter = (Vector2)transform.position + new Vector2(offsetX, hitboxOffset.y);

        Collider2D[] hits = Physics2D.OverlapBoxAll(worldCenter, hitboxSize, 0f, playerLayer);
        foreach (Collider2D col in hits)
        {
            if (!col.CompareTag("Player")) continue;
            if (playerHelthSystem != null)
            {
                playerHelthSystem.Die();
                return;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            if (other.TryGetComponent(out BulletSystem bullet))
            {
                if (bullet.type == BulletType.PlayerBullet)
                {
                    if (isAttacking)
                    {
                        if (invincibleHitSound != null && audioSource != null)
                            audioSource.PlayOneShot(invincibleHitSound);

                        Destroy(other.gameObject);
                        return;
                    }
                    Destroy(other.gameObject);
                    Die();
                }
            }
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        rb.gravityScale = 1f;
        rb.freezeRotation = false;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(Vector2.up * fallingOutPower, ForceMode2D.Impulse);
        rb.AddTorque(Random.Range(-20f, 20f));

        capsuleCol.excludeLayers = afterDeathLayer;

        anim.SetTrigger("die");
        StopAllCoroutines();
        StartCoroutine(Dead());
    }

    private IEnumerator Dead()
    {
        float timer = 0f;
        Vector3 initScale = transform.localScale;
        Vector3 targetScale = Vector3.zero;

        while (timer < deathDuration)
        {
            timer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(initScale, targetScale, timer / deathDuration);
            yield return null;
        }
        Destroy(gameObject);
    }

    private void Flip()
    {
        facingSign = isGoingRight ? 1 : -1;
        transform.localScale = new Vector3(
            Mathf.Abs(transform.localScale.x) * facingSign,
            transform.localScale.y,
            transform.localScale.z
        );
    }

    private void UpdateAnimation()
    {
        bool moving = (currentState == State.Move || currentState == State.FastMove);
        anim.SetBool("isMoving", moving);
        anim.SetBool("isAttacking", isAttacking);
    }

    private bool IsPlayerInView()
    {
        if (playerObject == null) return false;

        Vector2 offset = new Vector2(viewOffset.x * facingSign, viewOffset.y);
        Vector2 worldCenter = (Vector2)transform.position + offset;

        Collider2D[] hits = Physics2D.OverlapBoxAll(worldCenter, viewSize, 0f, playerLayer);
        bool found = false;
        foreach (Collider2D col in hits)
        {
            if (col.CompareTag("Player")) { found = true; break; }
        }
        if (!found) return false;

        Vector2 start = transform.position;
        Vector2 end = playerObject.transform.position;
        Vector2 direction = (end - start).normalized;
        float distance = Vector2.Distance(start, end);

        RaycastHit2D hit = Physics2D.Raycast(start, direction, distance, obstacleMask);
        return hit.collider == null;
    }

    private void OnDrawGizmosSelected()
    {
        int sign = Application.isPlaying ? facingSign : 1;

        Gizmos.color = Color.blue;
        Vector2 vCenter = (Vector2)transform.position + new Vector2(viewOffset.x * sign, viewOffset.y);
        Gizmos.DrawWireCube(vCenter, new Vector3(viewSize.x, viewSize.y, 0f));

        Gizmos.color = Color.red;
        Vector2 hCenter = (Vector2)transform.position + new Vector2(hitboxOffset.x * sign, hitboxOffset.y);
        Gizmos.DrawWireCube(hCenter, new Vector3(hitboxSize.x, hitboxSize.y, 0f));

        Gizmos.color = Color.cyan;
        if (Application.isPlaying)
        {
            Gizmos.DrawWireSphere(movePosRight, 0.2f);
            Gizmos.DrawWireSphere(movePosLeft, 0.2f);
            Gizmos.DrawLine(movePosRight, movePosLeft);
        }
        else
        {
            Vector3 r = transform.position; r.x += moveRadius;
            Vector3 l = transform.position; l.x -= moveRadius;
            Gizmos.DrawWireSphere(r, 0.2f);
            Gizmos.DrawWireSphere(l, 0.2f);
            Gizmos.DrawLine(r, l);
        }

        if (wallCheckPos != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(wallCheckPos.position, layerCheckRadius);
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (isDead) return;

        if (other.CompareTag("Player"))
        {
            if (playerHelthSystem != null)
                playerHelthSystem.Die();
        }
    }
}