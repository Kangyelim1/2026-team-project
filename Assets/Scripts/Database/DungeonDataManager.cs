using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using UnityEditor;
public class DungeonDataManager : MonoBehaviour
{
    [Header("-------------------------------------------------------------------------------")]
    [Header("던전 전용 데이터 매니져")]
    [Header("Json 파일 이름을 DungeonData 변경하고 반드시 StreamingAssets 파일에 넣어 주세요.")]

    [Header("-------------------------------------------------------------------------------")]
    public DungeonDataSO dungeonSO;

    void Start()
    {
        ConvertJsonToSO();
    }

   void ConvertJsonToSO()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "DungeonData.Json");

        if (File.Exists(path))
        {
            string jsonText = File.ReadAllText(path);

            List<DungeonData> importedDungeon = JsonConvert.DeserializeObject<List<DungeonData>>(jsonText);

            if(dungeonSO != null)
            {
                dungeonSO.Dungeons = importedDungeon;
#if UNITY_EDITOR
                EditorUtility.SetDirty(dungeonSO);
                AssetDatabase.SaveAssets();
#endif
                Debug.Log("DungeonData JSON 변환 성공");
            }
            else
                Debug.Log("DungeonSO 미연결");
        }
    }


}
