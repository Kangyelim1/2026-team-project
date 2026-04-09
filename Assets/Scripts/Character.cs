using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("Character Info")]
    public int id;
    public string charName;
    public float currentHP;
    public float maxHP;
    public int currentAP;
    public List<int> skillList = new List<int>();
    public HashSet<int> usedSkillsThisTurn = new HashSet<int>();

    public void OnTurnStart()
    {
        usedSkillsThisTurn.Clear();
    }

    public void InitPlayer(int playerID)
    {
        if (!DataManager.Instance.playerDict.ContainsKey(playerID))
        {
            Debug.LogError($"플레이어 ID {playerID} 없음");
            return;
        }

        var data = DataManager.Instance.playerDict[playerID];

        id = data.id;
        charName = data.name;
        maxHP = data.hp;
        currentHP = maxHP;
        currentAP = data.actionPoint;
        skillList = new List<int>(data.skills);

        Debug.Log($"[InitPlayer] {charName}, id:{id}, skillCount:{skillList.Count}");

        foreach (int skillId in skillList)
        {
            Debug.Log($"[InitPlayer Skill] {skillId}");
        }
    }

    public void InitEnemy(int enemyID)
    {
        if (!DataManager.Instance.enemyDict.ContainsKey(enemyID))
        {
            Debug.LogError($"적 ID {enemyID} 없음");
            return;
        }

        var data = DataManager.Instance.enemyDict[enemyID];

        id = data.id;
        charName = data.name;
        maxHP = data.hp;
        currentHP = maxHP;
        skillList = new List<int>(data.attackIds);

        Debug.Log($"[InitEnemy] {charName}, id:{id}, attackCount:{skillList.Count}");
    }

    public void UseSkill(int skillID, Character target)
    {
        Debug.Log($"[UseSkill 호출] skillID:{skillID}, currentAP:{currentAP}");

        if (BattleUI.Instance != null) BattleUI.Instance.ClearLog();

        if (!DataManager.Instance.skillDict.ContainsKey(skillID))
        {
            Debug.LogWarning($"[UseSkill] skillDict에 없음: {skillID}");
            return;
        }

        var skill = DataManager.Instance.skillDict[skillID];

        if (currentAP < skill.actionCost)
        {
            Debug.LogWarning("[UseSkill] 행동력 부족");
            if (BattleUI.Instance != null) BattleUI.Instance.AddLog("행동력 부족!");
            return;
        }

        if (!skill.usingAgain && usedSkillsThisTurn.Contains(skillID))
        {
            Debug.LogWarning("[UseSkill] 이번 턴에 이미 사용한 스킬");
            if (BattleUI.Instance != null) BattleUI.Instance.AddLog("이 스킬은 한 턴에 한 번만 사용할 수 있습니다!");
            return;
        }

        usedSkillsThisTurn.Add(skillID);
        currentAP -= skill.actionCost;

        Debug.Log($"[UseSkill 성공] {skill.name}, 남은 AP:{currentAP}");

        if (BattleUI.Instance != null)
            BattleUI.Instance.AddLog($"[{charName}]이(가) <{skill.name}>을(를) 사용했습니다!");

        for (int i = 0; i < skill.skillAbilityIds.Count; i++)
        {
            int abilityID = skill.skillAbilityIds[i];
            float currentEffect = (i < skill.skillEffectValues.Count) ? skill.skillEffectValues[i] : 0f;

            Debug.Log($"[ExecuteAbility 준비] abilityID:{abilityID}, effect:{currentEffect}");
            ExecuteAbility(abilityID, currentEffect, target);
        }
    }

    public void UseEnemySkill(int attackID, Character target)
    {
        if (BattleUI.Instance != null) BattleUI.Instance.ClearLog();

        if (!DataManager.Instance.enemyAttackDict.ContainsKey(attackID))
        {
            Debug.LogWarning($"[UseEnemySkill] enemyAttackDict에 없음: {attackID}");
            return;
        }

        var attack = DataManager.Instance.enemyAttackDict[attackID];

        if (BattleUI.Instance != null)
            BattleUI.Instance.AddLog($"[{charName}]이(가) <{attack.name}>을(를) 사용했습니다!");

        if (DataManager.Instance.abilityDict.ContainsKey(attack.skillAbilityId))
        {
            foreach (var effectValue in attack.attackEffectValues)
            {
                Debug.Log($"[Enemy ExecuteAbility] abilityID:{attack.skillAbilityId}, effect:{effectValue}");
                ExecuteAbility(attack.skillAbilityId, effectValue, target);
                break;
            }
        }
    }

    void ExecuteAbility(int abilityID, float skillEffect, Character target)
    {
        if (!DataManager.Instance.abilityDict.ContainsKey(abilityID))
        {
            Debug.LogWarning($"[ExecuteAbility] abilityDict에 없음: {abilityID}");
            return;
        }

        var ability = DataManager.Instance.abilityDict[abilityID];
        
        EffectType parsedEffect = EffectType.NormalAttack;
        if (!string.IsNullOrEmpty(ability.effect))
        {
            System.Enum.TryParse(ability.effect, true, out parsedEffect);
        }
        
        Debug.Log($"[ExecuteAbility] effectType:{parsedEffect}");

        switch (parsedEffect)
        {
            case EffectType.NormalAttack:
                target.TakeDamage(skillEffect);
                break;

            case EffectType.Heal:
                currentHP += skillEffect;
                if (currentHP > maxHP) currentHP = maxHP;

                if (BattleUI.Instance != null)
                    BattleUI.Instance.AddLog($"[{charName}]이(가) 체력을 {skillEffect} 회복하여 {currentHP}이 되었습니다.");
                break;

            case EffectType.Defence:
                if (BattleUI.Instance != null)
                    BattleUI.Instance.AddLog($"[{charName}]이(가) 방어 태세를 취했습니다.");
                break;

            default:
                if (BattleUI.Instance != null)
                    BattleUI.Instance.AddLog($"[{charName}]의 능력 [{ability.effect}] 이(가) 아직 구현되지 않았습니다.");
                break;
        }
    }

    public void TakeDamage(float amount)
    {
        currentHP -= amount;

        if (currentHP <= 0)
            currentHP = 0;

        Debug.Log($"[TakeDamage] {charName}, damage:{amount}, remainHP:{currentHP}");

        if (BattleUI.Instance != null)
            BattleUI.Instance.AddLog($"[{charName}]이(가) {amount} 대미지를 입음! (남은 체력: {currentHP})");
    }
}