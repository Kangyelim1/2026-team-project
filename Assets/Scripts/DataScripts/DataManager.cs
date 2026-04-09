using UnityEngine;
using System.Collections.Generic;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    public static int SelectedPlayerID = 1;

    // [Header("JSON Files")]
    // public TextAsset playerFile;
    // public TextAsset skillFile;
    // public TextAsset warpSkillFile;
    // public TextAsset enemyFile;
    // public TextAsset enemyAttackFile;
    // public TextAsset abilityFile;

    public Dictionary<int, PlayerData> playerDict = new Dictionary<int, PlayerData>();
    public Dictionary<int, SkillData> skillDict = new Dictionary<int, SkillData>();
    public Dictionary<int, EnemyData> enemyDict = new Dictionary<int, EnemyData>();
    public Dictionary<int, EnemyAttackData> enemyAttackDict = new Dictionary<int, EnemyAttackData>();
    public Dictionary<int, AbilityData> abilityDict = new Dictionary<int, AbilityData>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAllData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Debug.Log($"[DataManager Start] Player:{playerDict.Count}, Skill:{skillDict.Count}, Enemy:{enemyDict.Count}, EnemyAttack:{enemyAttackDict.Count}, Ability:{abilityDict.Count}");
    }

    void LoadAllData()
    {
        playerDict.Clear();
        skillDict.Clear();
        enemyDict.Clear();
        enemyAttackDict.Clear();
        abilityDict.Clear();

        LoadAbilityJson();
        LoadPlayerJson();
        LoadEnemyAttackJson();
        LoadEnemyJson();
        LoadSkillJson();
        LoadWarpSkillJson();

        Debug.Log("모든 JSON 데이터 로드 완료!");
        Debug.Log($"Player: {playerDict.Count}, Skill: {skillDict.Count}, Enemy: {enemyDict.Count}, EnemyAttack: {enemyAttackDict.Count}, Ability: {abilityDict.Count}");

        foreach (var p in playerDict.Values)
        {
            Debug.Log($"[Player Loaded] id:{p.id}, name:{p.name}, trait:{p.trait}, skillCount:{p.skills.Count}");
        }
    }

    void LoadAbilityJson()
    {
        var abilityFile = Resources.Load<TextAsset>("JsonFile/Skill_AbilityTable");
        if (abilityFile == null)
        {
            Debug.LogError("Resources/JsonFile/Skill_AbilityTable 파일을 찾을 수 없습니다.");
            return;
        }

        var wrapper = JsonUtility.FromJson<AbilityTableWrapper>(abilityFile.text);

        if (wrapper == null || wrapper.rows == null)
        {
            Debug.LogError("Ability JSON 파싱 실패");
            return;
        }

        foreach (var data in wrapper.rows)
        {
            if (!abilityDict.ContainsKey(data.id))
                abilityDict.Add(data.id, data);
        }
    }

    void LoadPlayerJson()
    {
        var playerFile = Resources.Load<TextAsset>("JsonFile/PlayerDataTable");
        if (playerFile == null)
        {
            Debug.LogError("Resources/JsonFile/PlayerDataTable 파일을 찾을 수 없습니다.");
            return;
        }

        var wrapper = JsonUtility.FromJson<PlayerDataTableWrapper>(playerFile.text);

        if (wrapper == null || wrapper.rows == null)
        {
            Debug.LogError("Player JSON 파싱 실패");
            return;
        }

        foreach (var data in wrapper.rows)
        {
            if (!playerDict.ContainsKey(data.id))
                playerDict.Add(data.id, data);
        }
    }

    void LoadEnemyAttackJson()
    {
        var enemyAttackFile = Resources.Load<TextAsset>("JsonFile/Enemy_Atteck");
        if (enemyAttackFile == null)
        {
            Debug.LogError("Resources/JsonFile/Enemy_Atteck 파일을 찾을 수 없습니다.");
            return;
        }

        var wrapper = JsonUtility.FromJson<EnemyAttackTableWrapper>(enemyAttackFile.text);

        if (wrapper == null || wrapper.rows == null)
        {
            Debug.LogError("EnemyAttack JSON 파싱 실패");
            return;
        }

        foreach (var data in wrapper.rows)
        {
            if (!enemyAttackDict.ContainsKey(data.id))
                enemyAttackDict.Add(data.id, data);
        }
    }

    void LoadEnemyJson()
    {
        var enemyFile = Resources.Load<TextAsset>("JsonFile/EnemyDataTable");
        if (enemyFile == null)
        {
            Debug.LogError("Resources/JsonFile/EnemyDataTable 파일을 찾을 수 없습니다.");
            return;
        }

        var wrapper = JsonUtility.FromJson<EnemyDataTableWrapper>(enemyFile.text);

        if (wrapper == null || wrapper.rows == null)
        {
            Debug.LogError("Enemy JSON 파싱 실패");
            return;
        }

        foreach (var data in wrapper.rows)
        {
            if (!enemyDict.ContainsKey(data.id))
                enemyDict.Add(data.id, data);
        }
    }

    void LoadSkillJson()
    {
        var skillFile = Resources.Load<TextAsset>("JsonFile/SkillDataTable");
        if (skillFile == null)
        {
            Debug.LogError("Resources/JsonFile/SkillDataTable 파일을 찾을 수 없습니다.");
            return;
        }

        var wrapper = JsonUtility.FromJson<SkillDataTableWrapper>(skillFile.text);

        if (wrapper == null || wrapper.rows == null)
        {
            Debug.LogError("Skill JSON 파싱 실패");
            return;
        }

        foreach (var data in wrapper.rows)
        {
            if (!skillDict.ContainsKey(data.id))
                skillDict.Add(data.id, data);

            Debug.Log($"[스킬 로드] id:{data.id}, name:{data.name}, userId:{data.userId}, type:{data.skillType}");

            if (playerDict.ContainsKey(data.userId))
            {
                if (!playerDict[data.userId].skills.Contains(data.id))
                {
                    playerDict[data.userId].skills.Add(data.id);
                    Debug.Log($"[플레이어 스킬 추가] player:{data.userId}, skill:{data.id}, name:{data.name}");
                }
            }
            else
            {
                Debug.LogWarning($"[플레이어 없음] userId:{data.userId}");
            }
        }
    }

    void LoadWarpSkillJson()
    {
        var warpSkillFile = Resources.Load<TextAsset>("JsonFile/WarpSkillDataTable");
        if (warpSkillFile == null)
        {
            Debug.LogWarning("Resources/JsonFile/WarpSkillDataTable 파일을 찾을 수 없습니다.");
            return;
        }

        var wrapper = JsonUtility.FromJson<WarpSkillDataTableWrapper>(warpSkillFile.text);

        if (wrapper == null || wrapper.rows == null)
        {
            Debug.LogError("WarpSkill JSON 파싱 실패");
            return;
        }

        foreach (var data in wrapper.rows)
        {
            SkillData converted = new SkillData();
            converted.id = data.id;
            converted.name = data.name;
            converted.userId = 0;
            converted.skillType = data.skillType;
            converted.skillAbilityIds = new List<int>(data.skillAbilityIds);
            converted.skillEffectValues = new List<float>(data.skillEffectValues);
            converted.actionCost = data.actionCost;
            converted.activate = data.activate;
            converted.usingAgain = data.usingAgain;
            converted.damageType = data.damageType;
            converted.skillEnhance = data.skillEnhance;
            converted.enhanceFigure = data.enhanceFigure;

            if (!skillDict.ContainsKey(converted.id))
                skillDict.Add(converted.id, converted);

            foreach (var player in playerDict.Values)
            {
                bool isCommon = !string.IsNullOrEmpty(data.userTrait) && data.userTrait.ToLower() == "common";
                if (isCommon || player.trait == data.userTrait)
                {
                    if (!player.skills.Contains(converted.id))
                    {
                        player.skills.Add(converted.id);
                        Debug.Log($"[워프 스킬 추가] player:{player.id}, skill:{converted.id}, name:{converted.name}");
                    }
                }
            }
        }
    }
}