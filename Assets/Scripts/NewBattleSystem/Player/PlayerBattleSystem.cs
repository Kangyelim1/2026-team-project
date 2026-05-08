using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBattleSystem : MonoBehaviour
{
    [Header("외부 스크립트")]
    public NewBattleManager _battleManager;
    public PlayerAttackSystem _playerAttackSystem;

    [Header("스킬 버튼")]
    public Button AttackButton;
    public Button NextTurnButton;
    public GameObject ButtonHose;

    [Header("몬스터")]
    public GameObject TargetEnemy;

    [Header("전퉁 시작전 오프닝 Poition")]
    public Transform playerOpPoint;
    public Transform EnemyOpPoint;
    

    public bool isTarget;

    public void Awake()
    {
        _battleManager = Object.FindAnyObjectByType<NewBattleManager>();
    }

    public void Start()
    {
        AttackButton.onClick.AddListener(Attack);
        NextTurnButton.onClick.AddListener(() => { _battleManager.NextTurn(); });
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

    void Attack()
    {
        if (!_battleManager.isPlayerTurn) return;

        _playerAttackSystem = Object.FindAnyObjectByType<PlayerAttackSystem>();

        if (_playerAttackSystem == null) return;

        StartCoroutine(_playerAttackSystem.isAttack());
        Debug.Log("Enemy 체력 감소 플레이어 공격 진행 완료");
    }
}
