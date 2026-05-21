using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public class SkillDataManager : MonoBehaviour
{
    [Header("-------------------------------------------------------------------------------")]
    [Header("스킬 전용 데이터 매니져")]

    [Header("Json 파일 이름을 SkillData 변경하고 반드시 StreamingAssets 파일에 넣어 주세요.")]

    [Header("-------------------------------------------------------------------------------")]

    public SkillDataSO skillDataSO;

    private void Start()
    {
        ConvertJsonToSO();
    }

    private void ConvertJsonToSO()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "SkillData.json");

        if (File.Exists(path))
        {
            string jesonText = File.ReadAllText(path);

            List<NewSkillData> importedSkills = JsonConvert.DeserializeObject<List<NewSkillData>>(jesonText);

            if (skillDataSO != null)
            {
                skillDataSO.Skills = importedSkills;

# if UNITY_EDITOR
                EditorUtility.SetDirty(skillDataSO);
                AssetDatabase.SaveAssets();
#endif
                Debug.Log("SkillData JSON 변환 성공");
            }
            else
                Debug.Log("SkillSO 미연결");
        }
    }
}
