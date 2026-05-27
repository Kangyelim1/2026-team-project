using UnityEngine;

public class PlayerHelthSystem : MonoBehaviour
{
    public PlayerSystem playerSystem;
    public GameManger gameManger;

    private void Update()
    {
        if(playerSystem == null)
            playerSystem = FindAnyObjectByType<PlayerSystem>();

        if(gameManger == null)
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
        Debug.Log("Die 함수 호출 확인");
        if (gameManger == null) return;
        Debug.Log("플레이어 사망");
        gameManger.isDiePlayer = true;
        Debug.Log("사망 애니매이션");
    }
}
