using UnityEngine;

public enum PlayerAttackType
{
    close_range,
    long_distance
} 

[CreateAssetMenu(fileName = "Player", menuName = "Database/Player")]
public class PlayerSO : ScriptableObject
{
    public string playerName;
    public int playerMaxHelth;
    public int playerDamage;
    public PlayerAttackType playerAttackType;
    public int playerSpeed;
}
