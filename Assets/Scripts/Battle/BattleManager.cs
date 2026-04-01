using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { Start, PlayerTurn, EnemyTurn, Win, Lose }

public class BattleManager : MonoBehaviour 
{
    public static BattleManager Instance;
    public BattleState currentState;

    public Character player;
    public Character enemy;

    void Awake() { Instance = this; }

    void Start() {
        SetupBattle(1, 1); // 1번 플레이어와 1번 적 조우 테스트
    }

    // 1. 스테이지 입장 및 조우
    public void SetupBattle(int playerID, int enemyID) {
        currentState = BattleState.Start;
        player.InitPlayer(playerID);
        enemy.InitEnemy(enemyID);
        
        Debug.Log($"{enemy.charName}와 조우했습니다!");
        StartPlayerTurn();
    }

    // 2. 플레이어 턴 시작
    void StartPlayerTurn() {
        if (CheckGameOver()) return;
        currentState = BattleState.PlayerTurn;
        Debug.Log("플레이어의 턴입니다.");
    }

    // 3. 적 턴 시작 (AI 행동)
    public void StartEnemyTurn() {
        if (CheckGameOver()) return;
        currentState = BattleState.EnemyTurn;
        
        // 간단한 적 AI: 랜덤 스킬 사용
        int randomSkillID = enemy.skillList[Random.Range(0, enemy.skillList.Count)];
        enemy.UseSkill(randomSkillID, player);
        
        // 공격 후 다시 플레이어 턴으로
        Invoke("StartPlayerTurn", 1.0f);
    }

    // 4. 승리/패배 판정
    bool CheckGameOver() {
        if (enemy.currentHP <= 0) {
            currentState = BattleState.Win;
            Debug.Log("승리! 보상을 획득합니다.");
            return true;
        }
        if (player.currentHP <= 0) {
            currentState = BattleState.Lose;
            Debug.Log("패배... 게임 오버.");
            return true;
        }
        return false;
    }
}