using UnityEngine;

public enum EnemyType
{
    Shortdistance,
    LongDistanc,
    Boom,
    Boss
}

[CreateAssetMenu(fileName = "EnemySO", menuName = "ScriptableObject/Enemy")]
public class EnemySO : ScriptableObject
{
    public string enemyName;
    public  EnemyType enemyType;
}
