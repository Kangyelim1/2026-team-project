using System;
using System.Collections.Generic;

// --- 열거형 (Enums) ---

public enum SkillType 
{ 
    active, 
    passive 
}

public enum DamageType 
{ 
    direct, 
    summoner, 
    indirect 
}

public enum EffectType // 행동 방식 종류
{ 
    Attack,           // 100: 상대방을 공격한다
    Defence,          // 101: 상대방의 공격을 방어한다
    Heal,             // 102: 자신의 체력을 회복한다
    Attack_Assist,    // 103: 자신의 공격을 강화한다
    Death_Evasion,    // 104: 사망 시 한번만 1체력이 된다
    Counter,          // 105: 상대방이 direct 공격을 할 때 피해를 준다
    Damage_Limit,     // 106: 한턴에 일정 이상 피해를 입지 않는다
    Double_Atteck     // 107: 데미지를 입히면 발동. (엑셀 스펠링에 맞춤)
}

// --- 테이블 데이터 클래스 모음 ---

// 1. PlayerDataTable : 플레이어의 기본 정보
[Serializable]
public class PlayerData 
{
    public int id;
    public string name;                  // ig.Name(기획용)
    public List<int> skills = new List<int>(); // 보유 스킬 (SkillDataTable[ID])
    public float hp;
    public int action;                   // Action
    public List<int> encounteredEnemies = new List<int>(); // 마주친 적 목록
}

// 2. SkillDataTable : 스킬 상세 정보
[Serializable]
public class SkillData 
{
    public int id;
    public string name;
    public string owner;                 // 쓰는 사용자 (기존 코드 호환용 추가)
    public SkillType skillType;          // 스킬 형태 (active, passive)
    public List<int> skillAbilities = new List<int>(); // 연결된 능력 정보
    public float effect;                 // Skill_Effcet (스킬 수치)
    public int cost;                     // Action_Cost (행동 비용)
    public bool isEnhance;               // Skill_Enhance (0 또는 1)
    public bool isActive;                // activate (0 또는 1)
    public bool useAgain;                // Using_Again (0 또는 1)
    public DamageType damageType;        // Type of damage
    public float enhanceFigure;          // Enhance_Figure (강화 수치)
}

// 3. EnemyDataTable : 적 기본 상태 정보
[Serializable]
public class EnemyData 
{
    public int id;
    public string name;
    public List<int> attacks = new List<int>(); // Atteck (Enemy_Atteck[ID] 리스트)
    public float hp;                            // 체력
}

// 4. Enemy_Atteck : 적의 공격/행동 패턴
[Serializable]
public class EnemyAttackData 
{
    public int id;
    public string name;
    public SkillType attackType;         // Atteck_Type (active/passive - SkillType과 동일 규격)
    public List<int> skillAbilities = new List<int>(); // 필요시 연결할 스킬 능력
    public List<int> attackEffects = new List<int>();  // Atteck_Effect (랜덤 리스트)
}

// 5. Skill_AbilityTable : 효과 정보 테이블
[Serializable]
public class AbilityData 
{
    public int id;
    public EffectType effect;            // Effect
    public string description;           // ig.desc(설명용)
}
