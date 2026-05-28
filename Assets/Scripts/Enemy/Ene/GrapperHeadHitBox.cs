using UnityEngine;

public class GrapperHeadHitBox : MonoBehaviour
{
    private GrapperMovement grapperMovement;

    private void Awake()
    {
        grapperMovement = GetComponentInParent<GrapperMovement>();

        if (grapperMovement == null)
            Debug.LogWarning("[GrapperHeadHitBox] 부모에서 GrapperMovement를 찾을 수 없습니다.");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            if (other.TryGetComponent(out BulletSystem bullet))
            {
                if (bullet.type == BulletType.PlayerBullet)
                {
                    Destroy(other.gameObject);

                    if (grapperMovement != null)
                        grapperMovement.Die();
                }
            }
        }
    }
}