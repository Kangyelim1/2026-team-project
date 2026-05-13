using System.Collections;
using Unity.VisualScripting;
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

    [Header("몬스터")]
    public GameObject TargetEnemy;

    [Header("전퉁 시작전 오프닝 Poition")]
    public Transform playerOpPoint;
    public Transform EnemyOpPoint;


    [Header("플레이어 스킬 아이콘")]
    public Image Skill01;
    public Image Skill02;
    public Image Skill03;
    public Image Skill04;

    public bool isTarget;
    public bool isAttack;

    public void Awake()
    {
        _battleManager = Object.FindAnyObjectByType<NewBattleManager>();
    }

    public void Start()
    {
        _playerSystem = Object.FindAnyObjectByType<PlayerSystem>();
        AttackButton01.onClick.AddListener(SkillAttack01);
        AttackButton02.onClick.AddListener(SkillAttack02);
        AttackButton03.onClick.AddListener(SkillAttack03);
        AttackButton04.onClick.AddListener(SkillAttack04);
        NextTurnButton.onClick.AddListener(() => { _battleManager.NextTurn();});

        ChangeSprite();
    }

    public void ChangeSprite()
    {
        Skill01.sprite = _playerSystem.skill01;
        Skill02.sprite = _playerSystem.skill02;
        Skill03.sprite = _playerSystem.skill03;
        Skill04.sprite = _playerSystem.skill04;
    }

    public void PlayerAttackTarget()
    {
        AutoSelectEnemey();
    }

    public void AutoSelectEnemey()
    {
        if (_battleManager.spawnedEnemy == null) return;

        TargetEnemy = _battleManager.spawnedEnemy.gameObject;
        isTarget = true;
        Debug.Log($"[PlayerBattleSystem] 타겟 선택: {TargetEnemy.name}");
    }

    void SkillAttack01()
    {
        if (!_battleManager.isPlayerTurn || isAttack) return;

        _playerAttackSystem = Object.FindAnyObjectByType<PlayerAttackSystem>();

        if (_playerAttackSystem == null) return;

        ButtonHose.gameObject.SetActive(false);
        StartCoroutine(_playerAttackSystem.isNomalAttack());
        Debug.Log("Enemy 체력 감소 플레이어 공격 진행 완료");
    }

    void SkillAttack02()
    {
        Debug.Log("스킬 공격2 진행");
    }

    void SkillAttack03()
    {
        Debug.Log("스킬 공격3 진행");
    }

    void SkillAttack04()
    {
        Debug.Log("스킬 공격4 진행");
    }
}
