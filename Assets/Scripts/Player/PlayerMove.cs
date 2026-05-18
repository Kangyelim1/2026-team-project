using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Animator worldPlayerAniamtor;

    private Rigidbody2D rb;
    private Vector2 movement;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        worldPlayerAniamtor = GetComponent<Animator>();
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal"); 
        float moveY = Input.GetAxisRaw("Vertical");   

        movement = new Vector2(moveX, moveY).normalized;

        if (moveX < -0.1f) Flip(false);
        else if (moveX > 0.1f) Flip(true);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);

        if (movement != Vector2.zero)
        {
            worldPlayerAniamtor.SetBool("IsLeft", true);
        }
        else
        {
            worldPlayerAniamtor.SetBool("IsLeft", false);
        }
    }

    void Flip(bool shouldFlip)
    {
        Vector3 scale = transform.localScale;

        if (shouldFlip) scale.x = -Mathf.Abs(scale.x);
        else scale.x = Mathf.Abs(scale.x);

        transform.localScale = scale;
    }
}
