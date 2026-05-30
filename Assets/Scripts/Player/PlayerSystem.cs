  using System.Collections;
using UnityEngine;

public class PlayerSystem : MonoBehaviour
{
    public float PlayerMoveSpeed = 5f;

    [Header("대시")]
    public float PlayerDashDistance = 4f;
    public float PlayerDashDuration = 0.15f;

    [Header("점프")]
    public float JumpForce = 7f;
    public float FallGravityScale = 4f;
    public float NormalGravityScale = 2f;

    public Rigidbody2D PlayerRigidbody;
    public SpriteRenderer PlayerSpriteRenderer;
    public Collider2D PlayerCollider;

    public EnemyHelthSystem enemyHelthSystem;
    public bool isDashAttack;

    public GameManger gameManger;
    public GameObject LockOnImage;

    private float moveX;
    private bool isDash;
    public bool isGround;

    private Camera mainCamera;

    private void Start()
    {
        gameManger = FindAnyObjectByType<GameManger>();
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
        moveX = Input.GetAxisRaw("Horizontal");

        Flip();

        if (PlayerRigidbody.linearVelocity.y < 0)
        {
            PlayerRigidbody.gravityScale = FallGravityScale;
        }
        else
        {
            PlayerRigidbody.gravityScale = NormalGravityScale;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGround && !isDash)
        {
            Jump();
        }

        if (Input.GetMouseButtonDown(1) && !isDash)
        {
            StartCoroutine(Dash());
        } 
    }

    private void FixedUpdate()
    {
        if (!isDash)
        {
            Move();
        }
    }

    private void Move()
    {
        float targetVelocityX = moveX * PlayerMoveSpeed;
        PlayerRigidbody.linearVelocity = new Vector2(targetVelocityX, PlayerRigidbody.linearVelocity.y);
    }

    private IEnumerator Dash()
    {
        Debug.Log("구르기");

        isDash = true;
        isDashAttack = true;

        float direction = transform.localScale.x < 0 ? -1f : 1f;

        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + new Vector3(direction * PlayerDashDistance, 0f, 0f);

        float time = 0f;

        while (time < PlayerDashDuration)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, targetPos, time / PlayerDashDuration);
            yield return null;
        }

        transform.position = targetPos;

        yield return new WaitForSeconds(0.5f);

        isDash = false;
        isDashAttack = false;
    }

    private void Jump()
    {
        Debug.Log("점프");

        isGround = false;

        PlayerRigidbody.linearVelocity = new Vector2(PlayerRigidbody.linearVelocity.x, 0f);
        PlayerRigidbody.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);

        StartCoroutine(JumpColliderOff());
    }

    private IEnumerator JumpColliderOff()
    {
        if (PlayerCollider != null)
        {
            PlayerCollider.enabled = false;
            Debug.Log("콜라이더 OFF");
        }

        yield return new WaitForSeconds(0.8f);

        if (PlayerCollider != null)
        {
            PlayerCollider.enabled = true;
            Debug.Log("콜라이더 ON");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = true;
        }

        if (collision.contacts.Length > 0 && collision.contacts[0].normal.y < -0.5f)
        {
            PlayerRigidbody.linearVelocity = new Vector2(PlayerRigidbody.linearVelocity.x, 0f);
        }
    }

    private void Flip()
    {
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        if (mouseWorldPos.x > transform.position.x)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
}