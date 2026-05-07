using UnityEngine;

public class NewBattleManager : MonoBehaviour
{
    [Header("데이터")]
    public PlayerSystem playerData;
    public EnemySystem enemyData;

    [Header("전투 상태")]
    public PlayerBattleSystem _playerBattleSystem;

    [Header("스폰 위치")]
    public Transform Player_SpawnPoint;
    public Transform Enemy_SpawnPoint;

    public GameObject spawnedPlayer;
    public GameObject spawnedEnemy;

    public bool isPlayerTurn = true;
    public bool isGameEnd = false;

    private void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        SpawnPlayer();
        SpawnEnemy();

        _playerBattleSystem = Object.FindAnyObjectByType<PlayerBattleSystem>();

        isPlayerTurn = true;

        Debug.Log("게임 시작");
        Debug.Log("플레이어 턴 시작");

        Turn();
    }

    private void SpawnPlayer()
    {
        if (playerData == null)
        {
            Debug.LogError("playerData가 비어 있습니다.");
            return;
        }

        if (playerData.playerPrefab == null)
        {
            Debug.LogError("playerPrefab이 비어 있습니다.");
            return;
        }

        spawnedPlayer = Instantiate(
            playerData.playerPrefab,
            Player_SpawnPoint.position,
            Player_SpawnPoint.rotation
        );
    }

    private void SpawnEnemy()
    {
        if (enemyData == null || enemyData.EnemyPrefab == null) return;

        spawnedEnemy = Instantiate(enemyData.EnemyPrefab, Enemy_SpawnPoint.position, Enemy_SpawnPoint.rotation);

        Debug.Log($"{spawnedEnemy.name} 적 생성");
    }

    public void Turn()
    {
        if (isGameEnd) return;

        if (isPlayerTurn)
            PlayerTurn();
        else
            EnemyTurn();
    }

    private void PlayerTurn()
    {
        if (spawnedPlayer == null)
        {
            Debug.Log("플레이어가 없음 게임 패배");
            EndGame(false);
            return;
        }

        Debug.Log($"{spawnedPlayer.name} 플레이어 공격");

        if (_playerBattleSystem != null && !_playerBattleSystem.isTarget)
        {
            _playerBattleSystem.PlayerAttackTarget();
        }
    }

    public void EndPlayerTurn()
    {
        isPlayerTurn = false;
        Turn();
    }

    private void EnemyTurn()
    {
        if (spawnedEnemy == null)
        {
            Debug.Log("적이 없음 게임 승리");
            EndGame(true);
            return;
        }

        Debug.Log($"{spawnedEnemy.name} 적 공격");

        EnemyAttackSystem currentEnemyAI = spawnedEnemy.GetComponent<EnemyAttackSystem>();

        if (currentEnemyAI != null)
        {
            currentEnemyAI.AutoSelectPlayer();
        }
        else
        {
            EndEnemyTurn();
        }
    }

    public void EndEnemyTurn()
    {
        isPlayerTurn = true;
        Turn();
    }

    public void EndGame(bool isVictory)
    {
        if (isGameEnd) return;
        isGameEnd = true;

        if (isVictory)
            Debug.Log("[BattleManager] 전투 승리!");
        else
            Debug.Log("[BattleManager] 전투 패배...");
        //Debug.Log("게임 종료");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReturnToField();
        }
        else
        {
            Debug.LogError("[BattleManager] GameManager 인스턴스를 찾을 수 없습니다. 씬 복귀 불가.");
        }
    }
}