using UnityEngine;
using UnityEngine.UI;

public class EnemySystem : MonoBehaviour
{
    public EnemySO enemySO;

    public string Enemy_Name;
    public int Enemy_Levle;
    public int Enemy_MaxHelth;
    public int Enemy_Damage;
    public int Enemy_Speed;
    public GameObject EnemyPrefab;

    [Header("VFX")]
    public GameObject HitEffect;

    [Header("Helth")]
    public int Enemy_CurrentHelth;
    public Slider enemyHelthSlider;
    [SerializeField] private float smoothSpeed = 5f;



    private void Start()
    {
        Enemy_Name = enemySO.EnemyName;
        Enemy_Levle = enemySO.EnemyLevel;
        Enemy_MaxHelth = enemySO.EnemyMaxHelth;
        Enemy_Damage = enemySO.EnemyDamage;
        Enemy_Speed = enemySO.EnemySpeed;

        Enemy_CurrentHelth = Enemy_MaxHelth;
    }

    private void Update()
    {
        enemyHelthSlider.minValue = 0;
        enemyHelthSlider.maxValue = Enemy_MaxHelth;

        HelthUI();
    }

    private void HelthUI()
    {
        enemyHelthSlider.value = Mathf.Lerp(enemyHelthSlider.value, Enemy_CurrentHelth, smoothSpeed * Time.deltaTime);
    }
}
