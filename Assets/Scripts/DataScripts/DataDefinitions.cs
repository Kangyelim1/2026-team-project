using System;
using System.Collections.Generic;

// 1. 엑셀의 enum들 모음
public enum SkillType { active, passive }
public enum DamageType { direct, summoner, indirect }

// 2. 엑셀 한 줄과 1:1로 대응하는 '스킬' 양식
[Serializable]
public class SkillData {
    public int id;
    public string name;
    public string owner; //쓰는 사용자
    public SkillType skillType;
    public List<int> skillAbilities = new List<int>(); //셀 안에 쉼표로 구분된 숫자들을 리스트로 변환
    public float effect;
    public int cost;
    public bool isEnhance;
    public bool isActive;
    public bool useAgain;
    public DamageType damageType;
}

[Serializable]
public class AbilityData 
{
    public int id;             // ID (100, 101...)
    public EffectType effect;  // Effect (Attack, Heal...)
    public string description; // ig,desc (설명용)
}

// 3. 나중에 추가될 '적'이 쓸 수치 양식 (여기에 계속 추가하면 됨)
[Serializable]
public class EnemyData {
    public int id;
    public string name;
    public int hp;
}

public enum EffectType //행동
{ 
    Attack,           // 공격
    Defence,          // 방어
    Heal,             // 회복
    Attack_Assist,    // 공격 보조
    Death_Evasion     // 사망 회피
}

