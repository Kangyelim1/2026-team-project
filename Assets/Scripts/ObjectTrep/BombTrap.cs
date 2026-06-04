using System.Collections;
using UnityEngine;

public class BombTrap : MonoBehaviour
{
    [Header("Æø¹ß ¼³Á¤")]
    public float fuseTime = 1.5f;
    public float explosionRadius = 2.5f;

    [Header("°¨Áö ·¹ÀÌ¾î")]
    public LayerMask detectionLayer;

    [Header("ÀÌÆåÆ®")]
    public GameObject explosionEffectPrefab;

    [Header("±ôºýÀÓ ¼³Á¤")]
    public float blinkSpeed = 0.15f; 

    private bool isTriggered = false;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isTriggered) return;

        if (collision.CompareTag("Bullet"))
        {
            BulletSystem bullet = collision.GetComponent<BulletSystem>();

            if (bullet != null && bullet.type == BulletType.PlayerBullet)
            {
                Destroy(collision.gameObject);
                Activate();
                return;
            }
        }

        if (collision.CompareTag("Player") || collision.CompareTag("Enemy"))
        {
            Activate();
        }
    }

    public void Activate()
    {
        if (isTriggered) return;
        isTriggered = true;
        StartCoroutine(FuseRoutine());
    }

    private IEnumerator FuseRoutine()
    {
        Debug.Log("ÆøÅº ±âÆø ½ÃÀÛ!");

        StartCoroutine(BlinkRed());

        yield return new WaitForSeconds(fuseTime);
        Explode();
    }

    private IEnumerator BlinkRed()
    {
        float elapsed = 0f;
        float currentBlinkSpeed = blinkSpeed;
        bool isRed = false;

        while (isTriggered)
        {
            isRed = !isRed;

            if (spriteRenderer != null)
                spriteRenderer.color = isRed ? Color.red : originalColor;

            elapsed += currentBlinkSpeed;
            currentBlinkSpeed = Mathf.Max(0.05f, blinkSpeed - (elapsed * 0.01f));

            yield return new WaitForSeconds(currentBlinkSpeed);
        }

        if (spriteRenderer != null)
            spriteRenderer.color = Color.red;
    }

    private void Explode()
    {
        isTriggered = false; 

        Debug.Log("Æø¹ß!");

        if (explosionEffectPrefab != null)
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            explosionRadius,
            detectionLayer
        );

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerHelthSystem playerHealth =
                    hit.GetComponent<PlayerHelthSystem>() ??
                    hit.GetComponentInParent<PlayerHelthSystem>() ??
                    hit.GetComponentInChildren<PlayerHelthSystem>();

                if (playerHealth != null)
                {
                    Debug.Log("ÆøÅº - ÇÃ·¹ÀÌ¾î »ç¸Á");
                    playerHealth.Die();
                }
                continue;
            }

            if (hit.CompareTag("Enemy") || hit.CompareTag("EnemyHitPoint"))
            {
                EnemyHelthSystem enemyHealth =
                    hit.GetComponent<EnemyHelthSystem>() ??
                    hit.GetComponentInParent<EnemyHelthSystem>() ??
                    hit.GetComponentInChildren<EnemyHelthSystem>();

                if (enemyHealth != null)
                {
                    Debug.Log("ÆøÅº - Àû »ç¸Á");
                    enemyHealth.Die();
                }
            }
        }

        Destroy(gameObject, 0.05f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
        Gizmos.DrawSphere(transform.position, explosionRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}