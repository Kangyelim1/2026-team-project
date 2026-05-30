using UnityEngine;

public class DestoryObjectHitPoint : MonoBehaviour
{
    public int currentHelth;

    public DestoryObejct DestoryObejct;
    public EnemyHelthSystem enemyHelthSystem;
    public BossPatternSystem bossPatternSystem;

    private bool isInitialized = false;

    private void Start()
    {
        DestoryObejct = GetComponentInParent<DestoryObejct>();

        enemyHelthSystem = FindAnyObjectByType<EnemyHelthSystem>();
        bossPatternSystem = FindAnyObjectByType<BossPatternSystem>();
    }

    private void Update()
    {
        if (enemyHelthSystem == null) enemyHelthSystem = FindAnyObjectByType<EnemyHelthSystem>();
        if (bossPatternSystem == null) bossPatternSystem = FindAnyObjectByType<BossPatternSystem>();

        if (enemyHelthSystem != null && !isInitialized)
        {
            currentHelth = enemyHelthSystem.currentBossHelth <= enemyHelthSystem.minBossHelth ? 65 : 50;
            isInitialized = true; 
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Debug.Log("충돌");
            currentHelth -= 5;

            if (currentHelth <= 0)
            {
                bossPatternSystem.isDestoryObject = true;
                Destroy(DestoryObejct.gameObject);

            }
        }
    }
}