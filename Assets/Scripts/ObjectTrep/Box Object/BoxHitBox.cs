using UnityEngine;

public class BoxHitBox : MonoBehaviour
{
    private BreakableBox breakableBox;

    private void Awake()
    {
        breakableBox = GetComponentInParent<BreakableBox>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            if (collision.TryGetComponent(out BulletSystem bullet))
            {
                if (bullet.type == BulletType.PlayerBullet)
                {
                    Destroy(collision.gameObject);
                    breakableBox?.TakeDamage();
                }
            }
        }
    }
}