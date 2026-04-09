using UnityEngine;

public enum BattleState
{
    Start,
    PlayerTurn,
    EnemyTurn,
    Event,
    Win,
    Lose
}



public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;
    public BattleState currentState;

    public Character player;
    public Character enemy;

    // 신녀 이벤트 등장 확률
    [Range(0, 100)]
    public int priestEventChance = 40; // 기본 40% 확률
    // 이벤트 중복 방지용
    private bool eventTriggered = false;

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

        player.OnTurnStart(); // 턴 시작시 스킬 사용기록 초기화

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
        Debug.Log("이벤트 종료");

        // 이벤트 상태 종료
        currentState = BattleState.Start;

        // 이벤트 중복 방지 초기화
        eventTriggered = false;
    }

    bool CheckGameOver()
    {
        // 이벤트 중이면 체크 안함
        if (currentState == BattleState.Event)
            return true;

        if (enemy.currentHP <= 0 && currentState != BattleState.Win)
        {
            currentState = BattleState.Win;
            OnBattleWin();
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

    //전투 승리 함수
    void OnBattleWin()
    {
        // 이미 이벤트 실행되었으면 실행하지 않음
        if (eventTriggered) return;

        eventTriggered = true;

        Debug.Log("승리! 보상을 획득합니다.");

        // BattleUI 로그 제거 (데미지 로그 섞임 방지)
        // BattleUI.Instance.AddLog("승리! 보상을 획득합니다.");

        int random = Random.Range(0, 100);
        Debug.Log($"신녀 이벤트 확률 체크: {random}");

        if (random < priestEventChance)
        {
            Debug.Log("신녀 이벤트 등장!");
            StartEvent();
        }
        else
        {
            Debug.Log("이벤트 없음");
            EndEvent();
        }
    }

    void StartEvent()
    {
        currentState = BattleState.Event;

        Debug.Log("신녀 이벤트 시작");

        if (EventManager.Instance != null)
            EventManager.Instance.ShowPriestEvent();
    }


}