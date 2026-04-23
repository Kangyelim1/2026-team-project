using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        // [스테이지별 첫 번째 등장 적 세팅 (새로운 ID 기준)]
        // ====================================================
        int firstEnemyID = 1;
        if (currentStage == 1) firstEnemyID = 1;      // 스테이지 1: 무한의 벼 (ID: 1)
        else if (currentStage == 2) firstEnemyID = 3; // 스테이지 2: 팥쥐 (ID: 3)
        else if (currentStage == 3) firstEnemyID = 5; // 스테이지 3: 계모 (ID: 5)
        else if (currentStage == 4) firstEnemyID = 6; // 스테이지 4: 원님 (ID: 6)

        SetupBattle(playerID, firstEnemyID);
    }

    public void SetupBattle(int playerID, int enemyID)
    {
        currentState = BattleState.Start;
        eventTriggered = false;

        player.InitPlayer(playerID);
        enemy.InitEnemy(enemyID);

        Debug.Log($"{enemy.charName} 등장! (ID: {enemyID})");

        // ====================================================
        // [전투 배경 이미지 동적 변경 로직 (요청하신 규칙)]
        // ====================================================
        if (BattleUI.Instance != null && BattleUI.Instance.battleBackground != null)
        {
            string bgName = "Bg_Yard"; // 기본 배경

            if (enemyID == 1) bgName = "Bg_Yard";            // 1번(벼) -> 마당(Bg_Yard)
            else if (enemyID == 2) bgName = "Bg_Morning";    // 2번(베) -> 아침(Bg_Morning)
            else if (enemyID == 3) bgName = "Bg_Morning";    // 3번(팥쥐) -> 아침(Bg_Morning)
            else if (enemyID == 4) bgName = "Bg_Night";      // 4번(독) -> 밤(Bg_Night)
            else if (enemyID == 5) bgName = "Bg_Village";    // 5번(계모) -> 마을(Bg_Village)
            else if (enemyID == 6) bgName = "Bg_Village";    // 6번(원님) -> 마을(Bg_Village)

            Sprite bgSprite = Resources.Load<Sprite>($"Backgrounds/{bgName}");
            if (bgSprite != null)
            {
                BattleUI.Instance.battleBackground.sprite = bgSprite;
            }
            else
            {
                Debug.LogWarning($"전투 배경을 찾을 수 없습니다! 경로: Resources/Backgrounds/{bgName}");
            }
        }
        // ====================================================

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
        // ====================================================

        if (defeatedEnemyID == 1) // 무한의 벼(ID:1) 처치 → 공격하는 베(ID:2) 등장
        {
            Debug.Log("무한의 벼 처치 완료! 공격하는 베 등장!");
            if (BattleUI.Instance != null)
                BattleUI.Instance.AddLog("또 다른 존재가 나타났다... 공격하는 베!");
            Invoke(nameof(StartBattle_Loom), 2.0f);
            return;
        }

        if (defeatedEnemyID == 3) // 팥쥐(ID:3) 처치 → 밑빠진 독(ID:4) 등장
        {
            Debug.Log("팥쥐 처치 완료! 밑빠진 독 등장!");
            if (BattleUI.Instance != null)
                BattleUI.Instance.AddLog("또 다른 존재가 나타났다... 밑빠진 독!");
            Invoke(nameof(StartBattle_Pot), 2.0f);
            return;
        }

        // ====================================================
        // [스테이지 클리어 처리]
        // 베(2), 독(4), 계모(5), 원님(6)을 처치했을 때 여기로 와서 스토리를 진행함
        // ====================================================
        currentStage++;
        PlayerPrefs.SetInt("CurrentStage", currentStage);
        PlayerPrefs.Save();

        Debug.Log($"스테이지 클리어! 다음 스테이지: {currentStage}");

        // 4스테이지(원님) 클리어 후 게임 클리어
        if (currentStage > 4)
        {
            Debug.Log("게임 클리어!");
            if (GameClearUI.Instance != null)
                GameClearUI.Instance.ShowGameClear();
            return;
        }

        // 바로 다음 스토리로 넘어갑니다.
        GoToNextStory();
    }

    // 공격하는 베(ID: 2) 연전 셋업
    private void StartBattle_Loom()
    {
        int playerID = DataManager.SelectedPlayerID;
        SetupBattle(playerID, 2);
    }

    // 밑빠진 독(ID: 4) 연전 셋업
    private void StartBattle_Pot()
    {
        int playerID = DataManager.SelectedPlayerID;
        SetupBattle(playerID, 4);
    }

    private void GoToNextStory()
    {
        Debug.Log($"스토리 씬으로 이동. 현재 Stage: {currentStage}");
        SceneManager.LoadScene("StoryScene");
    }

    // ====================================================
    // 이벤트 씬(회복/스킬 강화) 진행 관련 함수 (추가)
    // ====================================================
    void StartEvent()
    {
        currentState = BattleState.Event;
        Debug.Log("이벤트 씬 진입");
        if (EventManager.Instance != null)
        {
            EventManager.Instance.ShowPriestEvent();
        }
    }

    public void EndEvent()
    {
        Debug.Log("이벤트 종료 후 스토리 이동 준비");
        currentState = BattleState.Start;
        eventTriggered = false;
        GoToNextStory();
    }
}