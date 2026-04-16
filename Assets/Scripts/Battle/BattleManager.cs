using UnityEngine;
using UnityEngine.SceneManagement; // 스토리 씬으로 넘어가기 위해 필요함

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

    // 신녀 이벤트 등장 확률 (현재는 지정된 횟수에만 확정 등장하도록 사용 중)
    [Range(0, 100)]
    public int priestEventChance = 40;
    private bool eventTriggered = false;

    // [수정됨] 전투 횟수 카운트를 "현재 스토리 스테이지"와 동기화시킵니다!
    private int currentStage = 1;

    // UI 에러 방지용 (적 다음 공격 미리보기)
    public int plannedEnemyAttackID = -1;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // 씬 전환 시에도 캐릭터가 안 지워지도록 백업 호출
        int playerID = DataManager.SelectedPlayerID;
        if (playerID == 0) playerID = PlayerPrefs.GetInt("SavedPlayerID", 1);
        else { PlayerPrefs.SetInt("SavedPlayerID", playerID); PlayerPrefs.Save(); }

        // 현재 스테이지(전투 횟수)를 불러옵니다.
        currentStage = PlayerPrefs.GetInt("CurrentStage", 1);
        Debug.Log($"[Battle Start] SelectedPlayerID: {playerID} / CurrentStage: {currentStage}");

        // 스테이지 번호를 적 ID로 주면, 판이 넘어갈 때마다 다른 적이 나옵니다.
        SetupBattle(playerID, currentStage);
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

        PlanNextEnemyAttack(); // 적 공격 텍스트 갱신
        StartPlayerTurn();
    }

    public void PlanNextEnemyAttack()
    {
        if (enemy.skillList == null || enemy.skillList.Count == 0) return;
        plannedEnemyAttackID = enemy.skillList[Random.Range(0, enemy.skillList.Count)];
        if (BattleUI.Instance != null) BattleUI.Instance.UpdateEnemyIntention(plannedEnemyAttackID);
    }

    void StartPlayerTurn()
    {
        if (CheckGameOver()) return;

        currentState = BattleState.PlayerTurn;
        player.OnTurnStart();

        if (DataManager.Instance.playerDict.ContainsKey(player.id))
            player.currentAP = DataManager.Instance.playerDict[player.id].actionPoint;

        Debug.Log("플레이어의 턴입니다.");

        if (BattleUI.Instance != null)
        {
            BattleUI.Instance.AddLog("플레이어의 턴입니다.");
            BattleUI.Instance.RefreshSkillButtons();
            if (plannedEnemyAttackID != -1) BattleUI.Instance.UpdateEnemyIntention(plannedEnemyAttackID);
        }
    }

    public void StartEnemyTurn()
    {
        if (currentState != BattleState.PlayerTurn) return;
        if (CheckGameOver()) return;

        currentState = BattleState.EnemyTurn;

        if (BattleUI.Instance != null)
        {
            BattleUI.Instance.AddLog("적의 턴입니다.");
            BattleUI.Instance.HideEnemyIntention();
        }

        if (enemy.skillList == null || enemy.skillList.Count == 0)
        {
            Debug.LogWarning("적이 사용할 공격이 없습니다.");
            Invoke(nameof(StartPlayerTurn), 1.0f);
            return;
        }

        if (plannedEnemyAttackID == -1) PlanNextEnemyAttack();

        Debug.Log($"[Enemy Turn] plannedEnemyAttackID: {plannedEnemyAttackID}");

        // [작성하신 로직 완벽 유지] 랜덤 데미지 추가
        int randomDamage = Random.Range(3, 8);
        player.TakeDamage(randomDamage);
        if (BattleUI.Instance != null)
            BattleUI.Instance.AddLog($"{enemy.charName}가 {randomDamage} 데미지를 입힘");

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

            if (GameOverUI.Instance != null)
                GameOverUI.Instance.ShowGameOver();

            return true;
        }

        return false;
    }

    // 전투 승리 함수
    void OnBattleWin()
    {
        if (eventTriggered) return;
        eventTriggered = true;

        // [핵심 변경 1] 전투 승리 시, 다음 스테이지로 숫자를 1 올립니다.
        currentStage++;
        PlayerPrefs.SetInt("CurrentStage", currentStage);
        PlayerPrefs.Save();

        // (currentStage - 1) 이 곧 이전에 작성하신 battleCount 와 동일한 의미입니다.
        int winCount = currentStage - 1;
        Debug.Log($"현재 전투 승리 횟수: {winCount}");

        // [작성하신 로직 유지] 6번 이상 이기면 게임 클리어!
        if (winCount >= 6)
        {
            Debug.Log("게임 클리어");
            if (GameClearUI.Instance != null) GameClearUI.Instance.ShowGameClear();
            return;
        }

        Debug.Log("승리! 보상을 획득합니다.");

        // [작성하신 로직 유지] 2번째, 4번째 전투 승리 시 신녀 이벤트 확정 등장
        if (winCount == 2 || winCount == 4)
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

    public void EndEvent()
    {
        Debug.Log("이벤트 종료");
        currentState = BattleState.Start;
        eventTriggered = false;

        // [핵심 변경 2] 이벤트가 끝나면 다음 배틀을 바로 부르지 않고, 다음 스토리로 넘깁니다!
        GoToNextStory();
    }

    private void GoToNextStory()
    {
        Debug.Log($"스테이지 {currentStage} 스토리 씬을 불러옵니다.");
        SceneManager.LoadScene("StoryScene");
    }
}