using System;
using System.Collections.Generic;

// --- 열거형 (Enums) ---

public enum SkillType 
{ 
    Active, 
    Passive 
}

public enum DamageType 
{ 
    direct, 
    summoner, 
    indirect 
}

public enum PlayerTrait
{
    Summoner,
    Hunter
}

public enum EnemyTrait
{
    Mob,
    Elite,
    Boss
}

public enum AbilityCategory
{
    Aggressive,
    Defensive,
    Positive,
    Negative
}

public enum EffectType // Skill_AbilityTable의 Effect 컬럼과 일치시킴
{ 
    Normal_Attack,      // 100
    Attack_Assist,      // 101
    Counter,            // 102
    Double_Atteck,      // 103
    Armor_Penetration,  // 104
    Defence,            // 105
    Heal,               // 106
    Damage_Limit,       // 107
    Death_Evasion,      // 108
    Skill_Enforce       // 109
}

// --- 테이블 데이터 클래스 ---

[Serializable]
public class PlayerData 
{
    public int id;
    public string name;
    public List<int> skills = new List<int>(); // SkillDataTable 로드 시 User ID를 기반으로 자동 채워짐
    public float hp;
    public int action;
    public PlayerTrait trait;
    public List<int> encounteredEnemies = new List<int>();
}

[Serializable]
public class SkillData 
{
    public int id;
    public string name;
    public string owner; // "기본공격(콩쥐)" 에서 "콩쥐" 추출
    public SkillType skillType;
    public List<int> skillAbilities = new List<int>();
    public List<float> effects = new List<float>();
    public int cost;
    public bool isEnhance;
    public bool isActive;
    public bool useAgain;
    public DamageType damageType;
    public float enhanceFigure;
}

[Serializable]
public class EnemyData 
{
    public int id;
    public string name;
    public List<int> attacks = new List<int>(); // Enemy_Atteck[ID] 리스트
    public float hp;
    public EnemyTrait trait;
}

[Serializable]
public class EnemyAttackData 
{
    public int id;
    public string name;
    public SkillType attackType;
    public List<int> skillAbilities = new List<int>();
    public List<int> attackEffects = new List<int>(); // Atteck_Effect 랜덤 리스트
    public DamageType damageType; // 추가됨
}

[Serializable]
public class AbilityData 
{
    public int id;
    public EffectType effect;
    public AbilityCategory category;
    public string description;
}