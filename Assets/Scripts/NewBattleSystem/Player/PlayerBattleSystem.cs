using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBattleSystem : MonoBehaviour
{
    [Header("외부 스크립트")]
    public NewBattleManager _battleManager;
    public PlayerAttackSystem _playerAttackSystem;
    public PlayerSystem _playerSystem;

    [Header("스킬 버튼")]
    public Button AttackButton01;
    public Button AttackButton02;
    public Button AttackButton03;
    public Button AttackButton04;

    public Button NextTurnButton;
    public GameObject ButtonHose;

    [Header("전퉁 시작전 오프닝 Poition")]
    public Transform playerOpPoint;
    public Transform EnemyOpPoint;

    [Header("플레이어 스킬 아이콘")]
    public Image Skill01;
    public Image Skill02;
    public Image Skill03;
    public Image Skill04;

    public Image Skill04Gauge;

    public bool isTarget;
    public bool isAttack;

    private float currentSkill04GaugeValue = -1f;
    private bool isSkill04ReadyEffectPlayed;

    public void Awake()
    {
        _battleManager = Object.FindAnyObjectByType<NewBattleManager>();
    }

    public void Start()
    {
        _playerSystem = Object.FindAnyObjectByType<PlayerSystem>();
        _playerAttackSystem = Object.FindAnyObjectByType<PlayerAttackSystem>();

        AttackButton02.onClick.AddListener(SkillAttack02);
        AttackButton03.onClick.AddListener(SkillAttack03);
        AttackButton04.onClick.AddListener(SkillAttack04);
        NextTurnButton.onClick.AddListener(() => { _battleManager.NextTurn(); });

        ChangeSprite();

        if (Skill04Gauge != null)
        {
            Skill04Gauge.fillAmount = 0f;
        }
    }

    public void Update()
    {
        if (_playerAttackSystem == null)
        {
            _playerAttackSystem = Object.FindAnyObjectByType<PlayerAttackSystem>();
        }

        UpdateSkill04Gauge();

        if (isAttack || !_battleManager.isPlayerTurn) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) SkillAttack02();
        if (Input.GetKeyDown(KeyCode.Alpha2)) SkillAttack03();
        if (Input.GetKeyDown(KeyCode.Alpha3)) SkillAttack04();

        PlayerAttackTarget();
    }

    public void ChangeSprite()
    {
        if (_playerSystem == null) return;

        if (Skill01 != null) Skill01.sprite = _playerSystem.skill01;
        if (Skill02 != null) Skill02.sprite = _playerSystem.skill02;
        if (Skill03 != null) Skill03.sprite = _playerSystem.skill03;
        if (Skill04 != null) Skill04.sprite = _playerSystem.skill04;
    }

    public void PlayerAttackTarget()
    {
        AutoSelectEnemey();
    }

    public void AutoSelectEnemey()
    {
        if (_battleManager == null || _battleManager.spawnedEnemy == null) return;

        if (_playerAttackSystem == null)
        {
            _playerAttackSystem = Object.FindAnyObjectByType<PlayerAttackSystem>();
        }

        if (_playerAttackSystem == null) return;

        _playerAttackSystem.TargetEnemy = _battleManager.spawnedEnemy.gameObject;

        if (_playerAttackSystem.TargetEnemy != null)
        {
            isTarget = true;
            Debug.Log($"[PlayerBattleSystem] 타겟 선택: {_playerAttackSystem.TargetEnemy.name}");
        }
    }

    private void PlayButtonClickEffect(Button button)
    {
        if (button == null) return;

        Transform buttonTransform = button.transform;

        buttonTransform.DOKill();

        Sequence seq = DOTween.Sequence();
        seq.Append(buttonTransform.DOScale(0.9f, 0.07f).SetEase(Ease.OutQuad));
        seq.Append(buttonTransform.DOScale(1.1f, 0.12f).SetEase(Ease.OutBack));
        seq.Append(buttonTransform.DOScale(1f, 0.08f).SetEase(Ease.OutQuad));
    }

    private void PlaySkill04ReadyEffect()
    {
        if (AttackButton04 == null) return;

        Transform buttonTransform = AttackButton04.transform;

        buttonTransform.DOKill();
        buttonTransform.DOPunchScale(Vector3.one * 0.18f, 0.4f, 8, 0.8f);

        if (AttackButton04.image != null)
        {
            AttackButton04.image.DOKill();
            AttackButton04.image.DOFade(0.45f, 0.12f).SetLoops(2, LoopType.Yoyo);
        }
    }

    void SkillAttack02()
    {
        if (_battleManager == null || !_battleManager.isPlayerTurn || isAttack) return;

        _playerAttackSystem = Object.FindAnyObjectByType<PlayerAttackSystem>();
        if (_playerAttackSystem == null) return;

        PlayButtonClickEffect(AttackButton02);

        if (ButtonHose != null) ButtonHose.gameObject.SetActive(false);

        isAttack = true;
        Debug.Log("스킬 공격2 진행");

        AutoSelectEnemey();
        _playerAttackSystem.SkillAttack(_playerSystem.player_Name, _playerSystem.skillAttack02Type);
    }

    void SkillAttack03()
    {
        if (_battleManager == null || !_battleManager.isPlayerTurn || isAttack) return;

        _playerAttackSystem = Object.FindAnyObjectByType<PlayerAttackSystem>();
        if (_playerAttackSystem == null) return;

        PlayButtonClickEffect(AttackButton03);

        if (ButtonHose != null) ButtonHose.gameObject.SetActive(false);

        isAttack = true;
        Debug.Log("스킬 공격3 진행");

        AutoSelectEnemey();
        _playerAttackSystem.SkillAttack(_playerSystem.player_Name, _playerSystem.skillAttack03Type);
    }

    void SkillAttack04()
    {
        if (_battleManager == null || !_battleManager.isPlayerTurn || isAttack) return;

        _playerAttackSystem = Object.FindAnyObjectByType<PlayerAttackSystem>();
        if (_playerAttackSystem == null) return;

        if (_playerAttackSystem.currentSkill04Stemina < _playerAttackSystem.Skill04Stemina)
        {
            PlayButtonClickEffect(AttackButton04);
            return;
        }

        PlayButtonClickEffect(AttackButton04);

        if (Skill04Gauge != null)
        {
            Skill04Gauge.DOKill();
            Skill04Gauge.DOFillAmount(0f, 0.3f).SetEase(Ease.InQuad);
        }

        currentSkill04GaugeValue = 0f;
        isSkill04ReadyEffectPlayed = false;

        if (ButtonHose != null) ButtonHose.gameObject.SetActive(false);

        isAttack = true;
        Debug.Log("스킬 공격4 진행");

        AutoSelectEnemey();
        _playerAttackSystem.SkillAttack(_playerSystem.player_Name, _playerSystem.skillAttack04Type);
    }

    private void UpdateSkill04Gauge()
    {
        if (_playerAttackSystem == null || Skill04Gauge == null) return;
        if (_playerAttackSystem.Skill04Stemina <= 0) return;

        float value = (float)_playerAttackSystem.currentSkill04Stemina / _playerAttackSystem.Skill04Stemina;
        value = Mathf.Clamp01(value);

        if (Mathf.Approximately(currentSkill04GaugeValue, value)) return;

        currentSkill04GaugeValue = value;

        Skill04Gauge.DOKill();
        Skill04Gauge.DOFillAmount(value, 0.35f).SetEase(Ease.OutQuad);

        if (value >= 1f)
        {
            if (!isSkill04ReadyEffectPlayed)
            {
                isSkill04ReadyEffectPlayed = true;
                PlaySkill04ReadyEffect();
            }
        }
        else
        {
            isSkill04ReadyEffectPlayed = false;
        }
    }
}