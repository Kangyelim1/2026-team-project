using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleUI : MonoBehaviour
{
    [Header("Character Display")]
    public Image playerIllustration;
    public Image enemyIllustration;

    [Header("Background")]
    public Image battleBackground;

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

        if (playerNameText != null) playerNameText.text = p.charName;
        if (enemyNameText != null) enemyNameText.text = e.charName;

        if (playerHPText != null) playerHPText.text = $"HP: {p.currentHP} / {p.maxHP}";
        if (enemyHPText != null) enemyHPText.text = $"HP: {e.currentHP} / {e.maxHP}";

        // ★[수정된 로직] 콩쥐의 행동력(AP)을 "현재치 / 최대치" 로 명확히 표시합니다!
        if (apText != null)
        {
            apText.text = $"AP: {p.currentAP} / {p.maxAP}";
        }

        if (endTurnButton != null)
            endTurnButton.interactable = (BattleManager.Instance.currentState == BattleState.PlayerTurn);

        // 스킬 버튼 갱신
        foreach (var kvp in skillButtons)
        {
            int sID = kvp.Key;
            Button b = kvp.Value;
            if (b != null && DataManager.Instance.skillDict.ContainsKey(sID))
            {
                var sData = DataManager.Instance.skillDict[sID];
                bool isPlayable = (BattleManager.Instance.currentState == BattleState.PlayerTurn);

                if (p.currentAP < sData.actionCost) isPlayable = false;
                if (!sData.usingAgain && p.usedSkillsThisTurn.Contains(sID)) isPlayable = false;

                b.interactable = isPlayable;
            }
        }

        // ★[추가된 로직] AP가 0이 되면 턴이 자동으로 종료됩니다.
        if (p.currentAP <= 0 && BattleManager.Instance.currentState == BattleState.PlayerTurn)
        {
            Debug.Log("AP가 모두 소진되어 자동으로 턴을 종료합니다.");
            BattleManager.Instance.StartEnemyTurn();
        }
    }

    void CreateSkillButtons()
    {
        if (skillButtonParent == null || skillButtonPrefab == null ||
            BattleManager.Instance == null || BattleManager.Instance.player == null)
        {
            Debug.LogWarning("UI 세팅 누락됨");
            return;
        }

        foreach (Transform child in skillButtonParent)
        {
            Destroy(child.gameObject);
        }
        skillButtons.Clear();

        var player = BattleManager.Instance.player;

        foreach (int skillID in player.skillList)
        {
            if (!DataManager.Instance.skillDict.ContainsKey(skillID)) continue;

            var skillData = DataManager.Instance.skillDict[skillID];
            if (!skillData.activate) continue;

            if (!string.IsNullOrEmpty(skillData.skillType) && skillData.skillType.ToLower() == "passive") continue;

            GameObject btnObj = Instantiate(skillButtonPrefab, skillButtonParent);

            Image iconImage = btnObj.transform.Find("Icon").GetComponent<Image>();
            if (iconImage != null && !string.IsNullOrEmpty(skillData.iconPath))
            {
                Sprite icon = Resources.Load<Sprite>(skillData.iconPath);

                if (icon != null)
                {
                    iconImage.sprite = icon;
                }
                else
                {
                    Debug.LogWarning($"아이콘 로드 실패: {skillData.iconPath}");
                }
            }

            TextMeshProUGUI[] texts = btnObj.GetComponentsInChildren<TextMeshProUGUI>();

            if (texts.Length >= 2)
            {
                texts[0].text = skillData.name;
                texts[1].text = skillData.actionCost.ToString();
            }
            else if (texts.Length == 1)
            {
                texts[0].text = $"{skillData.name} (Cost: {skillData.actionCost})";
            }

            Button btn = btnObj.GetComponent<Button>();
            if (btn != null)
            {
                skillButtons[skillID] = btn;
                int capturedSkillID = skillID;

                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => {
                    if (BattleManager.Instance.currentState == BattleState.PlayerTurn)
                    {
                        player.UseSkill(capturedSkillID, BattleManager.Instance.enemy);
                    }
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
            if (string.IsNullOrEmpty(battleLogText.text))
                battleLogText.text = message;
            else
                battleLogText.text += "\n" + message;
        }
        Debug.Log($"[BattleLog] {message}");
    }

    public void UpdateEnemyIntention(int attackID)
    {
        if (enemyIntentionText == null) return;

        if (DataManager.Instance != null && DataManager.Instance.enemyAttackDict.ContainsKey(attackID))
        {
            var attackData = DataManager.Instance.enemyAttackDict[attackID];
            int expectedDamage = 0;
            if (attackData.attackEffectValues != null && attackData.attackEffectValues.Count > 0)
            {
                expectedDamage = (int)attackData.attackEffectValues[0];
            }

            enemyIntentionText.text = $"{attackData.name} ({expectedDamage} 데미지 예정)";
            enemyIntentionText.gameObject.SetActive(true);
        }
        else
        {
            enemyIntentionText.text = "의도 알 수 없음";
            enemyIntentionText.gameObject.SetActive(true);
        }
    }

    public void HideEnemyIntention()
    {
        if (enemyIntentionText != null)
        {
            enemyIntentionText.gameObject.SetActive(false);
        }
    }
}