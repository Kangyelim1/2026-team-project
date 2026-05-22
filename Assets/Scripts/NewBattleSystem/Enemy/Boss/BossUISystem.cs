using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossUISystem : MonoBehaviour
{
    public EnemySystem enemySystem;
    public TextMeshProUGUI bossNameText;

    [Header("체력 관련")]
    public GameObject bossUI;
    public Slider bossHelthSlider;
    public TextMeshProUGUI bossHelthText;


    private void Update()
    {
        if(enemySystem == null) enemySystem = FindAnyObjectByType<EnemySystem>();

        bossHelthSlider.minValue = 0;
        bossHelthSlider.maxValue = enemySystem.Enemy_MaxHelth;

        bossNameText.text = enemySystem.Enemy_Name;
        bossHelthText.text = $"{enemySystem.Enemy_CurrentHelth} / {enemySystem.Enemy_MaxHelth}";
        HelthUI();
        StartBass();
    }

    public void StartBass()
    {
        if (enemySystem.Enemy_Type == EnemyType.Boss)
        {
            enemySystem.enemyHelthSlider.gameObject.SetActive(false);
            enemySystem.HelthText.gameObject.SetActive(false);
            enemySystem.nameText.gameObject.SetActive(false);
            bossUI.gameObject.SetActive(true);
            return;
        }
    }

    private void HelthUI()
    {
        bossHelthSlider.value = Mathf.Lerp(bossHelthSlider.value, enemySystem.Enemy_CurrentHelth, enemySystem.smoothSpeed * Time.deltaTime);
    }
}
