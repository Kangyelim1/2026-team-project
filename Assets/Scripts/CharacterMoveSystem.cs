using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class PlayerMoveSystem : MonoBehaviour
{
    public float PlayerMoveSpeed = 5f;

    [Header("대시")]
    public float PlayerDashDistance = 4f;
    public float PlayerDashDuration = 0.15f;

    [Header("구르기")]
    public float PlayerRollDistance = 2.5f;
    public float PlayerRollDuration = 0.25f;

    public Rigidbody2D PlayerRigidbody;
    public SpriteRenderer PlayerSpriteRenderer;

    private float moveX;

    private bool isDash;
    private bool isRoll;

    private Camera mainCamera;

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

        if (Input.GetMouseButtonDown(1) && !isDash && !isRoll)
        {
            StartCoroutine(Dash());
        }

        if (Input.GetKeyDown(KeyCode.Space) && !isRoll && !isDash)
        {
            StartCoroutine(Roll());
        }
    }

    private void FixedUpdate()
    {
        if (!isDash && !isRoll)
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
        Debug.Log(" 근거리 공격 진행");
        isDash = true;

        float direction = PlayerSpriteRenderer.flipX ? -1f : 1f;

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
    }

    private IEnumerator Roll()
    {
        Debug.Log("구르기 진행");
        isRoll = true;

        float direction = PlayerSpriteRenderer.flipX ? -1f : 1f;

        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + new Vector3(direction * PlayerRollDistance, 0f, 0f);

        float time = 0f;

        while (time < PlayerRollDuration)
        {
            time += Time.deltaTime;

            transform.position = Vector3.Lerp(startPos, targetPos, time / PlayerRollDuration);
            yield return null;
        }

        transform.position = targetPos;

        isRoll = false;
    }

    private void Flip()
    {
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        if (mouseWorldPos.x < transform.position.x)
        {
            PlayerSpriteRenderer.flipX = true;
        }
        else
        {
            PlayerSpriteRenderer.flipX = false;
        }
    }
}