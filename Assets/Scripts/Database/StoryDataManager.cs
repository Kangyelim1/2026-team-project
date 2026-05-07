using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public class StoryDataManager : MonoBehaviour
{
    [Header("-------------------------------------------------------------------------------")]
    [Header("스토리 전용 데이터 매니져")]

    [Header("Json 파일 이름을 StoryData 변경하고 반드시 StreamingAssets 파일에 넣어 주세요.")]

    [Header("-------------------------------------------------------------------------------")]

    public StoryDataSO storyDataSO;

    private void Start()
    {
        ConvertJsonToSO();
    }

    private void ConvertJsonToSO()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "StoryData.json");

        if (File.Exists(path))
        {
            string jesonText = File.ReadAllText(path);

            List<NewStoryData> importedStorys = JsonConvert.DeserializeObject<List<NewStoryData>>(jesonText);

            if (storyDataSO != null)
            {
               storyDataSO.storys = importedStorys;

# if UNITY_EDITOR
                EditorUtility.SetDirty(storyDataSO);
                AssetDatabase.SaveAssets();
#endif
                Debug.Log("StoyrData JSON 변환 성공");
            }
            else
                Debug.Log("StorySO 미연결");
        }
    }
}
