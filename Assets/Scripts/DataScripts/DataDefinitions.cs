using System;
using System.Collections.Generic;

public enum SkillType
{
    active,
    passive
}

public enum DamageType
{
    direct,
    summon,
    indirect
}

public enum PlayerTrait
{
    Summoner,
    Hunter,
    Common
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

public enum EffectType
{
    NormalAttack,
    AttackAssist,
    Counter,
    DoubleAttack,
    ArmorPenetration,
    Defence,
    Heal,
    DamageLimit,
    DeathEvasion,
    SkillEnforce,
    Negative
}

[Serializable]
public class PlayerData
{
    public int id;
    public string name;
    public float hp;
    public int actionPoint;
    public PlayerTrait trait;
    public List<int> encounteredEnemyIds = new List<int>();
    public List<int> skills = new List<int>();
}

[Serializable]
public class SkillData
{
    public int id;
    public string name;
    public int userId;
    public SkillType skillType;
    public List<int> skillAbilityIds = new List<int>();
    public List<float> skillEffectValues = new List<float>();
    public int actionCost;
    public bool activate;
    public bool usingAgain;
    public DamageType damageType;
    public bool skillEnhance;
    public float enhanceFigure;
}

[Serializable]
public class WarpSkillData
{
    public int id;
    public string name;
    public SkillType skillType;
    public PlayerTrait userTrait;
    public List<int> skillAbilityIds = new List<int>();
    public List<float> skillEffectValues = new List<float>();
    public int actionCost;
    public bool activate;
    public bool usingAgain;
    public bool skillEnhance;
    public float enhanceFigure;
    public DamageType damageType;
}

[Serializable]
public class EnemyData
{
    public int id;
    public string name;
    public List<int> attackIds = new List<int>();
    public float hp;
    public EnemyTrait trait;
}

[Serializable]
public class EnemyAttackData
{
    public int id;
    public string name;
    public SkillType attackType;
    public int skillAbilityId;
    public List<int> attackEffectValues = new List<int>();
    public DamageType damageType;
}

[Serializable]
public class AbilityData
{
    public int id;
    public EffectType effect;
    public AbilityCategory category;
    public string description;
}

[Serializable]
public class PlayerDataTableWrapper
{
    public List<PlayerData> rows = new List<PlayerData>();
}

[Serializable]
public class SkillDataTableWrapper
{
    public List<SkillData> rows = new List<SkillData>();
}

[Serializable]
public class WarpSkillDataTableWrapper
{
    public List<WarpSkillData> rows = new List<WarpSkillData>();
}

[Serializable]
public class EnemyDataTableWrapper
{
    public List<EnemyData> rows = new List<EnemyData>();
}

[Serializable]
public class EnemyAttackTableWrapper
{
    public List<EnemyAttackData> rows = new List<EnemyAttackData>();
}

[Serializable]
public class AbilityTableWrapper
{
    public List<AbilityData> rows = new List<AbilityData>();
}