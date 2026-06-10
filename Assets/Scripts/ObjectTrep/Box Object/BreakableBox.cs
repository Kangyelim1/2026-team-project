using UnityEngine;

public class BreakableBox : MonoBehaviour
{
    [Header("Box Settings")]
    public int hp = 1;
    public GameObject breakEffect;

    [Header("Drop Item Prefabs")]
    public GameObject rapidGunItemPrefab;
    public GameObject shieldCharmItemPrefab;

    private bool isBroken = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isBroken) return;

        if (collision.CompareTag("Bullet"))
        {
            if (collision.TryGetComponent(out BulletSystem bullet))
            {
                if (bullet.type == BulletType.PlayerBullet)
                {
                    Destroy(collision.gameObject);
                    TakeDamage();
                }
            }
        }
    }

    private void TakeDamage()
    {
        hp--;
        if (hp <= 0)
            Break();
    }

    private void Break()
    {
        isBroken = true;

        if (breakEffect != null)
            Instantiate(breakEffect, transform.position, Quaternion.identity);

        TryDropItem();
        Destroy(gameObject);
    }

    private void TryDropItem()
    {
        float dropRoll = Random.value;  // 0.0 ~ 1.0
        if (dropRoll > 0.2f)
        {
            Debug.Log("Box: No drop (" + (dropRoll * 100f).ToString("F0") + "%)");
            return;
        }

        float itemRoll = Random.value;

        if (itemRoll <= 0.4f)
        {
            if (rapidGunItemPrefab != null)
            {
                Instantiate(rapidGunItemPrefab, transform.position, Quaternion.identity);
                Debug.Log("Box Drop: Rapid Gun!");
            }
        }
        else if (itemRoll <= 0.5f)
        {
            if (shieldCharmItemPrefab != null)
            {
                Instantiate(shieldCharmItemPrefab, transform.position, Quaternion.identity);
                Debug.Log("Box Drop: Shield Charm!");
            }
        }
        else
        {
            Debug.Log("Box Drop: Nothing (blank roll)");
        }
    }
}