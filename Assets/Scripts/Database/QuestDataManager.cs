using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class QuestDataManager : MonoBehaviour
{
    public QuestDataSO questDataSO;

    private void Start()
    {
        ConvertJsonToSO();
    }
    public void ConvertJsonToSO()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "QuestData.json");

        if (File.Exists(path))
        {
            string jsonText = File.ReadAllText(path);

            List<QuestData> importedQuests = JsonConvert.DeserializeObject<List<QuestData>>(jsonText);

            if (questDataSO)
            {
                questDataSO.quests = importedQuests;
# if UNITY_EDITOR
                EditorUtility.SetDirty(questDataSO);
                AssetDatabase.SaveAssets();
#endif
                Debug.Log("QuseData JSON변환 성공");
            }
            else
                Debug.Log("QuestSO 미연결");

        }
    }
}
