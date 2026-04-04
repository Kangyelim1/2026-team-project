using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

public class DataManager : MonoBehaviour {
    public static DataManager Instance;
    public static int SelectedPlayerID = 1;

    [Header("CSV Files")]
    public TextAsset playerFile;
    public TextAsset skillFile;
    public TextAsset warpSkillFile;
    public TextAsset enemyFile;
    public TextAsset enemyAttackFile;
    public TextAsset abilityFile;

    public Dictionary<int, PlayerData> playerDict = new Dictionary<int, PlayerData>();
    public Dictionary<int, SkillData> skillDict = new Dictionary<int, SkillData>();
    public Dictionary<int, EnemyData> enemyDict = new Dictionary<int, EnemyData>();
    public Dictionary<int, EnemyAttackData> enemyAttackDict = new Dictionary<int, EnemyAttackData>();
    public Dictionary<int, AbilityData> abilityDict = new Dictionary<int, AbilityData>();

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAllData();
        }
    }

    void LoadAllData() {
        // 순서 중요: 플레이어를 먼저 만들고 스킬을 로드하며 플레이어에게 할당
        ParseTable(playerFile, LoadPlayerRow);
        ParseTable(skillFile, LoadSkillRow);
        ParseTable(warpSkillFile, LoadWarpSkillRow);
        ParseTable(enemyFile, LoadEnemyRow);
        ParseTable(enemyAttackFile, LoadEnemyAttackRow);
        ParseTable(abilityFile, LoadAbilityRow);
        Debug.Log("모든 데이터 로드 완료!");
    }

    void ParseTable(TextAsset file, Action<string[]> rowParser) {
        if (file == null) return;
        string[] lines = file.text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        // i = 2부터 시작하여 헤더(i=0)와 타입설명(i=1) 행을 건너뜀
        for (int i = 2; i < lines.Length; i++) {
            string[] cols = Regex.Split(lines[i], ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
            for (int j = 0; j < cols.Length; j++) cols[j] = cols[j].TrimStart('\"').TrimEnd('\"');
            rowParser(cols);
        }
    }

    void LoadPlayerRow(string[] cols) {
        if (cols.Length < 4) return;
        PlayerData data = new PlayerData();
        data.id = int.Parse(cols[0]);
        data.name = cols[1];
        data.hp = float.Parse(cols[2]);
        data.action = int.Parse(cols[3]);
        if (cols.Length > 4 && Enum.TryParse(cols[4], true, out PlayerTrait pTrait)) data.trait = pTrait;
        if (cols.Length > 5) data.encounteredEnemies = ParseIntList(cols[5]);
        playerDict.Add(data.id, data);
    }

    void LoadSkillRow(string[] cols) {
        if (cols.Length < 10 || string.IsNullOrEmpty(cols[0])) return;
        SkillData data = new SkillData();
        if (!int.TryParse(cols[0], out data.id)) return;
        string fullName = cols[1];
        if (fullName.Contains("(") && fullName.Contains(")")) {
            int openBracket = fullName.IndexOf('(');
            data.name = fullName.Substring(0, openBracket);
            data.owner = fullName.Substring(openBracket + 1).Replace(")", "");
        } else {
            data.name = fullName;
            data.owner = "Common";
        }

        // 구버전 SkillDataTable 매핑 형식 (ID 기반 할당)
        if (int.TryParse(cols[2], out int ownerID)) {
            if (playerDict.ContainsKey(ownerID)) playerDict[ownerID].skills.Add(data.id);
        }
        if (Enum.TryParse(cols[3], true, out SkillType sType)) data.skillType = sType;

        data.skillAbilities = ParseIntList(cols[4]);
        data.effects = ParseFloatList(cols[5]);
        if (!int.TryParse(cols[6], out data.cost)) data.cost = 0;
        data.isActive = cols[7] == "1";
        data.useAgain = cols[8] == "1";

        if (Enum.TryParse(cols[9], true, out DamageType dType)) data.damageType = dType;
        data.isEnhance = cols[10] == "1";
        if (cols.Length > 11 && !float.TryParse(cols[11], out data.enhanceFigure)) data.enhanceFigure = 0;

        if (!skillDict.ContainsKey(data.id)) skillDict.Add(data.id, data);
    }

    void LoadWarpSkillRow(string[] cols) {
        if (cols.Length < 10 || string.IsNullOrEmpty(cols[0])) return;
        SkillData data = new SkillData();
        if (!int.TryParse(cols[0], out data.id)) return;
        string fullName = cols[1];
        if (fullName.Contains("(") && fullName.Contains(")")) {
            int openBracket = fullName.IndexOf('(');
            data.name = fullName.Substring(0, openBracket);
            data.owner = fullName.Substring(openBracket + 1).Replace(")", "");
        } else {
            data.name = fullName;
            data.owner = "Common";
        }

        // 신버전 WarpSkillDataTable 매핑 형식 (Trait 기반 할당 및 컬럼 순서 변경)
        if (Enum.TryParse(cols[2], true, out SkillType sType)) data.skillType = sType;

        string userStr = cols[3];
        if (userStr.Equals("Common", StringComparison.OrdinalIgnoreCase)) {
            foreach (var p in playerDict.Values) p.skills.Add(data.id);
        } else if (Enum.TryParse(userStr, true, out PlayerTrait ownerTrait)) {
            foreach (var p in playerDict.Values) {
                if (p.trait == ownerTrait) p.skills.Add(data.id);
            }
        }

        data.skillAbilities = ParseIntList(cols[4]);
        data.effects = ParseFloatList(cols[5]);
        if (!int.TryParse(cols[6], out data.cost)) data.cost = 0;
        data.isActive = cols[7] == "1";
        data.useAgain = cols[8] == "1";

        data.isEnhance = cols[9] == "1";
        if (cols.Length > 10 && !float.TryParse(cols[10], out data.enhanceFigure)) data.enhanceFigure = 0;
        if (cols.Length > 11 && Enum.TryParse(cols[11], true, out DamageType dType)) data.damageType = dType;

        if (!skillDict.ContainsKey(data.id)) skillDict.Add(data.id, data);
    }

    void LoadEnemyRow(string[] cols) {
        if (cols.Length < 4) return;
        EnemyData data = new EnemyData();
        data.id = int.Parse(cols[0]);
        data.name = cols[1];
        data.attacks = ParseIntList(cols[2]);
        data.hp = float.Parse(cols[3]);
        if (cols.Length > 4 && Enum.TryParse(cols[4], true, out EnemyTrait eTrait)) data.trait = eTrait;
        enemyDict.Add(data.id, data);
    }

    void LoadEnemyAttackRow(string[] cols) {
        if (cols.Length < 5) return;
        EnemyAttackData data = new EnemyAttackData();
        data.id = int.Parse(cols[0]);
        data.name = cols[1];
        if (Enum.TryParse(cols[2], true, out SkillType sType)) data.attackType = sType;
        data.skillAbilities = ParseIntList(cols[3]);
        data.attackEffects = ParseIntList(cols[4]);
        if (cols.Length > 5 && Enum.TryParse(cols[5], true, out DamageType dType)) data.damageType = dType;
        enemyAttackDict.Add(data.id, data);
    }

    void LoadAbilityRow(string[] cols) {
        if (cols.Length < 3) return;
        AbilityData data = new AbilityData();
        data.id = int.Parse(cols[0]);
        if (Enum.TryParse(cols[1], true, out EffectType eType)) data.effect = eType;
        if (Enum.TryParse(cols[2], true, out AbilityCategory cat)) data.category = cat;
        if (cols.Length > 3) data.description = cols[3];
        abilityDict.Add(data.id, data);
    }

    List<int> ParseIntList(string str) {
        List<int> list = new List<int>();
        if (string.IsNullOrEmpty(str)) return list;
        string[] items = str.Split(',');
        foreach (var item in items) {
            if (int.TryParse(item.Trim(), out int result)) list.Add(result);
        }
        return list;
    }

    List<float> ParseFloatList(string str) {
        List<float> list = new List<float>();
        if (string.IsNullOrEmpty(str)) return list;
        string[] items = str.Split(',');
        foreach (var item in items) {
            if (float.TryParse(item.Trim(), out float result)) list.Add(result);
        }
        return list;
    }
}