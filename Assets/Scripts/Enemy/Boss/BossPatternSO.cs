using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "BossPatternData", menuName = "ScriptableObject/BossPatternData")]
public class BossPatternSO : ScriptableObject
{
    public List<BossPatternData> bossPatternDataList = new List<BossPatternData>();
}
