using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class NewBattleManager : MonoBehaviour
{
    [Header("데이터")]
    public List<EnemySystem> enemyData = new List<EnemySystem>();
    public List<PlayerSystem> playerData = new List<PlayerSystem>();

    [Header("전투 상태")]
    public int EnemyDieCount;
    public int currentEnemyCount;
    public PlayerBattleSystem _playerBattleSystem;
    public EnemyAttackSystem _enemyAttackSystem;

    [Header("스폰 위치")]
    public List<Transform> Player_SpawnPoint;
    public List<Transform> Enemy_SpawnPoint;

    public List<GameObject> spawnedPlayers = new List<GameObject>();
    public List<GameObject> SpawnEnemy = new List<GameObject>();

    public int currentPlayerTurnIndex = 0;
    public int currentEnemyTurnIndex = 0;

    public bool isPlayerTurn = true;
    public bool isGameEnd = false;

    private void Start()
    {
        StartGame();
        _playerBattleSystem = Object.FindAnyObjectByType<PlayerBattleSystem>();
    }

    private void StartGame()
    {
        SpawnPlayers();
        SpawnEnemies();

        isPlayerTurn = true;
        currentPlayerTurnIndex = 0;
        currentEnemyTurnIndex = 0;

        Debug.Log("게임 시작");
        Debug.Log("플레이어 턴 시작");
        Turn();
    }

    private void SpawnPlayers()
    {
        spawnedPlayers.Clear();

        int spawnCount = Mathf.Min(playerData.Count, Player_SpawnPoint.Count);

        for (int i = 0; i < spawnCount; i++)
        {
            if (playerData[i] == null) continue;
            if (playerData[i].playerPrefab == null) continue;

            GameObject player = Instantiate(playerData[i].playerPrefab, Player_SpawnPoint[i].position, Player_SpawnPoint[i].rotation);
            spawnedPlayers.Add(player);
        }
    }

    private void SpawnEnemies()
    {
        SpawnEnemy.Clear();

        currentEnemyCount = 3;
        int spawnCount = Mathf.Min(currentEnemyCount, Enemy_SpawnPoint.Count);

        for (int i = 0; i < spawnCount; i++)
        {
            EnemySystem randomEnemy = enemyData[Random.Range(0, enemyData.Count)];

            if (randomEnemy == null) continue;
            if (randomEnemy.EnemyPrefab == null) continue;

            GameObject enemy = Instantiate(randomEnemy.EnemyPrefab, Enemy_SpawnPoint[i].position,Enemy_SpawnPoint[i].rotation);
            SpawnEnemy.Add(enemy);
        }

        EnemyDieCount = 0;

        Debug.Log($"적 {SpawnEnemy.Count}명 생성");
    }

    public void Turn()
    {
        if (isGameEnd) return;

        if (isPlayerTurn) PlayerTurn();
        else EnemyTurn();
    }

    private void PlayerTurn()
    {
        if (spawnedPlayers.Count <= 0)
        {
            Debug.Log("플레이어가 없음 게임 패배");
            isGameEnd = true;
            return;
        }

        GameObject currentPlayer = spawnedPlayers[currentPlayerTurnIndex];

        Debug.Log($"{currentPlayer.name} 플레이어 공격");

        if (!_playerBattleSystem.isTarget)
        {
            _playerBattleSystem.PlayerAttackTarget();
        } 

    }

    public void EndPlayerTurn()
    {
        currentPlayerTurnIndex++;

        if (currentPlayerTurnIndex >= spawnedPlayers.Count)
        {
            currentPlayerTurnIndex = 0;
            isPlayerTurn = false;
        }
        Turn();
    }

    private void EnemyTurn()
    {
        if (SpawnEnemy.Count <= 0) return;

        if (currentEnemyTurnIndex >= SpawnEnemy.Count)
        {
            currentEnemyTurnIndex = 0;
            isPlayerTurn = true;
            Turn();
            return;
        }

        GameObject currentEnemy = SpawnEnemy[currentEnemyTurnIndex];
        Debug.Log($"{currentEnemy.name} 적 공격 (인덱스: {currentEnemyTurnIndex})");

        EnemyAttackSystem currentEnemyAI = currentEnemy.GetComponent<EnemyAttackSystem>();

        if (currentEnemyAI != null) currentEnemyAI.AutoSelectPlayer();
        else EndEnemyTurn();
    }

    public void EndEnemyTurn()
    {
        currentEnemyTurnIndex++;
        Turn();
    }

    public void EnemyDie(GameObject enemy)
    {
        if (SpawnEnemy.Contains(enemy))
        {
            SpawnEnemy.Remove(enemy);
        }

        Destroy(enemy);

        EnemyDieCount++;

        Debug.Log($"적 사망 / 현재 웨이브 사망 수 : {EnemyDieCount}");

        if (EnemyDieCount >= currentEnemyCount)
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        isGameEnd = true;
        Debug.Log("게임 종료");
    }
}