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

        // ★[수정된 로직] StoryManager에서 저장한 스토리에 맞는 적 ID를 불러옵니다.
        int enemyID = PlayerPrefs.GetInt("TargetEnemyID", currentStage);

        Debug.Log($"Battle Start! SelectedPlayerID: {playerID}, CurrentStage: {currentStage}, TargetEnemyID: {enemyID}");

        // 스토리에 맞는 적 ID를 매개변수로 넣어 전투를 세팅합니다.
        SetupBattle(playerID, enemyID);
    }

    public void SetupBattle(int playerID, int enemyID)
    {
        currentState = BattleState.Start;
        player.InitPlayer(playerID);
        enemy.InitEnemy(enemyID);

        Debug.Log($"전투 시작: {enemy.charName} 등장!");

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
        {
            BattleUI.Instance.UpdateEnemyIntention(plannedEnemyAttackID);
        }
    }

    void StartPlayerTurn()
    {
        if (CheckGameOver()) return;

        currentState = BattleState.PlayerTurn;
        player.OnTurnStart();

        if (DataManager.Instance.playerDict.ContainsKey(player.id))
        {
            player.currentAP = DataManager.Instance.playerDict[player.id].actionPoint;
        }

        if (BattleUI.Instance != null)
        {
            BattleUI.Instance.AddLog("플레이어의 턴!");
            BattleUI.Instance.RefreshSkillButtons();
            if (plannedEnemyAttackID != -1)
            {
                BattleUI.Instance.UpdateEnemyIntention(plannedEnemyAttackID);
            }
        }
    }

    public void StartEnemyTurn()
    {
        if (currentState != BattleState.PlayerTurn) return;
        if (CheckGameOver()) return;

        currentState = BattleState.EnemyTurn;

        if (BattleUI.Instance != null)
        {
            BattleUI.Instance.AddLog("적의 턴...");
            BattleUI.Instance.HideEnemyIntention();
        }

        if (enemy.skillList == null || enemy.skillList.Count == 0)
        {
            Debug.LogWarning("적에게 스킬이 없습니다.");
            Invoke(nameof(StartPlayerTurn), 1.0f);
            return;
        }

        if (plannedEnemyAttackID == -1)
        {
            PlanNextEnemyAttack();
        }

        Debug.Log($"Enemy Turn plannedEnemyAttackID : {plannedEnemyAttackID}");
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
            Debug.Log("패배했습니다.");
            if (BattleUI.Instance != null) BattleUI.Instance.AddLog("패배했습니다...");
            if (GameOverUI.Instance != null) GameOverUI.Instance.ShowGameOver();
            return true;
        }
        return false;
    }

    void OnBattleWin()
    {
        if (eventTriggered) return;
        eventTriggered = true;

        currentStage++;
        PlayerPrefs.SetInt("CurrentStage", currentStage);
        PlayerPrefs.Save();

        int winCount = currentStage - 1;
        Debug.Log($"승리! 현재 winCount: {winCount}");

        if (winCount >= 6)
        {
            Debug.Log("게임 클리어!");
            if (GameClearUI.Instance != null) GameClearUI.Instance.ShowGameClear();
            return;
        }

        if (winCount == 2 || winCount == 4)
        {
            Debug.Log("이벤트 발생 조건 충족!");
            StartEvent();
        }
        else
        {
            Debug.Log("이벤트 없음, 다음 스토리로 이동.");
            EndEvent();
        }
    }

    void StartEvent()
    {
        currentState = BattleState.Event;
        Debug.Log("이벤트 UI 표시");
        if (EventManager.Instance != null) EventManager.Instance.ShowPriestEvent();
    }

    public void EndEvent()
    {
        Debug.Log("이벤트 종료 -> 다음 스토리 이동");
        currentState = BattleState.Start;
        eventTriggered = false;
        GoToNextStory();
    }

    private void GoToNextStory()
    {
        Debug.Log($"스테이지 {currentStage} 스토리 씬 로드");
        SceneManager.LoadScene("StoryScene");
    }
}