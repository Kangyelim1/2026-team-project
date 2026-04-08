using UnityEngine;

public enum BattleState
{
    Start,
    PlayerTurn,
    EnemyTurn,
    Win,
    Lose
}

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;
    public BattleState currentState;

    public Character player;
    public Character enemy;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        int playerID = DataManager.SelectedPlayerID != 0 ? DataManager.SelectedPlayerID : 1;
        Debug.Log($"[Battle Start] SelectedPlayerID: {playerID}");
        SetupBattle(playerID, 1);
    }

    public void SetupBattle(int playerID, int enemyID)
    {
        currentState = BattleState.Start;

        player.InitPlayer(playerID);
        enemy.InitEnemy(enemyID);

        Debug.Log($"{enemy.charName}와 조우했습니다!");

        if (BattleUI.Instance != null)
        {
            BattleUI.Instance.ClearLog();
            BattleUI.Instance.AddLog($"{enemy.charName}와 조우했습니다!");
            BattleUI.Instance.RefreshSkillButtons();
        }

        StartPlayerTurn();
    }

    void StartPlayerTurn()
    {
        if (CheckGameOver()) return;

        currentState = BattleState.PlayerTurn;

        if (DataManager.Instance.playerDict.ContainsKey(player.id))
            player.currentAP = DataManager.Instance.playerDict[player.id].actionPoint;

        Debug.Log("플레이어의 턴입니다.");

        if (BattleUI.Instance != null)
        {
            BattleUI.Instance.AddLog("플레이어의 턴입니다.");
            BattleUI.Instance.RefreshSkillButtons();
        }
    }

    public void StartEnemyTurn()
    {
        if (currentState != BattleState.PlayerTurn) return;
        if (CheckGameOver()) return;

        currentState = BattleState.EnemyTurn;

        if (BattleUI.Instance != null)
            BattleUI.Instance.AddLog("적의 턴입니다.");

        if (enemy.skillList == null || enemy.skillList.Count == 0)
        {
            Debug.LogWarning("적이 사용할 공격이 없습니다.");
            Invoke(nameof(StartPlayerTurn), 1.0f);
            return;
        }

        int randomSkillID = enemy.skillList[Random.Range(0, enemy.skillList.Count)];
        Debug.Log($"[Enemy Turn] randomSkillID: {randomSkillID}");

        enemy.UseEnemySkill(randomSkillID, player);

        if (CheckGameOver())
        {
            CancelInvoke(nameof(StartPlayerTurn));
            return;
        }

        Invoke(nameof(StartPlayerTurn), 1.0f);
    }

    public void EndEvent()
    {
        Debug.Log("이벤트 종료, 전투 진행 재개");
        currentState = BattleState.PlayerTurn;

        if (BattleUI.Instance != null)
        {
            BattleUI.Instance.AddLog("이벤트가 종료되었습니다.");
            BattleUI.Instance.RefreshSkillButtons();
        }
    }

    bool CheckGameOver()
    {
        if (enemy.currentHP <= 0)
        {
            currentState = BattleState.Win;
            Debug.Log("승리! 보상을 획득합니다.");

            if (BattleUI.Instance != null)
                BattleUI.Instance.AddLog("승리! 보상을 획득합니다.");

            return true;
        }

        if (player.currentHP <= 0)
        {
            currentState = BattleState.Lose;
            Debug.Log("패배... 게임 오버.");

            if (BattleUI.Instance != null)
                BattleUI.Instance.AddLog("패배... 게임 오버.");

            return true;
        }

        return false;
    }
}