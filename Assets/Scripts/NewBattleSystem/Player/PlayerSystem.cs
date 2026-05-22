using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSystem : MonoBehaviour
{
    public PlayerSO playerSO; 
    public string player_Name;
    public int player_MaxHelth;
    public int player_Damage;
    public int player_Defense;
    public int player_Speed;
    public GameObject playerPrefab;
    public Animator playerAnimator;

    [Header("버튼")]
    public Sprite skill01;
    public Sprite skill02;
    public Sprite skill03;
    public Sprite skill04;

    [Header("AttackType")]
    public string skillAttack02Type;
    public string skillAttack03Type;
    public string skillAttack04Type;

    [Header("Helth")]
    public int player_CurrentHelth;
    public Slider playerHelthSlider;
    [SerializeField] private float smoothSpeed = 5f;
    public TextMeshProUGUI playerHelthText;

    [Header("VFX")]
    public GameObject HitEffect;
    public GameObject DefanseEffect;
    public GameObject DebuffEffect;

    private void Awake()
    {
        player_Name = playerSO.playerName;
        player_MaxHelth = playerSO.playerMaxHelth;
        player_Damage = playerSO.playerDamage;
        player_Speed = playerSO.playerSpeed;

        skill01 = playerSO.Skill01Image;
        skill02 = playerSO.Skill02Image;
        skill03 = playerSO.Skill03Image;
        skill04 = playerSO.Skill04Image;

        skillAttack02Type = playerSO.SkillAttackType02;
        skillAttack03Type = playerSO.SkillAttackType03;
        skillAttack04Type = playerSO.SkillAttackType04;

        player_CurrentHelth = player_MaxHelth;
    }

    private void Start()
    {
        if(HitEffect != null) HitEffect.gameObject.SetActive(false);
        if (DefanseEffect != null)  DefanseEffect.gameObject.SetActive(false);
        if (DebuffEffect != null) DebuffEffect.gameObject.SetActive(false);
    }

    private void Update()
    {
        playerHelthSlider.minValue = 0;
        playerHelthSlider.maxValue = player_MaxHelth;
        playerHelthText.text = $"{player_CurrentHelth} / {player_MaxHelth}";

        HelthUI();
    }

    private void HelthUI()
    {
        playerHelthSlider.value = Mathf.Lerp(player_CurrentHelth, player_CurrentHelth, smoothSpeed * Time.deltaTime);
    }

    public void Hit()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        Sequence seq = DOTween.Sequence();
        seq.Append(sr.DOColor(Color.red, 0.08f));
        seq.Append(sr.DOColor(Color.white, 0.12f));
    }
}
