using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonDatabase", menuName = "DataBase/DungeonDatabase")]
public class DungeonDataSO : ScriptableObject
{
    public List<DungeonData> Dungeons = new List<DungeonData>();
}
