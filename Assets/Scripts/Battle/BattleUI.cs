using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class BattleUI : MonoBehaviour
{
    [Header("Character Display")]
    public Image playerIllustration;
    public Image enemyIllustration;

    [Header("Background")] 
    public Image battleBackground; // 전투 배경 이미지

    [Header("Names")]
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI enemyNameText;

    [Header("Stats")]
    public TextMeshProUGUI playerHPText;
    public TextMeshProUGUI enemyHPText;
    public TextMeshProUGUI apText;

    [Header("Skills & Buttons")]
    public Transform skillButtonParent;
    public GameObject skillButtonPrefab;
    public Button endTurnButton;

    [Header("Battle Log")]
    public TextMeshProUGUI battleLogText;

    //  [여기가 핵심!] 적의 다음 행동을 화면에 띄워줄 텍스트 변수
    [Header("Enemy Intention UI")]
    public TextMeshProUGUI enemyIntentionText;

    private Dictionary<int, Button> skillButtons = new Dictionary<int, Button>();

    public static BattleUI Instance;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (endTurnButton != null)
        {
            endTurnButton.onClick.RemoveAllListeners();
            endTurnButton.onClick.AddListener(() => BattleManager.Instance.StartEnemyTurn());
        }

        Invoke(nameof(DelayedCreateButtons), 0.1f);
    }

    void Update()
    {
        UpdateBattleUI();
    }

    void UpdateBattleUI()
    {
        if (BattleManager.Instance == null || BattleManager.Instance.player == null || BattleManager.Instance.enemy == null)
            return;

        var p = BattleManager.Instance.player;
        var e = BattleManager.Instance.enemy;

        if (playerNameText != null)
            playerNameText.text = p.charName;

        if (enemyNameText != null)
            enemyNameText.text = e.charName;

        if (playerHPText != null)
            playerHPText.text = $"HP: {p.currentHP} / {p.maxHP}";

        if (enemyHPText != null)
            enemyHPText.text = $"HP: {e.currentHP} / {e.maxHP}";

        if (apText != null)
            apText.text = $"AP: {p.currentAP}";

        if (endTurnButton != null)
            endTurnButton.interactable = (BattleManager.Instance.currentState == BattleState.PlayerTurn);

        // 스킬 버튼들 사용 가능 여부 실시간 업데이트
        foreach (var kvp in skillButtons)
        {
            int sID = kvp.Key;
            Button b = kvp.Value;
            if (b != null && DataManager.Instance.skillDict.ContainsKey(sID))
            {
                var sData = DataManager.Instance.skillDict[sID];
                bool isPlayable = (BattleManager.Instance.currentState == BattleState.PlayerTurn);

                // AP 부족시 비활성화
                if (p.currentAP < sData.actionCost) isPlayable = false;

                // UsingAgain이 false이고 이번 턴에 이미 썼다면 비활성화
                if (!sData.usingAgain && p.usedSkillsThisTurn.Contains(sID)) isPlayable = false;

                b.interactable = isPlayable;
            }
        }

        if (p.currentAP <= 0 && BattleManager.Instance.currentState == BattleState.PlayerTurn)
        {
            Debug.Log("행동력 소진! 자동으로 적 턴으로 전환합니다.");
            BattleManager.Instance.StartEnemyTurn();
        }
    }

    void CreateSkillButtons()
    {
        if (skillButtonParent == null || skillButtonPrefab == null || BattleManager.Instance == null || BattleManager.Instance.player == null)
        {
            Debug.LogWarning("[UI] 버튼 생성 실패: 참조 누락");
            return;
        }

        foreach (Transform child in skillButtonParent)
            Destroy(child.gameObject);

        skillButtons.Clear();

        var player = BattleManager.Instance.player;

        foreach (int skillID in player.skillList)
        {
            if (!DataManager.Instance.skillDict.ContainsKey(skillID)) continue;

            var skillData = DataManager.Instance.skillDict[skillID];

            if (!skillData.activate) continue;

            if (!string.IsNullOrEmpty(skillData.skillType) && skillData.skillType.ToLower() == "passive") continue;

            GameObject btnObj = Instantiate(skillButtonPrefab, skillButtonParent);
            TextMeshProUGUI[] texts = btnObj.GetComponentsInChildren<TextMeshProUGUI>();
            if (texts.Length >= 2)
            {
                texts[0].text = skillData.name;
                texts[1].text = skillData.actionCost.ToString();
            }
            else if (texts.Length == 1)
            {
                texts[0].text = $"{skillData.name}\n(Cost: {skillData.actionCost})";
            }

            Button btn = btnObj.GetComponent<Button>();
            if (btn != null)
            {
                skillButtons[skillID] = btn;
                int capturedSkillID = skillID;
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    if (BattleManager.Instance.currentState == BattleState.PlayerTurn)
                        player.UseSkill(capturedSkillID, BattleManager.Instance.enemy);
                });
            }
        }
    }

    void DelayedCreateButtons()
    {
        if (BattleManager.Instance != null && BattleManager.Instance.player != null)
        {
            CreateSkillButtons();
        }
    }

    public void RefreshSkillButtons()
    {
        CreateSkillButtons();
    }

    public void ClearLog()
    {
        if (battleLogText != null) battleLogText.text = "";
    }

    public void AddLog(string message)
    {
        if (battleLogText != null)
        {
            if (string.IsNullOrEmpty(battleLogText.text)) battleLogText.text = message;
            else battleLogText.text += "\n" + message;
        }
        Debug.Log(message);
    }

    // ==========================================================
    //  에러의 원인! 이 두 함수가 추가되었습니다. 
    // ==========================================================

    // 적이 다음 턴에 쓸 공격을 미리 화면(EnemyIntentionText)에 띄워줍니다.
    public void UpdateEnemyIntention(int attackID)
    {
        if (enemyIntentionText == null) return;

        if (DataManager.Instance != null && DataManager.Instance.enemyAttackDict.ContainsKey(attackID))
        {
            var attackData = DataManager.Instance.enemyAttackDict[attackID];

            int expectedDamage = 0;
            if (attackData.attackEffectValues != null && attackData.attackEffectValues.Count > 0)
            {
                expectedDamage = attackData.attackEffectValues[0];
            }

            enemyIntentionText.text = $"예정 공격: {attackData.name}\n(데미지: {expectedDamage})";
            enemyIntentionText.gameObject.SetActive(true);
        }
        else
        {
            enemyIntentionText.text = "알 수 없는 공격";
            enemyIntentionText.gameObject.SetActive(true);
        }
    }

    // 적이 자기 턴에 공격을 실행할 때는 예고 텍스트를 잠시 숨깁니다.
    public void HideEnemyIntention()
    {
        if (enemyIntentionText != null)
        {
            enemyIntentionText.gameObject.SetActive(false);
        }
    }
}