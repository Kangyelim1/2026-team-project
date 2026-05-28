using Cainos.LucidEditor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHelthSystem : MonoBehaviour
{
    public EnemySystem enemySystem;

    [Header("보스 체력")]
    public int maxBossHelth;
    public int MinBsooHelth;
    public int currentBossHelth;
    public Slider bossSlider;

    private void Start()
    {
        enemySystem = GetComponentInParent<EnemySystem>();

        if (enemySystem.enemyType == EnemyType.Boss)
        {
            maxBossHelth = currentBossHelth;
            SliderUI();
        }
    }

    private void Update()
    {
        if (enemySystem.enemyType == EnemyType.Boss)
        {
            SliderUI();
        }
    }

    private void SliderUI()
    {
        if (bossSlider == null) return;

        bossSlider.maxValue = maxBossHelth;
        bossSlider.minValue = 0;
        bossSlider.value = currentBossHelth;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            if (collision.gameObject.TryGetComponent(out BulletSystem bullet))
            {
                if (bullet.type == BulletType.PlayerBullet)
                {
                    Helth();
                }
            }
        }
    }


    void Helth()
    {
        if (enemySystem.enemyType == EnemyType.Boss)
        {
            currentBossHelth -= 5;
            Debug.Log("보스 체력 감소");
            SliderUI();

            if (currentBossHelth <= 0)
            {
                Die();
            }
        }
        else Die();
    }

    public void Die()
    {
        Debug.Log("몬스터 사망");

        if (bossSlider != null)
        {
            bossSlider.gameObject.SetActive(false);
        }

        Destroy(enemySystem.gameObject);
    }
}
