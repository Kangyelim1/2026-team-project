using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions; // CSV 파싱용 정규식 추가

public class DataManager : MonoBehaviour {
    public static DataManager Instance;

    public TextAsset skillFile;
    public TextAsset abilityFile;

    public List<SkillData> skillTable = new List<SkillData>();
    public List<AbilityData> abilityTable = new List<AbilityData>();

    public Dictionary<int, SkillData> skillDict = new Dictionary<int, SkillData>();
    public Dictionary<int, AbilityData> abilityDict = new Dictionary<int, AbilityData>();

    void Awake() {
        if (Instance == null) Instance = this;
        LoadAllData();
    }

    void LoadAllData() {
        ParseTable(skillFile, LoadSkillRow);
        ParseTable(abilityFile, LoadAbilityRow);
    }

    // [수정됨] CSV의 쉼표 구분을 완벽하게 처리하는 로직
    void ParseTable(TextAsset file, System.Action<string[]> rowParser) {
        if (file == null) return;
        
        // 줄바꿈 기준으로 행 분리
        string[] lines = file.text.Split(new string[] { "\r\n", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        
        for (int i = 2; i < lines.Length; i++) { // 헤더 2줄 건너뜀
            // [중요] 일반 Split(',') 대신 정규식을 사용하여 큰따옴표 안의 쉼표는 무시함
            string[] cols = Regex.Split(lines[i], ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
            
            // 각 셀의 불필요한 따옴표 제거
            for (int j = 0; j < cols.Length; j++) {
                cols[j] = cols[j].TrimStart('\"').TrimEnd('\"');
            }
            
            rowParser(cols);
        }
    }

    void LoadSkillRow(string[] cols) {
        if (cols.Length < 10) return;

        SkillData data = new SkillData();
        data.id = int.Parse(cols[0]);

        string fullName = cols[1];
        if (fullName.Contains("(") && fullName.Contains(")")) {
            int openBracket = fullName.IndexOf('(');
            data.name = fullName.Substring(0, openBracket);
            data.owner = fullName.Substring(openBracket + 1).Replace(")", "");
        } else {
            data.name = fullName;
            data.owner = "Common";
        }

        data.skillType = (SkillType)System.Enum.Parse(typeof(SkillType), cols[2]);

        // 스킬 어빌리티 리스트 처리
        string[] abilities = cols[3].Split(',');
        foreach (var a in abilities) {
            if (int.TryParse(a.Trim(), out int id)) data.skillAbilities.Add(id);
        }

        data.effect = float.Parse(cols[4]);
        data.cost = int.Parse(cols[5]);
        data.isEnhance = cols[6] == "1";
        data.isActive = cols[7] == "1";
        data.useAgain = cols[8] == "1";
        data.damageType = (DamageType)System.Enum.Parse(typeof(DamageType), cols[9]);

        skillTable.Add(data);
        if (!skillDict.ContainsKey(data.id)) skillDict.Add(data.id, data);
    }

    void LoadAbilityRow(string[] cols) {
        if (cols.Length < 2) return;

        AbilityData data = new AbilityData();
        data.id = int.Parse(cols[0]);
        data.effect = (EffectType)System.Enum.Parse(typeof(EffectType), cols[1]);
        if (cols.Length > 2) data.description = cols[2];

        abilityTable.Add(data);
        if (!abilityDict.ContainsKey(data.id)) abilityDict.Add(data.id, data);
    }
}