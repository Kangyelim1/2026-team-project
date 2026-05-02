using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

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
        // 1. 선택한 캐릭터 ID 가져오기 (1: 콩쥐, 2: 해님달님)
        int playerID = DataManager.SelectedPlayerID;
        if (playerID == 0) playerID = PlayerPrefs.GetInt("SavedPlayerID", 1);
        else
        {
            PlayerPrefs.SetInt("SavedPlayerID", playerID);
            PlayerPrefs.Save();
        }

        // 2. 현재 스테이지 가져오기
        currentStage = PlayerPrefs.GetInt("CurrentStage", 1);

        if (playerID == 2 && currentStage < 11)
        {
            currentStage = 11;
            PlayerPrefs.SetInt("CurrentStage", currentStage);
            PlayerPrefs.Save();
        }

        // ★ [수정됨] 3. 엔딩 스토리(스테이지 6 또는 16)를 보고 넘어왔거나, 엔딩 씬 플래그가 켜져있다면 게임 클리어!
        if (currentStage == 6 || currentStage == 16 || PlayerPrefs.GetInt("IsEndingScene", 0) == 1)
        {
            PlayerPrefs.SetInt("IsEndingScene", 0);
            PlayerPrefs.Save();

            Debug.Log("마지막 스토리 완료 확인! ➡️ 전투 없이 바로 엔딩 클리어 창 팝업!");

            if (BattleUI.Instance != null && BattleUI.Instance.battleBackground != null)
            {
                Sprite bgSprite = Resources.Load<Sprite>("Backgrounds/Bg_NightBridge");
                if (bgSprite != null) BattleUI.Instance.battleBackground.sprite = bgSprite;
            }

            if (BattleUI.Instance != null)
            {
                if (BattleUI.Instance.playerIllustration != null) BattleUI.Instance.playerIllustration.gameObject.SetActive(false);
                if (BattleUI.Instance.enemyIllustration != null) BattleUI.Instance.enemyIllustration.gameObject.SetActive(false);
            }

            currentState = BattleState.Win;

            // 게임 클리어 창 팝업!
            if (GameClearUI.Instance != null) GameClearUI.Instance.ShowGameClear();
            return;
        }

        // 4. 사제 이벤트 (포켓몬 센터 같은 회복 이벤트)
        if (PlayerPrefs.GetInt("NeedEvent", 0) == 1)
        {
            currentState = BattleState.Event;
            if (EventManager.Instance != null) EventManager.Instance.ShowPriestEvent();
            return;
        }

        // 5. 현재 스테이지에 맞는 적 ID 설정
        currentEnemyID = 1;

        // --- 콩쥐 스테이지 (1 ~ 5) ---
        if (currentStage == 1) currentEnemyID = 1;
        else if (currentStage == 2) currentEnemyID = 3;
        else if (currentStage == 3) currentEnemyID = 5;
        else if (currentStage == 4) currentEnemyID = 6;
        else if (currentStage == 5) currentEnemyID = 7; // 최종보스: 내면의 콩쥐

        // --- 해님달님 스테이지 (11 ~ 15) ---
        else if (currentStage == 11) currentEnemyID = 11; // 아기 호랑이
        else if (currentStage == 12) currentEnemyID = 12; // 부모 호랑이
        else if (currentStage == 13) currentEnemyID = 13; // 유령 호랑이
        else if (currentStage == 14) currentEnemyID = 14; // 호랑이 무리
        else if (currentStage == 15) currentEnemyID = 15; // 최종보스: 산의 주인

        SetupBattle(playerID, currentEnemyID);
    }

    public void SetupBattle(int playerID, int enemyID)
    {
        currentState = BattleState.Start;
        eventTriggered = false;
        currentEnemyID = enemyID;

        player.InitPlayer(playerID);
        enemy.InitEnemy(enemyID);

        if (BattleUI.Instance != null)
        {
            // 배경 이미지 설정
            if (BattleUI.Instance.battleBackground != null)
            {
                string bgName = "Bg_Yard";
                if (enemyID == 1) bgName = "Bg_Yard";
                else if (enemyID == 2 || enemyID == 3) bgName = "Bg_Morning";
                else if (enemyID == 4 || enemyID == 11 || enemyID == 12) bgName = "Bg_Morning";
                else if (enemyID == 13) bgName = "Bg_Night";
                else if (enemyID == 5 || enemyID == 6 || enemyID == 14) bgName = "Bg_Village";
                else if (enemyID == 7 || enemyID == 15) bgName = "Bg_NightBridge"; // 최종보스들

                Sprite bgSprite = Resources.Load<Sprite>($"Backgrounds/{bgName}");
                if (bgSprite != null) BattleUI.Instance.battleBackground.sprite = bgSprite;
            }

            // 플레이어 이미지 설정 (콩쥐 vs 해님달님)
            if (BattleUI.Instance.playerIllustration != null)
            {
                string playerImgName = (playerID == 2) ? "Haenim_Normal" : "Kongjwi_Sword";
                Sprite playerSprite = Resources.Load<Sprite>($"Portraits/{playerImgName}");
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
                // 콩쥐 적
                if (enemyID == 1) enemyImgName = "Monster_Rice";
                else if (enemyID == 2) enemyImgName = "Monster_Loom";
                else if (enemyID == 3) enemyImgName = "Patjwi_Normal";
                else if (enemyID == 4) enemyImgName = "Monster_Pot";
                else if (enemyID == 5) enemyImgName = "StepMother_Angry";
                else if (enemyID == 6) enemyImgName = "Magistrate_Normal";
                else if (enemyID == 7) enemyImgName = "FutureKongjwi_Normal";
                // 해님달님 적
                else if (enemyID == 11) enemyImgName = "BabyTiger_Normal";
                else if (enemyID == 12) enemyImgName = "ParentTiger_Angry";
                else if (enemyID == 13) enemyImgName = "GhostTiger_Normal";
                else if (enemyID == 14) enemyImgName = "TigerPack_Normal";
                else if (enemyID == 15) enemyImgName = "BossTiger_Normal";

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

        // 턴 시작 시 AP 회복 로직 (데이터 매니저 연동용)
        if (DataManager.Instance != null && DataManager.Instance.playerDict != null)
        {
            if (DataManager.Instance.playerDict.ContainsKey(player.id))
                player.currentAP = DataManager.Instance.playerDict[player.id].actionPoint;
        }

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

        // 적 스킬 사용
        enemy.UseEnemySkill(plannedEnemyAttackID, player);
        if (BattleUI.Instance != null) BattleUI.Instance.AddLog($"{enemy.charName}의 공격!");

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

        // 콩쥐 중간 보스 이벤트 분기
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

        // ★ [수정됨] 최종 보스 처치 시 바로 클리어 창을 띄우지 않고, 엔딩 스토리 씬으로 넘김!
        if (currentEnemyID == 7 || currentEnemyID == 15)
        {
            Debug.Log("마지막 보스 처치 완료! 엔딩 스토리로 넘어갑니다.");
            Invoke(nameof(GoToNextStoryAfterBoss), 2.0f); // 2초 뒤 스토리 씬으로 이동
            return;
        }

        // 일반 승리 시 이벤트 센터로 갈 확률
        if (EventManager.Instance != null && Random.Range(0, 100) < priestEventChance)
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

    private void StartBattle_Loom() { SetupBattle(player.id, 2); }
    private void StartBattle_Pot() { SetupBattle(player.id, 4); }

    public void GoToNextStoryAfterBoss()
    {
        currentStage++;
        PlayerPrefs.SetInt("CurrentStage", currentStage);
        PlayerPrefs.SetInt("NeedEvent", 0);
        PlayerPrefs.Save();

        SceneManager.LoadScene("StoryScene");
    }
}