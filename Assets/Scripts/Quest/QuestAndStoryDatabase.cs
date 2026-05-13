using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestAndStoryDatabase : MonoBehaviour
{
    [Header("활당된 퀘스트 데이터SO 리스트")]
    public List<QuestDataSO> questDataSOs = new List<QuestDataSO>();
    [Header("활당된 스토리 데이터SO 리스트")]
    public List<StoryDataSO> storyDataSOs = new List<StoryDataSO>();
}
