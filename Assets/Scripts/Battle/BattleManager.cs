using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//이벤트 추가
public enum BattleState { Start, PlayerTurn, EnemyTurn, Event, Win, Lose }

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;
    public BattleState currentState;

    public Character player;
    public Character enemy;

    // 스토리 리스트
    public List<StoryData> storyList = new List<StoryData>();

    // 현재 스토리
    private StoryData currentStory;

    // 현재 적 인덱스
    private int currentEnemyIndex = 0;


    void Awake() { Instance = this; }

    void Start()
    {
        int playerID = DataManager.SelectedPlayerID != 0 ? DataManager.SelectedPlayerID : 1;

        // 테스트용 적 리스트
        SetupStory(1, playerID);


        SetupBattle(playerID, 1); // 선택된 캐릭터와 1번 적 조우 테스트
    }
    //스토리 시작
    public void SetupStory(int storyID, int playerID)
    {
        // 스토리 찾기
        foreach (var story in storyList)
        {
            if (story.storyID == storyID)
            {
                currentStory = story;
                break;
            }
        }

        currentEnemyIndex = 0;

        SetupBattle(playerID, currentStory.enemyList[currentEnemyIndex]);
    }
    // 1. 스테이지 입장 및 조우
    public void SetupBattle(int playerID, int enemyID)
    {
        currentState = BattleState.Start;
        player.InitPlayer(playerID);
        enemy.InitEnemy(enemyID);

        Debug.Log($"{enemy.charName}와 조우했습니다!");
        StartPlayerTurn();
    }

    // 2. 플레이어 턴 시작
    void StartPlayerTurn()
    {
        if (CheckGameOver()) return;
        currentState = BattleState.PlayerTurn;
        player.currentAP = 3;  // 플레이어 AP (일단 임의로 3으로 지정)
        Debug.Log("플레이어의 턴입니다.");
    }

    // 3. 적 턴 시작 (AI 행동)
    public void StartEnemyTurn()
    {
        if (CheckGameOver()) return;
        currentState = BattleState.EnemyTurn;

        // 간단한 적 AI: 랜덤 스킬 사용
        int randomSkillID = enemy.skillList[Random.Range(0, enemy.skillList.Count)];
        enemy.UseEnemySkill(randomSkillID, player);

        // 적의 공격으로 플레이어가 죽었는지 즉시 확인
        if (CheckGameOver())
        {
            CancelInvoke("StartPlayerTurn"); // 턴 전환 취소
            return;
        }

        // 공격 후 다시 플레이어 턴으로
        Invoke("StartPlayerTurn", 1.0f);
    }
    bool CheckGameOver()
    {

        if (enemy.currentHP <= 0)
        {

            Debug.Log($"{enemy.charName} 처치!");

            currentEnemyIndex++;

            // 이벤트 먼저 실행
            StartEvent();
            return true;
        }

        if (player.currentHP <= 0)
        {
            currentState = BattleState.Lose;
            Debug.Log("패배... 게임 오버.");
            return true;
        }

        return false;
    }

    // 다음 적 불러오는 함수
    void NextEnemy()
    {

        enemy.InitEnemy(currentStory.enemyList[currentEnemyIndex]);

        Debug.Log($"{enemy.charName} 등장!");

        StartPlayerTurn();
    }
    void StartEvent()
    {
        currentState = BattleState.Event;
        Debug.Log("신녀가 등장했습니다!");

        // UI 호출
        EventManager.Instance.ShowPriestEvent();
    }

    public void EndEvent()
    {
        // 마지막 적 체크
        if (currentEnemyIndex >= currentStory.enemyList.Count)
        {
            currentState = BattleState.Win;
            Debug.Log("스토리 클리어!");
            return;
        }

        Debug.Log("다음 적 등장!");

        Invoke("NextEnemy", 1.0f);
    }

    // 4. 승리/패배 판정 (임시로 가려둘게요,,)
    //bool CheckGameOver() {
    //    if (enemy.currentHP <= 0) {
    //        currentState = BattleState.Win;
    //        Debug.Log("승리! 보상을 획득합니다.");
    //        return true;

    //    }
    //    if (player.currentHP <= 0) {
    //        currentState = BattleState.Lose;
    //        Debug.Log("패배... 게임 오버.");
    //        return true;
    //    }
    //    return false;
    //}



}