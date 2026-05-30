using Unity.VisualScripting;
using UnityEngine;

public class DestoryObejct : MonoBehaviour
{
    public EnemyHelthSystem enemyHelthSystem;
    public BossPatternSystem bossPatternSystem;

    public int cuttentHelth = 50;
    public int DownForce;
    public Rigidbody2D rb;

    public void Start()
    {
        rb.AddForceY(-DownForce, ForceMode2D.Force);
    }

    public void Update()
    {
        if(enemyHelthSystem == null)
            enemyHelthSystem = Object.FindAnyObjectByType<EnemyHelthSystem>();

        if(bossPatternSystem == null)
            bossPatternSystem = Object.FindAnyObjectByType<BossPatternSystem>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            if(collision.TryGetComponent<BulletSystem>(out BulletSystem bullet))
            {
                if(bullet.type == BulletType.PlayerBullet)
                {
                    cuttentHelth -= 5;

                    if(cuttentHelth <= 0)
                    {
                        bossPatternSystem.isDestoryObject = true;
                        Destroy(gameObject);
                    }
                }
            }
        }
    }
}
