using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class BattleUI : MonoBehaviour
{
    [Header("Character Display")]
    public Image playerIllustration;
    public Image enemyIllustration;

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
        Debug.Log($"[UI] player.skillList count = {player.skillList.Count}");

        foreach (int skillID in player.skillList)
        {
            Debug.Log($"[UI] 검사 중 skillID = {skillID}");

            if (!DataManager.Instance.skillDict.ContainsKey(skillID))
            {
                Debug.LogWarning($"[UI] skillDict에 없음: {skillID}");
                continue;
            }

            var skillData = DataManager.Instance.skillDict[skillID];
            Debug.Log($"[UI] skill found: {skillData.name}, type:{skillData.skillType}");

            if (!string.IsNullOrEmpty(skillData.skillType) && skillData.skillType.ToLower() == "passive")
            {
                Debug.Log($"[UI] 패시브라서 버튼 생성 안 함: {skillData.name}");
                continue;
            }

            GameObject btnObj = Instantiate(skillButtonPrefab, skillButtonParent);

            TextMeshProUGUI[] texts = btnObj.GetComponentsInChildren<TextMeshProUGUI>();
            if (texts.Length >= 2)
            {
                texts[0].text = skillData.name;
                texts[1].text = skillData.actionCost.ToString(); // 두 번째 텍스트에 비용만 표시
            }
            else if (texts.Length == 1)
            {
                // 아직 프리팹에 두 번째 텍스트를 안 넣었을 경우를 대비한 옛날 방식 호환 코드
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
                    Debug.Log($"[버튼 클릭] skillID:{capturedSkillID}");

                    if (BattleManager.Instance.currentState == BattleState.PlayerTurn)
                    {
                        player.UseSkill(capturedSkillID, BattleManager.Instance.enemy);
                    }
                    else
                    {
                        Debug.LogWarning("[버튼 클릭] 지금은 플레이어 턴이 아님");
                    }
                });
            }
            else
            {
                Debug.LogWarning("[UI] 버튼 프리팹에 Button 컴포넌트 없음");
            }

            Debug.Log($"[UI] 버튼 생성 완료: {skillData.name}");
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
        if (battleLogText != null)
            battleLogText.text = "";
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

        Debug.Log(message);
    }
}