using System.Collections;
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
    public int maxAP; // [추가됨] 최대 행동력 저장용

    public List<int> skillList = new List<int>();
    public HashSet<int> usedSkillsThisTurn = new HashSet<int>();

    public void OnTurnStart()
    {
        usedSkillsThisTurn.Clear();
    }

    public void InitPlayer(int playerID)
    {
        if (!DataManager.Instance.playerDict.ContainsKey(playerID)) return;

        var data = DataManager.Instance.playerDict[playerID];
        id = data.id;
        charName = data.name;
        maxHP = data.hp;
        currentHP = maxHP;

        // [수정됨] 현재 행동력과 최대 행동력을 모두 초기화
        currentAP = data.actionPoint;
        maxAP = data.actionPoint;

        skillList = new List<int>(data.skills);
    }

    public void InitEnemy(int enemyID)
    {
        if (!DataManager.Instance.enemyDict.ContainsKey(enemyID)) return;

        var data = DataManager.Instance.enemyDict[enemyID];
        id = data.id;
        charName = data.name;
        maxHP = data.hp;
        currentHP = maxHP;
        skillList = new List<int>(data.attackIds);
    }

    public void UseSkill(int skillID, Character target)
    {
        if (!DataManager.Instance.skillDict.ContainsKey(skillID)) return;
        var skill = DataManager.Instance.skillDict[skillID];

        if (currentAP < skill.actionCost) return;

        if (!skill.usingAgain && usedSkillsThisTurn.Contains(skillID)) return;

        usedSkillsThisTurn.Add(skillID);
        currentAP -= skill.actionCost;

        if (BattleUI.Instance != null)
            BattleUI.Instance.AddLog($"{charName}가 {skill.name}을(를) 사용했습니다!");

        for (int i = 0; i < skill.skillAbilityIds.Count; i++)
        {
            int abilityID = skill.skillAbilityIds[i];
            float currentEffect = (i < skill.skillEffectValues.Count) ? skill.skillEffectValues[i] : 0f;
            ExecuteAbility(abilityID, currentEffect, target);
        }

        
    }

    public void UseEnemySkill(int attackID, Character target)
    {
        if (BattleUI.Instance != null) BattleUI.Instance.ClearLog();

        if (!DataManager.Instance.enemyAttackDict.ContainsKey(attackID)) return;
        var attack = DataManager.Instance.enemyAttackDict[attackID];

        if (BattleUI.Instance != null)
            BattleUI.Instance.AddLog($"{charName}의 {attack.name} 공격!");

        if (DataManager.Instance.abilityDict.ContainsKey(attack.skillAbilityId))
        {
            foreach (var effectValue in attack.attackEffectValues)
            {
                ExecuteAbility(attack.skillAbilityId, effectValue, target);
                break;
            }
        }
    }

    private void ExecuteAbility(int abilityID, float skillEffect, Character target)
    {
        if (!DataManager.Instance.abilityDict.ContainsKey(abilityID)) return;
        var ability = DataManager.Instance.abilityDict[abilityID];
        EffectType parsedEffect = EffectType.NormalAttack;

        if (!string.IsNullOrEmpty(ability.effect))
            System.Enum.TryParse(ability.effect, true, out parsedEffect);

        switch (parsedEffect)
        {
            case EffectType.NormalAttack:
                target.TakeDamage(skillEffect);
                if(BattleUI.Instance != null)
                {
                    bool isPlayer = (target == BattleManager.Instance.player);
                    BattleUI.Instance.ShowDamage(isPlayer, (int)skillEffect);
                }
                break;
            case EffectType.Heal:
                currentHP += skillEffect;
                if (currentHP > maxHP) currentHP = maxHP;
                break;
        }
    }

    public void TakeDamage(float amount)
    {
        currentHP -= amount;
        if (currentHP < 0) currentHP = 0;
    }
}