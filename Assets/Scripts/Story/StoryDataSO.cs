using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StoryDataBase", menuName = "DataBase/StoryDatabase")]
public class StoryDataSO : ScriptableObject
{
   public List<NewStoryData> storys = new List<NewStoryData>();
}
