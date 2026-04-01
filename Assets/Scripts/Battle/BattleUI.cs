using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro 사용 시

public class BattleUI : MonoBehaviour 
{
    [Header("Character Display")]  // 일러스트
    public Image playerIllustration; // 좌측 플레이어 일러스트
    public Image enemyIllustration;  // 우측 적 일러스트
    
    [Header("Stats")]  // 체력 and 행동력
    public TextMeshProUGUI playerHPText; // 플레이어 체력 (예: 100 / 100)
    public TextMeshProUGUI enemyHPText;  // 적 체력
    public TextMeshProUGUI apText;   // 왼쪽 하단 행동력 표시

    [Header("Skills & Buttons")]  // 스킬 버튼 같은거
    public Transform skillButtonParent; // 하단 스킬 버튼들이 생성될 곳
    public GameObject skillButtonPrefab;
    public Button endTurnButton;       // 우측 턴 넘기기 버튼

    void Start() {
        // 턴 넘기기 버튼에 기능 연결
        endTurnButton.onClick.AddListener(() => BattleManager.Instance.StartEnemyTurn()); 
        CreateSkillButtons();
    }

    void Update() {
        UpdateBattleUI();
    }

    // 1. 실시간 수치 업데이트 (HP, AP)
    void UpdateBattleUI() {
        var p = BattleManager.Instance.player;
        var e = BattleManager.Instance.enemy;

        playerHPText.text = $"HP: {p.currentHP} / {p.maxHP}";
        enemyHPText.text = $"HP: {e.currentHP} / {e.maxHP}";
        apText.text = $"AP: {p.currentAP}"; // 행동력 표시
        
        // 플레이어 턴일 때만 버튼 활성화
        endTurnButton.interactable = (BattleManager.Instance.currentState == BattleState.PlayerTurn);
    }

    // 2. 엑셀 데이터를 기반으로 스킬 버튼 생성
    void CreateSkillButtons() {
        // 기존 버튼 제거
        foreach (Transform child in skillButtonParent) Destroy(child.gameObject);

        var player = BattleManager.Instance.player;
        foreach (int skillID in player.skillList) {
            if (!DataManager.Instance.skillDict.ContainsKey(skillID)) continue;
            var skillData = DataManager.Instance.skillDict[skillID];

            GameObject btnObj = Instantiate(skillButtonPrefab, skillButtonParent);
            btnObj.GetComponentInChildren<TextMeshProUGUI>().text = $"{skillData.name}\n(Cost: {skillData.cost})";
            
            // 버튼 클릭 시 스킬 발동
            btnObj.GetComponent<Button>().onClick.AddListener(() => {
                if (BattleManager.Instance.currentState == BattleState.PlayerTurn) {
                    player.UseSkill(skillID, BattleManager.Instance.enemy);
                }
            });
        }
    }
}