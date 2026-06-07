using System.Collections;
using UnityEngine;

public class PlayerSystem : MonoBehaviour
{
    public float PlayerMoveSpeed = 5f;
    public Animator playerAnimator;

    [Header("대시")]
    public float PlayerDashDistance = 4f;
    public float PlayerDashDuration = 0.15f;
    private bool isNotDash;

    [Header("점프")]
    public float JumpForce = 4.5f;
    public float colliderOffTime = 0.5f;
    public float FallGravityScale = 4f;
    public float NormalGravityScale = 2f;
    private bool isNotJump;

    [Header("더블 점프")]
    public float doubleJumpMultiplier = 1.2f;
    private bool canDoubleJump = false;
    private bool hasDoubleJumped = false;

    public Rigidbody2D PlayerRigidbody;
    public SpriteRenderer PlayerSpriteRenderer;
    public Collider2D PlayerCollider;

    public EnemyHelthSystem enemyHelthSystem;
    public bool isDashAttack;

    public GameManger gameManger;
    public GameObject LockOnImage;
    public GameSoundManager gameSoundManager;

    private float moveX;
    private bool isDash;
    public bool isGround;
    public bool isPattern;
    public bool IsDash => isDash;

    private Camera mainCamera;

    private void Start()
    {
        gameManger = FindAnyObjectByType<GameManger>();
        playerAnimator = GetComponentInChildren<Animator>();
        gameSoundManager = FindAnyObjectByType<GameSoundManager>();
    }

    private void Awake()
    {
        if (PlayerRigidbody == null) PlayerRigidbody = GetComponent<Rigidbody2D>();
        if (PlayerSpriteRenderer == null) PlayerSpriteRenderer = GetComponent<SpriteRenderer>();
        if (PlayerCollider == null) PlayerCollider = GetComponent<Collider2D>();

        mainCamera = Camera.main;
    }

    private void Update()
    {
        Flip();

        if (PlayerRigidbody.linearVelocity.y < 0)
            PlayerRigidbody.gravityScale = FallGravityScale;
        else
            PlayerRigidbody.gravityScale = NormalGravityScale;

        if (Input.GetKeyDown(KeyCode.Space) && !isDash && !isNotJump)
        {
            if (isGround)
            {
                Jump();
                canDoubleJump = true;
                hasDoubleJumped = false;
            }
            else if (canDoubleJump && !hasDoubleJumped)
            {
                DoubleJump();
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDash && !isNotDash)
        {
            StartCoroutine(Dash());
        }
    }

    private void FixedUpdate()
    {
        if (!isDash)
            Move();
    }

    private void Move()
    {
        moveX = Input.GetAxisRaw("Horizontal");

        Vector3 direction = new Vector3(moveX, 0, 0).normalized;
        Vector3 targetVelocityX = direction * PlayerMoveSpeed * Time.deltaTime;

        transform.position += targetVelocityX;

        if (direction != Vector3.zero)
            playerAnimator.SetBool("isRun", true);
        else
            playerAnimator.SetBool("isRun", false);
    }

    private IEnumerator Dash()
    {
        isDash = true;
        isDashAttack = true;

        float direction = transform.localScale.x < 0 ? -1f : 1f;

        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + new Vector3(direction * PlayerDashDistance, 0f, 0f);

        float time = 0f;
        playerAnimator.SetBool("isRolling", true);

        SkillHUDManager.Instance?.TriggerCooldown(SkillType.Dash, PlayerDashDuration + 0.5f);

        yield return new WaitForSeconds(0.1f);
        while (time < PlayerDashDuration)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, targetPos, time / PlayerDashDuration);
            yield return null;
        }

        transform.position = targetPos;

        yield return new WaitForSeconds(0.5f);
        playerAnimator.SetBool("isRolling", false);

        isDash = false;
        isDashAttack = false;
    }

    private void Jump()
    {
        isGround = false;
        PlayerRigidbody.linearVelocity = new Vector2(PlayerRigidbody.linearVelocity.x, 0f);
        PlayerRigidbody.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);

        StartCoroutine(JumpColliderOff());
    }

    private void DoubleJump()
    {
        hasDoubleJumped = true;
        canDoubleJump = false;

        PlayerRigidbody.linearVelocity = new Vector2(PlayerRigidbody.linearVelocity.x, 0f);
        PlayerRigidbody.AddForce(Vector2.up * JumpForce * doubleJumpMultiplier, ForceMode2D.Impulse);
    }

    public void HighJump(float multiplier)
    {
        if (!isGround || isDash || isNotJump) return;

        isGround = false;
        PlayerRigidbody.linearVelocity = new Vector2(PlayerRigidbody.linearVelocity.x, 0f);
        PlayerRigidbody.AddForce(Vector2.up * JumpForce * multiplier, ForceMode2D.Impulse);

        StartCoroutine(JumpColliderOff());
    }

    private IEnumerator JumpColliderOff()
    {
        playerAnimator.SetBool("isJump", true);
        if (PlayerCollider != null)
            PlayerCollider.enabled = false;

        yield return new WaitForSeconds(colliderOffTime);

        if (PlayerCollider != null)
            PlayerCollider.enabled = true;
        playerAnimator.SetBool("isJump", false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = true;
            canDoubleJump = false;
            hasDoubleJumped = false;
        }

        if (collision.contacts.Length > 0 && collision.contacts[0].normal.y < -0.5f)
        {
            PlayerRigidbody.linearVelocity = new Vector2(PlayerRigidbody.linearVelocity.x, 0f);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("NotJump"))
        {
            isNotJump = true;
            isNotDash = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("NotJump"))
        {
            isNotJump = false;
            isNotDash = false;
        }
    }

    private void Flip()
    {
        if (mainCamera == null) return;

        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        if (mouseWorldPos.x > transform.position.x)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }
}