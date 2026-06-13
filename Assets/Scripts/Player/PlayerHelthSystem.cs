using UnityEngine;

public class PlayerHelthSystem : MonoBehaviour
{
    public PlayerSystem playerSystem;
    public GameManger gameManger;
    public PlayerAttackSystem playerAttackSystem;

    private void Update()
    {
        if (playerSystem == null)
            playerSystem = FindAnyObjectByType<PlayerSystem>();

        if (gameManger == null)
            gameManger = FindAnyObjectByType<GameManger>();

        if (playerAttackSystem == null)
            playerAttackSystem = FindAnyObjectByType<PlayerAttackSystem>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (playerAttackSystem != null && playerAttackSystem.IsInvincible)
        {
            bool parrySuccess = playerAttackSystem.TryParryCounter(collision);

            if (parrySuccess)
                Debug.Log("패링 성공 / 적에게 6 데미지");

            return;
        }

        if (collision.gameObject.CompareTag("Bullet"))
        {
            if (collision.gameObject.TryGetComponent(out BulletSystem bullet))
            {
                if (bullet.type == BulletType.EnemyBullet && bullet.type == BulletType.Missile)
                {
                    Die();
                }
            }
        }

        if (collision.gameObject.CompareTag("LaserHitPoint"))
        {
            Die();
        }

        if (collision.gameObject.CompareTag("DestoryAttackObject"))
        {
            Die();
        }
    }

    public void Die()
    {
        PlayerItemSystem itemSystem = GetComponentInParent<PlayerItemSystem>();
        if (itemSystem == null)
            itemSystem = GetComponent<PlayerItemSystem>();

        if (itemSystem != null && itemSystem.TryBlockHit())
            return;

        if (gameManger.isDiePlayer) return;

        if (CameraShake.Instance != null)
            CameraShake.Instance.Shake(1.5f);

        gameManger.isDiePlayer = true;

        Debug.Log("Die 함수 호출 확인");

        if (gameManger == null || playerSystem == null)
            return;

        Debug.Log("플레이어 사망");
        gameManger.isDiePlayer = true;
        Destroy(playerSystem.gameObject);
    }
}