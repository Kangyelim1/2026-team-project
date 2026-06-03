using UnityEngine;

public class TrapSystem : MonoBehaviour
{
    [Header("트랩 범위")]
    public float width = 1f;
    public float height = 1f;
    public Vector2 offset = Vector2.zero;

    [Header("감지 설정")]
    public LayerMask targetLayer;

    private float cooldown = 0.5f;
    private float lastHitTime = -999f;

    private void Update()
    {
        if (Time.time - lastHitTime >= cooldown)
        {
            CheckOverlap();
        }
    }

    private void CheckOverlap()
    {
        Vector2 center = (Vector2)transform.position + offset;
        Vector2 size = new Vector2(width, height);

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0f, targetLayer);

        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag("Player")) continue;

            PlayerHelthSystem playerHealth =
                hit.GetComponent<PlayerHelthSystem>() ??
                hit.GetComponentInParent<PlayerHelthSystem>() ??
                hit.GetComponentInChildren<PlayerHelthSystem>();

            if (playerHealth != null)
            {
                playerHealth.Die();
                lastHitTime = Time.time;
                break;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.4f);
        Vector2 center = (Vector2)transform.position + offset;
        Gizmos.DrawCube(center, new Vector2(width, height));  

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, new Vector2(width, height));
    }
}