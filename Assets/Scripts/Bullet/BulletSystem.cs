using JetBrains.Annotations;
using System.Collections;
using UnityEngine;

public class BulletSystem : MonoBehaviour
{
    public BulletSO bulletSO;
    public float lifeTime;
    public BulletType type;

    private void Awake()
    {
        lifeTime = bulletSO.bulletRifeTime;
        type = bulletSO.bulletType;
    }

    private void Start()
    {
        StartCoroutine(DestroyBullet());
    }


    IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }

        if (collision.CompareTag("EnemyHitPoint") && type == BulletType.PlayerBullet)
        {
            Debug.Log("몬스터 명중");
            Destroy(gameObject);
        }

        if (collision.CompareTag("PlayerHitPoint") && type == BulletType.EnemyBullet)
        {
            Debug.Log("플레이어 명중");
            Destroy(gameObject);
        }
    }
}
