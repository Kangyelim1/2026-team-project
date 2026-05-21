using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class QuestDataManager : MonoBehaviour
{
    [Header("-------------------------------------------------------------------------------")]
    [Header("퀘스트 전용 데이터 매니져")]
    [Header("Json 파일 이름을 QuestData 변경하고 반드시 StreamingAssets 파일에 넣어 주세요.")]

    [Header("-------------------------------------------------------------------------------")]

    public QuestDataSO questDataSO;

    private void Start()
    {
        ConvertJsonToSO();
    }
    public void ConvertJsonToSO()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "QuestData.json");      // 경로 지정및 파일 이름

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
