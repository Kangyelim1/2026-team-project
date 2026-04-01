using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour 
{
    [Header("Character Info")]
    public int id;
    public string charName;
    public float currentHP;
    public float maxHP;
    public int currentAP; // 플로우 차트의 '행동력'
    public List<int> skillList = new List<int>();

    // 데이터 기반 초기화 (플레이어용)
    public void InitPlayer(int playerID) 
    {
        if (!DataManager.Instance.playerDict.ContainsKey(playerID)) return;
        var data = DataManager.Instance.playerDict[playerID];
        
        charName = data.name;
        maxHP = data.hp;
        currentHP = maxHP;
        currentAP = data.action;
        skillList = new List<int>(data.skills);

        // [추가] 스킬이 몇 개나 들어왔는지 콘솔에 찍어봅니다.
        Debug.Log($"{charName}의 스킬 개수: {skillList.Count}개 로드됨");
    }

    // 데이터 기반 초기화 (적용)
    public void InitEnemy(int enemyID) 
    {
        if (!DataManager.Instance.enemyDict.ContainsKey(enemyID)) return;
        var data = DataManager.Instance.enemyDict[enemyID];
        
        charName = data.name;
        maxHP = data.hp;
        currentHP = maxHP;
        skillList = new List<int>(data.attacks);
    }

    public void UseSkill(int skillID, Character target) 
    {
        if (!DataManager.Instance.skillDict.ContainsKey(skillID)) return;
        var skill = DataManager.Instance.skillDict[skillID];

        // 1. 행동력(AP) 체크 및 소모
        if (currentAP < skill.cost) {
            Debug.Log("행동력 부족!");
            return;
        }
        currentAP -= skill.cost;

        // 2. 스킬 효과 실행 (ExecuteAbility 로직은 기존과 동일)
        foreach (int abilityID in skill.skillAbilities) {
            ExecuteAbility(abilityID, skill.effect, target);
        }
    }

    void ExecuteAbility(int id, float power, Character target) {
        if (!DataManager.Instance.abilityDict.ContainsKey(id)) return;
        var ability = DataManager.Instance.abilityDict[id];

        switch (ability.effect) {
            case EffectType.Attack:
                if (target != null) target.TakeDamage(power);
                break;
            case EffectType.Heal:
                currentHP = Mathf.Min(maxHP, currentHP + power);
                break;
        }
    }

    public void TakeDamage(float amount) {
        currentHP -= amount;
        if (currentHP <= 0) currentHP = 0;
        Debug.Log($"{charName}이(가) {amount} 대미지를 입음. 남은 HP: {currentHP}");
    }
}