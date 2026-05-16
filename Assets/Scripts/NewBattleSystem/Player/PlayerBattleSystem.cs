using DG.Tweening;
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

    public void Update()
    {
        if (isAttack || !_battleManager.isPlayerTurn) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) SkillAttack01();
        if (Input.GetKeyDown(KeyCode.Alpha2)) SkillAttack02();
        if (Input.GetKeyDown(KeyCode.Alpha3)) SkillAttack03();
        if (Input.GetKeyDown(KeyCode.Alpha4)) SkillAttack04();

        PlayerAttackTarget();
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

        _playerAttackSystem.TargetEnemy = _battleManager.spawnedEnemy.gameObject;

        if (_playerAttackSystem.TargetEnemy)
        {
            isTarget = true;
            Debug.Log($"[PlayerBattleSystem] 타겟 선택: {_playerAttackSystem.TargetEnemy.name}");
            ButtonHose.gameObject.SetActive(true);
        }
        else
        {
            ButtonHose.gameObject.SetActive(false);
        }     
    }

    void SkillAttack01()
    {
        if (!_battleManager.isPlayerTurn || isAttack) return;
        _playerAttackSystem = Object.FindAnyObjectByType<PlayerAttackSystem>();

        if (_playerAttackSystem == null) return;

        AutoSelectEnemey();
        StartCoroutine(_playerAttackSystem.isNomalAttack());
        Debug.Log("Enemy 체력 감소 플레이어 공격 진행 완료");
    }

    void SkillAttack02()
    {
        if (!_battleManager.isPlayerTurn || isAttack) return;
        _playerAttackSystem = Object.FindAnyObjectByType<PlayerAttackSystem>();
        if (_playerAttackSystem == null) return;
        
        Debug.Log("스킬 공격2 진행");
        _playerAttackSystem.SkillAttack(_playerSystem.player_Name, _playerSystem.skillAttack02Type);
    }

    void SkillAttack03()
    {
        if (!_battleManager.isPlayerTurn || isAttack) return;
        _playerAttackSystem = Object.FindAnyObjectByType<PlayerAttackSystem>();
        if (_playerAttackSystem == null) return;

        Debug.Log("스킬 공격3 진행");
        _playerAttackSystem.SkillAttack(_playerSystem.player_Name, _playerSystem.skillAttack03Type);
    }

    void SkillAttack04()
    {
        if (!_battleManager.isPlayerTurn || isAttack 
            || _playerAttackSystem.currentSkill04Stemina < _playerAttackSystem.Skill04Stemina) return;

        _playerAttackSystem = Object.FindAnyObjectByType<PlayerAttackSystem>();
        if (_playerAttackSystem == null) return;

        Debug.Log("스킬 공격4 진행");
        _playerAttackSystem.SkillAttack(_playerSystem.player_Name, _playerSystem.skillAttack04Type);
    }
}
