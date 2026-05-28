using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(CapsuleCollider2D))]
[RequireComponent(typeof(AudioSource))]
public class GrapperMovement : MonoBehaviour
{
    [Header("적 이름")]
    public string enemyType = "Grapper";

    [Header("이동")]
    public float moveSpeed = 4f;
    public float moveRadius = 3f;
    public float trunDuration = 0.3f;

    [Header("랜덤 공격")]
    public float minAttackInterval = 2f;
    public float maxAttackInterval = 5f;
    public float attackDuration = 2f;

    [Header("벽 감지")]
    public Transform wallCheckPos;
    public float layerCheckRadius = 0.05f;
    public LayerMask obstacleMask;

    [Header("공격 판정")]
    public Vector2 hitboxOffset = new Vector2(0.5f, 0f);
    public Vector2 hitboxSize = new Vector2(1f, 1f);
    public LayerMask playerLayer;

    [Header("무적 사운드")]
    public AudioClip invincibleHitSound;

    [Header("죽음")]
    public float deathDuration = 1.5f;
    public float fallingOutPower = 12f;

    [Header("사망 후 제외 레이어")]
    public LayerMask afterDeathLayer;

    public enum State { Move, Turn, Attack }
    private State currentState;

    private bool isGoingRight = true;
    private bool isDead = false;
    public bool isAttacking = false;
    private int facingSign = 1;

    private float attackTimer = 0f;
    private float nextAttackTime = 0f;

    private Vector3 movePosRight;
    private Vector3 movePosLeft;
    private Vector3 targetPos;

    private Coroutine turnCoroutine;
    private Coroutine attackCoroutine;

    private Rigidbody2D rb;
    private Animator anim;
    private CapsuleCollider2D capsuleCol;
    private AudioSource audioSource;

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

        playerHelthSystem = FindAnyObjectByType<PlayerHelthSystem>();

        if (moveRadius < 0) moveRadius *= -1f;
        if (maxAttackInterval < minAttackInterval)
            maxAttackInterval = minAttackInterval;

        movePosRight = movePosLeft = transform.position;
        movePosRight.x += moveRadius;
        movePosLeft.x -= moveRadius;

        targetPos = movePosRight;
        isGoingRight = true;
        isDead = false;
        isAttacking = false;

        attackTimer = 0f;
        nextAttackTime = GetRandomAttackTime();

        SetState(State.Move);
    }

    private void Update()
    {
        if (isDead) return;

        switch (currentState)
        {
            case State.Move:
                attackTimer += Time.deltaTime;
                MoveHandler();

                if (attackTimer >= nextAttackTime)
                    SetState(State.Attack);
                break;
        }

        if (isAttacking)
            AttackCheck();

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
                if (attackCoroutine != null) StopCoroutine(attackCoroutine);
                attackCoroutine = StartCoroutine(AttackStep());
                break;
        }
    }

    private void MoveHandler()
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

        float step = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

        if (Vector3.Distance(transform.position, targetPos) < 0.05f)
            SetState(State.Turn);
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
        rb.linearVelocity = Vector2.zero;
        isAttacking = true;
        attackTimer = 0f;

        anim.SetTrigger("startAttack");

        yield return new WaitForSeconds(attackDuration);

        isAttacking = false;
        anim.SetTrigger("endAttack");

        attackTimer = 0f;
        nextAttackTime = GetRandomAttackTime();

        attackCoroutine = null;
        SetState(State.Move);
    }

    private float GetRandomAttackTime()
    {
        return Random.Range(minAttackInterval, maxAttackInterval);
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
        if (isDead) return;

        if (other.CompareTag("Bullet"))
        {
            if (other.TryGetComponent(out BulletSystem bullet))
            {
                if (bullet.type == BulletType.PlayerBullet)
                {
                    Destroy(other.gameObject);

                    if (isAttacking)
                    {
                        if (invincibleHitSound != null && audioSource != null)
                            audioSource.PlayOneShot(invincibleHitSound);
                        return;
                    }

                    Die();
                }
            }
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

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        isAttacking = false;

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
        bool moving = (currentState == State.Move);
        anim.SetBool("isMoving", moving);
        anim.SetBool("isAttacking", isAttacking);
    }

    private void OnDrawGizmosSelected()
    {
        int sign = Application.isPlaying ? facingSign : 1;

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
            Vector3 r = transform.position;
            Vector3 l = transform.position;
            r.x += moveRadius;
            l.x -= moveRadius;

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
}