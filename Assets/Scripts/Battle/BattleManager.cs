using UnityEngine;
using UnityEngine.SceneManagement;

public enum BattleState { Start, PlayerTurn, EnemyTurn, Event, Win, Lose }

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    public BattleState currentState;
    public Character player;
    public Character enemy;

    [Range(0, 100)]
    public int priestEventChance = 40;

    private bool eventTriggered = false;
    private int currentStage = 1;

    [HideInInspector]
    public int plannedEnemyAttackID = -1;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        int playerID = DataManager.SelectedPlayerID;
        if (playerID == 0)
        {
            playerID = PlayerPrefs.GetInt("SavedPlayerID", 1);
        }
        else
        {
            PlayerPrefs.SetInt("SavedPlayerID", playerID);
            PlayerPrefs.Save();
        }

        currentStage = PlayerPrefs.GetInt("CurrentStage", 1);
        Debug.Log($"Battle Start! PlayerID: {playerID}, Stage: {currentStage}");

        // ====================================================
        // [스테이지별 첫 번째 등장 적 ID]
        // 스테이지 1 → 무한의 벼(ID:1) 등장
        // 스테이지 2 → 팥쥐(ID:2) 등장
        // 스테이지 3 → 계모(ID:3) 등장
        // 스테이지 4 → 원님(ID:4) 등장
        // ====================================================
        int firstEnemyID = currentStage; // ID가 스테이지 번호와 일치
        SetupBattle(playerID, firstEnemyID);
    }

    public void SetupBattle(int playerID, int enemyID)
    {
        currentState = BattleState.Start;
        eventTriggered = false;

        player.InitPlayer(playerID);
        enemy.InitEnemy(enemyID);

        Debug.Log($"{enemy.charName} 등장!");

        if (BattleUI.Instance != null)
        {
            BattleUI.Instance.ClearLog();
            BattleUI.Instance.AddLog($"{enemy.charName}이(가) 나타났다!");
            BattleUI.Instance.RefreshSkillButtons();
        }

        PlanNextEnemyAttack();
        StartPlayerTurn();
    }

    public void PlanNextEnemyAttack()
    {
        if (enemy.skillList == null || enemy.skillList.Count == 0) return;
        plannedEnemyAttackID = enemy.skillList[Random.Range(0, enemy.skillList.Count)];

        if (BattleUI.Instance != null)
            BattleUI.Instance.UpdateEnemyIntention(plannedEnemyAttackID);
    }

    void StartPlayerTurn()
    {
        if (CheckGameOver()) return;

        currentState = BattleState.PlayerTurn;
        player.OnTurnStart();

        if (DataManager.Instance.playerDict.ContainsKey(player.id))
            player.currentAP = DataManager.Instance.playerDict[player.id].actionPoint;

        Debug.Log("플레이어 턴 시작.");

        if (BattleUI.Instance != null)
        {
            BattleUI.Instance.AddLog("내 턴이 돌아왔다.");
            BattleUI.Instance.RefreshSkillButtons();
            if (plannedEnemyAttackID != -1)
                BattleUI.Instance.UpdateEnemyIntention(plannedEnemyAttackID);
        }
    }

    public void StartEnemyTurn()
    {
        if (currentState != BattleState.PlayerTurn) return;
        if (CheckGameOver()) return;

        currentState = BattleState.EnemyTurn;

        if (BattleUI.Instance != null)
        {
            BattleUI.Instance.AddLog("적의 턴.");
            BattleUI.Instance.HideEnemyIntention();
        }

        if (enemy.skillList == null || enemy.skillList.Count == 0)
        {
            Invoke(nameof(StartPlayerTurn), 1.0f);
            return;
        }

        if (plannedEnemyAttackID == -1) PlanNextEnemyAttack();

        int randomDamage = Random.Range(3, 8);
        player.TakeDamage(randomDamage);

        if (BattleUI.Instance != null)
            BattleUI.Instance.AddLog($"{enemy.charName}의 공격! {randomDamage} 피해!");

        enemy.UseEnemySkill(plannedEnemyAttackID, player);

        if (CheckGameOver())
        {
            CancelInvoke(nameof(StartPlayerTurn));
            return;
        }

        PlanNextEnemyAttack();
        Invoke(nameof(StartPlayerTurn), 1.0f);
    }

    bool CheckGameOver()
    {
        if (currentState == BattleState.Event) return true;

        if (enemy.currentHP <= 0)
        {
            if (currentState != BattleState.Win)
            {
                currentState = BattleState.Win;
                OnBattleWin();
            }
            return true;
        }

        if (player.currentHP <= 0)
        {
            currentState = BattleState.Lose;
            if (BattleUI.Instance != null)
                BattleUI.Instance.AddLog("눈앞이 깜깜해졌다...");
            if (GameOverUI.Instance != null)
                GameOverUI.Instance.ShowGameOver();
            return true;
        }

        return false;
    }

    void OnBattleWin()
    {
        if (eventTriggered) return;
        eventTriggered = true;

        int defeatedEnemyID = enemy.id;
        Debug.Log($"처치한 적 ID: {defeatedEnemyID}");

        // ====================================================
        // [연속 전투 분기]
        //
        // 무한의 벼(1) 처치 → 공격하는 베(5) 등장 후 연전
        // 팥쥐(2) 처치    → 밑빠진 독(6) 등장 후 연전
        // 공격하는 베(5) 처치 → 스토리(스테이지2)로 이동
        // 밑빠진 독(6) 처치   → 스토리(스테이지3)로 이동
        // 계모(3) 처치    → 스토리(스테이지4)로 이동
        // 원님(4) 처치    → 게임 클리어
        // ====================================================

        if (defeatedEnemyID == 1) // 무한의 벼 처치 → 베 등장
        {
            Debug.Log("무한의 벼 처치 완료! 공격하는 베 등장!");
            if (BattleUI.Instance != null)
                BattleUI.Instance.AddLog("또 다른 존재가 나타났다... 공격하는 베!");
            Invoke(nameof(StartBattle_Loom), 2.0f);
            return;
        }

        if (defeatedEnemyID == 2) // 팥쥐 처치 → 밑빠진 독 등장
        {
            Debug.Log("팥쥐 처치 완료! 밑빠진 독 등장!");
            if (BattleUI.Instance != null)
                BattleUI.Instance.AddLog("또 다른 존재가 나타났다... 밑빠진 독!");
            Invoke(nameof(StartBattle_Pot), 2.0f);
            return;
        }

        // 공격하는 베(5), 밑빠진 독(6), 계모(3), 원님(4) 처치 시
        // → 스테이지 올리고 스토리 또는 게임 클리어로 이동
        currentStage++;
        PlayerPrefs.SetInt("CurrentStage", currentStage);
        PlayerPrefs.Save();

        Debug.Log($"스테이지 클리어! 다음 스테이지: {currentStage}");

        // 원님(4) 처치 후(스테이지가 5가 됨) → 게임 클리어
        if (currentStage > 4)
        {
            Debug.Log("게임 클리어!");
            if (GameClearUI.Instance != null)
                GameClearUI.Instance.ShowGameClear();
            return;
        }

        // 스테이지 이동 전 이벤트 여부 체크 (필요 시 조건 수정 가능)
        GoToNextStory();
    }

    // 공격하는 베(ID: 5) 전투 시작
    private void StartBattle_Loom()
    {
        int playerID = DataManager.SelectedPlayerID;
        SetupBattle(playerID, 5);
    }

    // 밑빠진 독(ID: 6) 전투 시작
    private void StartBattle_Pot()
    {
        int playerID = DataManager.SelectedPlayerID;
        SetupBattle(playerID, 6);
    }

    void StartEvent()
    {
        currentState = BattleState.Event;
        if (EventManager.Instance != null)
            EventManager.Instance.ShowPriestEvent();
    }

    public void EndEvent()
    {
        currentState = BattleState.Start;
        eventTriggered = false;
        GoToNextStory();
    }

    private void GoToNextStory()
    {
        Debug.Log($"스토리 씬으로 이동. 현재 Stage: {currentStage}");
        SceneManager.LoadScene("StoryScene");
    }
}