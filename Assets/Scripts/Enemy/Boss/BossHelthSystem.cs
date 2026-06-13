using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHelthSystem : MonoBehaviour
{
    public Slider bossHelthSlider;
    public TextMeshProUGUI bossHelthText;

    public EnemyHelthSystem enemyHelthSystem;
    public float smoothSpeed = 5f;

    private void Start()
    {
        enemyHelthSystem = FindAnyObjectByType<EnemyHelthSystem>();

        bossHelthSlider.minValue = 0;
        bossHelthSlider.maxValue = enemyHelthSystem.maxBossHelth;
        bossHelthSlider.value = enemyHelthSystem.currentBossHelth;
    }

   private void Update()
   {
        int currentHp = Mathf.Clamp(enemyHelthSystem.currentBossHelth, 0, enemyHelthSystem.maxBossHelth);

        bossHelthSlider.value = Mathf.Lerp(bossHelthSlider.value, currentHp, smoothSpeed * Time.deltaTime);

        if (Mathf.Abs(bossHelthSlider.value - currentHp) < 0.1f)
            bossHelthSlider.value = currentHp;

        bossHelthText.text = $"{enemyHelthSystem.currentBossHelth} / {enemyHelthSystem.maxBossHelth}";
    }
}