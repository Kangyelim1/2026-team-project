using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillDataBase", menuName = "DataBase/SkillDataBase")]
public class SkillDataSO : ScriptableObject
{
    public List<NewSkillData> Skills = new List<NewSkillData>();
}
