using UnityEngine;

public class PlayerHelthSystem : MonoBehaviour
{
    public PlayerMoveSystem MoveSystem;
    public GameManger gameManger;

    private void Start()
    {
        MoveSystem = FindAnyObjectByType<PlayerMoveSystem>();
        gameManger = FindAnyObjectByType<GameManger>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            if (collision.gameObject.TryGetComponent(out BulletSystem bullet))
            {
                if (bullet.type == BulletType.EnemyBullet)
                {
                    Die();
                }
            }
        }
    }

    public void Die()
    {
        Debug.Log("플레이어 사망");
        StartCoroutine(gameManger.DiePlayer());
        Destroy(MoveSystem.gameObject);
    }
}
