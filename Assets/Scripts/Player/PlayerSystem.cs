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
    public float doubleJumpWindow = 0.5f;
    private bool canDoubleJump = false;
    private bool hasDoubleJumped = false;
    private float jumpPressTime = -999f;

    public Rigidbody2D PlayerRigidbody;
    public SpriteRenderer PlayerSpriteRenderer;
    public Collider2D PlayerCollider;

    public EnemyHelthSystem enemyHelthSystem;
    public bool isDashAttack;

    public GameManger gameManger;
    public GameObject LockOnImage;
    public GameObject FakeDestoryObject;
    public GameSoundManager gameSoundManager;

    private float moveX;
    private bool isDash;
    private bool isDashMoving;
    private int groundContactCount = 0;

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

        if (Input.GetKeyDown(KeyCode.Space) && !isDashMoving && !isNotJump)
        {
            if (isGround)
            {
                Jump();
                jumpPressTime = Time.time;
                canDoubleJump = true;
                hasDoubleJumped = false;
            }
            else if (canDoubleJump && !hasDoubleJumped)
            {
                if (Time.time - jumpPressTime <= doubleJumpWindow)
                    DoubleJump();
            }
        }

        if (canDoubleJump && !hasDoubleJumped)
        {
            if (Time.time - jumpPressTime > doubleJumpWindow)
                canDoubleJump = false;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDash && !isNotDash)
            StartCoroutine(Dash());
    }

    private void FixedUpdate()
    {
        if (!isDashMoving)
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
        isDashMoving = true;
        isDashAttack = true;

        GameSoundManager.Instance?.PlaySFX(GameSoundManager.Instance.playerDashSound);

        float direction = transform.localScale.x < 0 ? -1f : 1f;
        float dashSpeed = PlayerDashDistance / PlayerDashDuration;

        playerAnimator.SetBool("isRolling", true);
        SkillHUDManager.Instance?.TriggerCooldown(SkillType.Dash, PlayerDashDuration + 0.5f);

        float originalGravity = PlayerRigidbody.gravityScale;
        PlayerRigidbody.gravityScale = 0f;

        yield return new WaitForSeconds(0.1f);

        float time = 0f;
        while (time < PlayerDashDuration)
        {
            time += Time.deltaTime;
            PlayerRigidbody.linearVelocity = new Vector2(direction * dashSpeed, 0f);
            yield return null;
        }

        PlayerRigidbody.linearVelocity = Vector2.zero;
        PlayerRigidbody.gravityScale = originalGravity;

        isDashMoving = false;
        isDashAttack = false;

        yield return new WaitForSeconds(0.5f);
        playerAnimator.SetBool("isRolling", false);

        isDash = false;
    }

    private void Jump()
    {
        GameSoundManager.Instance?.PlaySFX(GameSoundManager.Instance.playerJumpSound);

        isGround = false;
        PlayerRigidbody.linearVelocity = new Vector2(PlayerRigidbody.linearVelocity.x, 0f);
        PlayerRigidbody.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);

        StartCoroutine(JumpColliderOff());
    }

    private void DoubleJump()
    {
        GameSoundManager.Instance?.PlaySFX(GameSoundManager.Instance.playerDoubleJumpSound);

        hasDoubleJumped = true;
        canDoubleJump = false;

        PlayerRigidbody.linearVelocity = new Vector2(PlayerRigidbody.linearVelocity.x, 0f);
        PlayerRigidbody.AddForce(Vector2.up * JumpForce * doubleJumpMultiplier, ForceMode2D.Impulse);
    }

    public void HighJump(float multiplier)
    {
        if (!isGround || isDashMoving || isNotJump) return;

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

        yield return new WaitForFixedUpdate();

        playerAnimator.SetBool("isJump", false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Box"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    GameSoundManager.Instance?.PlaySFX(GameSoundManager.Instance.playerLandSound);

                    groundContactCount++;
                    isGround = true;
                    canDoubleJump = false;
                    hasDoubleJumped = false;
                    break;
                }
            }
        }

        if (collision.contacts.Length > 0 && collision.contacts[0].normal.y < -0.5f)
            PlayerRigidbody.linearVelocity = new Vector2(PlayerRigidbody.linearVelocity.x, 0f);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Box"))
        {
            groundContactCount--;
            if (groundContactCount <= 0)
            {
                groundContactCount = 0;
                isGround = false;
            }
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