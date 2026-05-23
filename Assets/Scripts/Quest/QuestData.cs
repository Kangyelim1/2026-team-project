using System;
using UnityEngine;

[Serializable]
public class QuestData
{
    public int Quest_ID;
    public string Quest_Title;
    public string Quest_Description;
    public string Quest_Type;
    public int Quest_StoryID;
    public string Quest_EnemyNPC;
    public string Location_Asset;
    public string BGM_Asset;
    public int Qeust_EnemyCount;
    public string Rewards;
    public string Destination;
    public Sprite rewardIcon1;
    public Sprite rewardIcon2;
}
