using UnityEngine;
using UnityEngine.UIElements.Experimental;

[CreateAssetMenu(fileName = "Enemy", menuName = "Database/Enemy")]
public class EnemySO : ScriptableObject
{
    public string EnemyName;
    public int EnemyLevel;
    public int EnemyMaxHelth;
    public int EnemyDamage;
    public int EnemySpeed;
}
