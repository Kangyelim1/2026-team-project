using UnityEngine;

public enum EnemyType
{
    Shortdistance,
    LongDistanc,
    Boom
}

[CreateAssetMenu(fileName = "EnemySO", menuName = "ScriptableObject/Enemy")]
public class EnemySO : ScriptableObject
{
    public string enemyName;
    public  EnemyType enemyType;
}
