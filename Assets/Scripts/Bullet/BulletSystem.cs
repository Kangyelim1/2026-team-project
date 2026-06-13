using Cainos.LucidEditor;
using JetBrains.Annotations;
using System.Collections;
using UnityEngine;

public class BulletSystem : MonoBehaviour
{
    public BulletSO bulletSO;
    public float lifeTime;
    public BulletType type;
    public int damage = 1;

    [Header("VFX")]
    public GameObject hitVFX;

    [Header("미사일 전용")]
    public GameObject missileVFX;
    public GameSoundManager gameSoundManager;
    private void Awake()
    {
        lifeTime = bulletSO.bulletRifeTime;
        type = bulletSO.bulletType;

        gameSoundManager = FindAnyObjectByType<GameSoundManager>();
    }

    private void Start()
    {
        StartCoroutine(DestroyBullet());
        if (missileVFX != null) missileVFX.gameObject.SetActive(false);
        if(hitVFX != null) hitVFX.gameObject.SetActive(false);
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

        if (collision.CompareTag("Ground"))
        {
            if(type == BulletType.Missile)
            {
                gameSoundManager.OnFindEnemySound("미사일 폭발");
                missileVFX.gameObject.SetActive(true);
                Destroy(gameObject, 0.06f);
            }
            else
            {
                Destroy(gameObject);
            }
            
        }

        if (collision.CompareTag("EnemyHitPoint") && type == BulletType.PlayerBullet)
        {
            Debug.Log("몬스터 명중");
            hitVFX.gameObject.SetActive(true);
            Destroy(gameObject, 0.06f);
        }

        if (collision.CompareTag("PlayerHitPoint") && type == BulletType.EnemyBullet)
        {
            Debug.Log("플레이어 명중");
            Destroy(gameObject);
        }

        if (collision.CompareTag("PlayerHitPoint") && type == BulletType.Missile)
        {
            Debug.Log("미사일 플레이어 명중");
            Destroy(gameObject);
        }

        if (collision.CompareTag("DestoryObject") && type == BulletType.PlayerBullet)
        {
            Debug.Log("삭제 오브젝트 명중");
            hitVFX.gameObject.SetActive(true);
            Destroy(gameObject, 0.06f);
        }
    }
}
