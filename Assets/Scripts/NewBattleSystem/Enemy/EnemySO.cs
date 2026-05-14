using UnityEngine;
using UnityEngine.UIElements.Experimental;

public enum EnemyAttackType
{
    close_range,
    long_distance
}

[CreateAssetMenu(fileName = "Enemy", menuName = "BattleCharacter/Enemy")]
public class EnemySO : ScriptableObject
{
    public string EnemyName;
    public int EnemyLevel;
    public int EnemyMaxHelth;
    public int EnemyDamage;
    public EnemyAttackType enemyAttackType;
    public int EnemySpeed;
}
