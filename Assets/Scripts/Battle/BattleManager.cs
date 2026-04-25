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

    private int currentEnemyID = 1;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        int playerID = DataManager.SelectedPlayerID;
        if (playerID == 0) playerID = PlayerPrefs.GetInt("SavedPlayerID", 1);
        else
        {
            PlayerPrefs.SetInt("SavedPlayerID", playerID);
            PlayerPrefs.Save();
        }

        currentStage = PlayerPrefs.GetInt("CurrentStage", 1);

        if (PlayerPrefs.GetInt("NeedEvent", 0) == 1)
        {
            currentState = BattleState.Event;
            if (EventManager.Instance != null) EventManager.Instance.ShowPriestEvent();
            return;
        }

        currentEnemyID = 1;
        if (currentStage == 1) currentEnemyID = 1;
        else if (currentStage == 2) currentEnemyID = 3;
        else if (currentStage == 3) currentEnemyID = 5;
        else if (currentStage == 4) currentEnemyID = 6;
        else if (currentStage == 5) currentEnemyID = 7; // ★ 5스테이지: 최종보스 '흑화 콩쥐' (ID: 7)

        SetupBattle(playerID, currentEnemyID);
    }

    public void SetupBattle(int playerID, int enemyID)
    {
        currentState = BattleState.Start;
        eventTriggered = false;
        currentEnemyID = enemyID;

        player.InitPlayer(playerID);
        enemy.InitEnemy(enemyID);

        // 마지막 5스테이지 콩쥐는 이름을 '내면의 콩쥐' 등으로 강제 변경
        if (enemyID == 7) enemy.charName = "내면의 콩쥐";

        if (BattleUI.Instance != null)
        {
            // 배경 설정
            if (BattleUI.Instance.battleBackground != null)
            {
                string bgName = "Bg_Yard";
                if (enemyID == 1) bgName = "Bg_Yard";
                else if (enemyID == 2 || enemyID == 3) bgName = "Bg_Morning";
                else if (enemyID == 4) bgName = "Bg_Night";
                else if (enemyID == 5 || enemyID == 6) bgName = "Bg_Village";
                else if (enemyID == 7) bgName = "Bg_NightBridge"; // 찐막 전투는 밤 다리 위에서!

                Sprite bgSprite = Resources.Load<Sprite>($"Backgrounds/{bgName}");
                if (bgSprite != null) BattleUI.Instance.battleBackground.sprite = bgSprite;
            }

            // 플레이어 이미지
            if (BattleUI.Instance.playerIllustration != null)
            {
                Sprite playerSprite = Resources.Load<Sprite>("Portraits/Kongjwi_Sword");
                if (playerSprite != null)
                {
                    BattleUI.Instance.playerIllustration.sprite = playerSprite;
                    BattleUI.Instance.playerIllustration.gameObject.SetActive(true);
                }
            }

            // 적 이미지 설정
            if (BattleUI.Instance.enemyIllustration != null)
            {
                string enemyImgName = "";
                if (enemyID == 1) enemyImgName = "Monster_Rice";
                else if (enemyID == 2) enemyImgName = "Monster_Loom";
                else if (enemyID == 3) enemyImgName = "Patjwi_Normal";
                else if (enemyID == 4) enemyImgName = "Monster_Pot";
                else if (enemyID == 5) enemyImgName = "StepMother_Angry";
                else if (enemyID == 6) enemyImgName = "Magistrate_Normal";
                else if (enemyID == 7) enemyImgName = "FutureKongjwi_Normal"; // ★ 마지막 보스 사진: 흑화 콩쥐!

                Sprite enemySprite = Resources.Load<Sprite>($"Portraits/{enemyImgName}");
                if (enemySprite != null)
                {
                    BattleUI.Instance.enemyIllustration.sprite = enemySprite;
                    BattleUI.Instance.enemyIllustration.gameObject.SetActive(true);
                }
            }

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
        if (BattleUI.Instance != null) BattleUI.Instance.UpdateEnemyIntention(plannedEnemyAttackID);
    }

    void StartPlayerTurn()
    {
        if (CheckGameOver()) return;
        currentState = BattleState.PlayerTurn;
        player.OnTurnStart();

        if (DataManager.Instance.playerDict.ContainsKey(player.id))
            player.currentAP = DataManager.Instance.playerDict[player.id].actionPoint;

        if (BattleUI.Instance != null)
        {
            BattleUI.Instance.AddLog("내 턴이 돌아왔다.");
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
        BattleUI.Instance.ShowDamage(true, randomDamage);

        if (BattleUI.Instance != null) BattleUI.Instance.AddLog($"{enemy.charName}의 공격! {randomDamage} 피해!");
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
            if (BattleUI.Instance != null) BattleUI.Instance.AddLog("눈앞이 깜깜해졌다...");
            if (GameOverUI.Instance != null) GameOverUI.Instance.ShowGameOver();
            return true;
        }
        return false;
    }

    void OnBattleWin()
    {
        if (eventTriggered) return;
        eventTriggered = true;

        if (currentEnemyID == 1)
        {
            Invoke(nameof(StartBattle_Loom), 2.0f);
            return;
        }
        else if (currentEnemyID == 3)
        {
            Invoke(nameof(StartBattle_Pot), 2.0f);
            return;
        }

        // ★ 수정: 마지막 보스(ID 7 콩쥐)를 잡았으면 이벤트 띄우지 않고 바로 게임 클리어 창 호출!
        if (currentEnemyID == 7)
        {
            Debug.Log("마지막 보스 처치 완료! 게임 클리어 창 팝업!");
            if (GameClearUI.Instance != null) GameClearUI.Instance.ShowGameClear();
            return;
        }

        if (EventManager.Instance != null)
        {
            currentState = BattleState.Event;
            EventManager.Instance.ShowPriestEvent();
            return;
        }
        else
        {
            GoToNextStoryAfterBoss();
        }
    }

    private void StartBattle_Loom() { SetupBattle(DataManager.SelectedPlayerID, 2); }
    private void StartBattle_Pot() { SetupBattle(DataManager.SelectedPlayerID, 4); }

    public void GoToNextStoryAfterBoss()
    {
        currentStage++;
        PlayerPrefs.SetInt("CurrentStage", currentStage);
        PlayerPrefs.SetInt("NeedEvent", 0);
        PlayerPrefs.Save();

        SceneManager.LoadScene("StoryScene");
    }
}