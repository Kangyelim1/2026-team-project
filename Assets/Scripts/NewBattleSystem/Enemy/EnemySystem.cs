using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemySystem : MonoBehaviour
{
    public EnemySO enemySO;

    public string Enemy_Name;
    public int Enemy_Levle;
    public int Enemy_MaxHelth;
    public int Enemy_Damage;
    public EnemyType Enemy_Type;
    public int Enemy_Speed;
    public GameObject EnemyPrefab;
    public TextMeshProUGUI nameText;

    [Header("VFX")]
    public GameObject HitEffect;
    public GameObject AttackEffect;

    [Header("Helth")]
    public int Enemy_CurrentHelth;
    public Slider enemyHelthSlider;
    public float smoothSpeed = 5f;
    public TextMeshProUGUI HelthText;


    [Header("보스 전용")]
    public List<string> bossPatternNameLIst = new List<string>();
    public GameObject Boss_Image;
    public GameObject Skill04Effect;
    

    private void Awake()
    {
        Enemy_Name = enemySO.EnemyName;
        Enemy_Levle = enemySO.EnemyLevel;
        Enemy_MaxHelth = enemySO.EnemyMaxHelth;
        Enemy_Damage = enemySO.EnemyDamage;
        Enemy_Type = enemySO.enemyType;
        Enemy_Speed = enemySO.EnemySpeed;
        Enemy_CurrentHelth = Enemy_MaxHelth;
    }

    private void Start()
    {
        if (HitEffect != null) HitEffect.gameObject.SetActive(false);
        if (AttackEffect != null) AttackEffect.gameObject.SetActive(false);
        if (Skill04Effect != null) Skill04Effect.gameObject.SetActive(false);
    }

    private void Update()
    {
        enemyHelthSlider.minValue = 0;
        enemyHelthSlider.maxValue = Enemy_MaxHelth;

        nameText.text = Enemy_Name;
        HelthText.text = $"{Enemy_CurrentHelth} / {Enemy_MaxHelth}";
        HelthUI();
    }

    private void HelthUI()
    {
        enemyHelthSlider.value = Mathf.Lerp(enemyHelthSlider.value, Enemy_CurrentHelth, smoothSpeed * Time.deltaTime);
    }

    public void Hit()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        Sequence seq = DOTween.Sequence();
        seq.Append(sr.DOColor(Color.red, 0.08f));
        seq.Append(sr.DOColor(Color.white, 0.12f));
    }
}
