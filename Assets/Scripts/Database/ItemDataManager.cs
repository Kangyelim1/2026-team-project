using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public class ItemDataManager : MonoBehaviour
{
    [Header("-------------------------------------------------------------------------------")]
    [Header("아이템 전용 데이터 매니져")]

    [Header("Json 파일 이름을 ItemData 변경하고 반드시 StreamingAssets 파일에 넣어 주세요.")]

    [Header("-------------------------------------------------------------------------------")]

    public ItemDataSO itemDataSO;

    private void Start()
    {
        ConvertJsonToSO();
    }

    private void ConvertJsonToSO()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "ItemData.json");

        if (File.Exists(path))
        {
            string jesonText = File.ReadAllText(path);

            List<ItemData> importedItem = JsonConvert.DeserializeObject<List<ItemData>>(jesonText);

            if (itemDataSO != null)
            {
                itemDataSO.items = importedItem;

# if UNITY_EDITOR
                EditorUtility.SetDirty(itemDataSO);
                AssetDatabase.SaveAssets();
#endif
                Debug.Log("ItemData JSON 변환 성공");
            }
            else
                Debug.Log("ItemSO 미연결");
        }
    }
}
