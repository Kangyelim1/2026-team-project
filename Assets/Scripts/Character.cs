using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour 
{
    public int currentHP = 100;
    public int currentMP = 10;

    public void UseSkill(int skillID, Character target = null) 
    {
        if (!DataManager.Instance.skillDict.ContainsKey(skillID)) return;
        SkillData skill = DataManager.Instance.skillDict[skillID];

        if (currentMP < skill.cost) {
            Debug.Log("코스트 부족!");
            return;
        }
        currentMP -= skill.cost;

        foreach (int abilityID in skill.skillAbilities) {
            ExecuteAbility(abilityID, skill.effect, target);
        }
    }

    void ExecuteAbility(int id, float power, Character target) 
    {
        if (!DataManager.Instance.abilityDict.ContainsKey(id)) return;
        AbilityData ability = DataManager.Instance.abilityDict[id];

        switch (ability.effect) {
            case EffectType.Attack:
                if (target != null) {
                    target.currentHP -= (int)power;
                    Debug.Log($"{target.name}에게 {power}만큼 공격!"); 
                }
                break;
            case EffectType.Heal:
                currentHP += (int)power;
                Debug.Log($"{power}만큼 회복!");
                break;
        }
    }
}