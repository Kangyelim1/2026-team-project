using System.Collections;
using UnityEngine;

public class PlayerSystem : MonoBehaviour
{
    public float PlayerMoveSpeed = 5f;

    [Header("대시")]
    public float PlayerDashDistance = 4f;
    public float PlayerDashDuration = 0.15f;

    public float JumpForce = 7f;

    public Rigidbody2D PlayerRigidbody;
    public SpriteRenderer PlayerSpriteRenderer;

    public EnemyHelthSystem enemyHelthSystem;
    public bool isDashAttack;

    public GameManger gameManger;

    public GameObject LockOnImage;

    private float moveX;
    private bool isDash;
    private bool isJump;


    private Camera mainCamera;

    private void Start()
    {
        gameManger = FindAnyObjectByType<GameManger>();
    }

    private void Awake()
    {
        if (PlayerRigidbody == null)
            PlayerRigidbody = GetComponent<Rigidbody2D>();

        if (PlayerSpriteRenderer == null)
            PlayerSpriteRenderer = GetComponent<SpriteRenderer>();

        mainCamera = Camera.main;
    }

    private void Update()
    {
        moveX = Input.GetAxisRaw("Horizontal");

        Flip();

        if (Input.GetMouseButtonDown(1) && !isDash && !isJump)
        {
            StartCoroutine(Dash());
        }

        if (Input.GetKeyDown(KeyCode.Space) && !isJump && !isDash)
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {               
        if (!isDash && !isJump)
        {   
            Move();
        }
    }

    private void Move()
    {
        Vector2 moveVector = new Vector2(moveX * PlayerMoveSpeed * Time.fixedDeltaTime, 0f);

        PlayerRigidbody.MovePosition(PlayerRigidbody.position + moveVector);
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

        isDash = false;
        isDashAttack = false;
    }

    private void Jump()
    {
        Debug.Log("점프");

        isJump = true;

        PlayerRigidbody.linearVelocity = new Vector2(PlayerRigidbody.linearVelocity.x, 0f);
        PlayerRigidbody.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJump = false;
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