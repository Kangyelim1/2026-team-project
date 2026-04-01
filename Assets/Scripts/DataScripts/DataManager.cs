using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

public class DataManager : MonoBehaviour {
    public static DataManager Instance;

    [Header("CSV Files")]
    public TextAsset playerFile;
    public TextAsset skillFile;
    public TextAsset enemyFile;
    public TextAsset enemyAttackFile;
    public TextAsset abilityFile;

    [Header("Data Tables")]
    public List<PlayerData> playerTable = new List<PlayerData>();
    public List<SkillData> skillTable = new List<SkillData>();
    public List<EnemyData> enemyTable = new List<EnemyData>();
    public List<EnemyAttackData> enemyAttackTable = new List<EnemyAttackData>();
    public List<AbilityData> abilityTable = new List<AbilityData>();

    public Dictionary<int, PlayerData> playerDict = new Dictionary<int, PlayerData>();
    public Dictionary<int, SkillData> skillDict = new Dictionary<int, SkillData>();
    public Dictionary<int, EnemyData> enemyDict = new Dictionary<int, EnemyData>();
    public Dictionary<int, EnemyAttackData> enemyAttackDict = new Dictionary<int, EnemyAttackData>();
    public Dictionary<int, AbilityData> abilityDict = new Dictionary<int, AbilityData>();

    void Awake() {
        if (Instance == null) Instance = this;
        LoadAllData();
    }

    void LoadAllData() {
        ParseTable(playerFile, LoadPlayerRow);
        ParseTable(skillFile, LoadSkillRow);
        ParseTable(enemyFile, LoadEnemyRow);
        ParseTable(enemyAttackFile, LoadEnemyAttackRow);
        ParseTable(abilityFile, LoadAbilityRow);
        Debug.Log("모든 데이터 로드 완료!");
    }

    void ParseTable(TextAsset file, Action<string[]> rowParser) {
        if (file == null) return;
        string[] lines = file.text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 2; i < lines.Length; i++) {
            string[] cols = Regex.Split(lines[i], ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
            for (int j = 0; j < cols.Length; j++) cols[j] = cols[j].TrimStart('\"').TrimEnd('\"');
            rowParser(cols);
        }
    }

    // 1. PlayerDataTable 파싱
    void LoadPlayerRow(string[] cols) {
        if (cols.Length < 5) return;
        PlayerData data = new PlayerData();
        data.id = int.Parse(cols[0]);
        data.name = cols[1];
        data.skills = ParseIntList(cols[2]);
        data.hp = float.Parse(cols[3]);
        data.action = int.Parse(cols[4]);
        if (cols.Length > 5) data.encounteredEnemies = ParseIntList(cols[5]);

        playerTable.Add(data);
        playerDict.Add(data.id, data);
    }

    // 2. SkillDataTable 파싱 (기존 로직 확장)
    void LoadSkillRow(string[] cols) {
    // 1. 열 개수가 부족하면 무시 (빈 줄 방지)
    if (cols.Length < 10 || string.IsNullOrEmpty(cols[0])) return;

    SkillData data = new SkillData();
    data.id = int.Parse(cols[0]);

    // 이름/주인 분리 로직
    string fullName = cols[1];
    if (fullName.Contains("(") && fullName.Contains(")")) {
        int openBracket = fullName.IndexOf('(');
        data.name = fullName.Substring(0, openBracket);
        data.owner = fullName.Substring(openBracket + 1).Replace(")", "");
    } else {
        data.name = fullName;
        data.owner = "Common";
    }

    // 2. Enum.TryParse를 사용하여 안전하게 파싱 (에러 방지 핵심)
    if (Enum.TryParse(cols[2], true, out SkillType sType)) {
        data.skillType = sType;
    } else {
        Debug.LogWarning($"[ID {data.id}] SkillType이 비어있거나 잘못되었습니다: {cols[2]}");
        data.skillType = SkillType.active; // 기본값 설정
    }

    data.skillAbilities = ParseIntList(cols[3]);
    data.effect = float.Parse(cols[4]);
    data.cost = int.Parse(cols[5]);
    data.isEnhance = cols[6] == "1";
    data.isActive = cols[7] == "1";
    data.useAgain = cols[8] == "1";

    if (Enum.TryParse(cols[9], true, out DamageType dType)) {
        data.damageType = dType;
    } else {
        Debug.LogWarning($"[ID {data.id}] DamageType이 비어있거나 잘못되었습니다: {cols[9]}");
        data.damageType = DamageType.direct; // 기본값 설정
    }

    if (cols.Length > 10 && !string.IsNullOrEmpty(cols[10])) 
        data.enhanceFigure = float.Parse(cols[10]);

    skillTable.Add(data);
    skillDict.Add(data.id, data);
}

    // 3. EnemyDataTable 파싱
    void LoadEnemyRow(string[] cols) {
        if (cols.Length < 4) return;
        EnemyData data = new EnemyData();
        data.id = int.Parse(cols[0]);
        data.name = cols[1];
        data.attacks = ParseIntList(cols[2]);
        data.hp = float.Parse(cols[3]);

        enemyTable.Add(data);
        enemyDict.Add(data.id, data);
    }

    // 4. Enemy_Atteck 파싱
    void LoadEnemyAttackRow(string[] cols) {
        if (cols.Length < 5) return;
        EnemyAttackData data = new EnemyAttackData();
        data.id = int.Parse(cols[0]);
        data.name = cols[1];
        data.attackType = (SkillType)Enum.Parse(typeof(SkillType), cols[2].ToLower());
        data.skillAbilities = ParseIntList(cols[3]);
        data.attackEffects = ParseIntList(cols[4]);

        enemyAttackTable.Add(data);
        enemyAttackDict.Add(data.id, data);
    }

    // 5. Skill_AbilityTable 파싱
    void LoadAbilityRow(string[] cols) {
        if (cols.Length < 2) return;
        AbilityData data = new AbilityData();
        data.id = int.Parse(cols[0]);
        data.effect = (EffectType)Enum.Parse(typeof(EffectType), cols[1]);
        if (cols.Length > 2) data.description = cols[2];

        abilityTable.Add(data);
        abilityDict.Add(data.id, data);
    }

    // 셀 내부의 "1,2,3" 같은 문자열을 List<int>로 변환하는 도구
    List<int> ParseIntList(string str) {
        List<int> list = new List<int>();
        if (string.IsNullOrEmpty(str)) return list;
        string[] items = str.Split(',');
        foreach (var item in items) {
            if (int.TryParse(item.Trim(), out int result)) list.Add(result);
        }
        return list;
    }
}