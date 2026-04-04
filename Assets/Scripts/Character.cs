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
        if (BattleUI.Instance != null) BattleUI.Instance.ClearLog();

        if (!DataManager.Instance.skillDict.ContainsKey(skillID)) return;
        var skill = DataManager.Instance.skillDict[skillID];

        // 1. 행동력(AP) 체크 및 소모
        if (currentAP < skill.cost) {
            if (BattleUI.Instance != null) BattleUI.Instance.AddLog("행동력 부족!");
            return;
        }
        currentAP -= skill.cost;
        if (BattleUI.Instance != null) BattleUI.Instance.AddLog($"[{charName}]이(가) <{skill.name}>을(를) 사용했습니다!");

        // 2. 스킬 효과 실행 (모든 어빌리티에 대해 순차적으로 해당하는 이펙트 값을 전달)
        for (int i = 0; i < skill.skillAbilities.Count; i++) {
            int abilityID = skill.skillAbilities[i];
            float currentEffect = (i < skill.effects.Count) ? skill.effects[i] : 0f;
            ExecuteAbility(abilityID, currentEffect, target);
        }
    }

    public void UseEnemySkill(int attackID, Character target) 
    {
        if (BattleUI.Instance != null) BattleUI.Instance.ClearLog();

        if (!DataManager.Instance.enemyAttackDict.ContainsKey(attackID)) return;
        var attack = DataManager.Instance.enemyAttackDict[attackID];

        if (BattleUI.Instance != null) BattleUI.Instance.AddLog($"[{charName}]이(가) <{attack.name}>을(를) 사용했습니다!");

        // 적군의 경우 별도의 코스트나 AP 소모 로직은 생략
        for (int i = 0; i < attack.skillAbilities.Count; i++) {
            int abilityID = attack.skillAbilities[i];
            float currentEffect = (i < attack.attackEffects.Count) ? (float)attack.attackEffects[i] : 0f;
            ExecuteAbility(abilityID, currentEffect, target);
        }
    }

    void ExecuteAbility(int abilityID, float skillEffect, Character target) {
        if (!DataManager.Instance.abilityDict.ContainsKey(abilityID)) return;
        var ability = DataManager.Instance.abilityDict[abilityID];

        switch (ability.effect) {
        case EffectType.Normal_Attack: //
            target.TakeDamage(skillEffect);
            break;

        case EffectType.Heal:
            currentHP += skillEffect;
            if (currentHP > maxHP) currentHP = maxHP;
            if (BattleUI.Instance != null) BattleUI.Instance.AddLog($"[{charName}]이(가) 체력을 {skillEffect} 회복하여 {currentHP}이 되었습니다.");
            break;
        }
    }

    public void TakeDamage(float amount) {
        currentHP -= amount;
        if (currentHP <= 0) currentHP = 0;
        if (BattleUI.Instance != null) BattleUI.Instance.AddLog($"[{charName}]이(가) {amount} 대미지를 입음! (남은 체력: {currentHP})");
    }
}