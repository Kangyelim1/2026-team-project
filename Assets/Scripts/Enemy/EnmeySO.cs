using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EnemyDatabase", menuName = "DataBase/EnemyDatabase")]
public class EnmeySO : ScriptableObject
{
    public List<EnemyDataBase> Enemys = new List<EnemyDataBase>();
}
