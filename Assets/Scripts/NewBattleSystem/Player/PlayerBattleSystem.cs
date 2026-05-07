using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBattleSystem : MonoBehaviour
{
    [Header("외부 스크립트")]
    public NewBattleManager _battleManager;
    public PlayerAttackSystem _playerAttackSystem;

    public GameObject TargetEnemy;
    public Button AttackButton;

    public bool isTarget;

    public void Awake()
    {
        _battleManager = Object.FindAnyObjectByType<NewBattleManager>();
    }

    public void Start()
    {
        AttackButton.onClick.AddListener(Attack);
    }

    public void PlayerAttackTarget()
    {
        AutoSelectEnemey();
    }

    public void AutoSelectEnemey()
    {
        if (_battleManager.isPlayerTurn || TargetEnemy == null)
        {
            TargetEnemy = _battleManager.spawnedEnemy.gameObject;
            Debug.Log($"타겟: {TargetEnemy.name}");
            isTarget = true;
        }
    }

    void Attack()
    {
        if (!_battleManager.isPlayerTurn) return;

        _playerAttackSystem = Object.FindAnyObjectByType<PlayerAttackSystem>();

        StartCoroutine(_playerAttackSystem.isAttack());
        Debug.Log("Enemy 체력 감소 플레이어 공격 진행 완료");
    }
}
