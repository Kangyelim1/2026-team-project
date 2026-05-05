using UnityEngine;

[CreateAssetMenu(fileName = "Player", menuName = "Database/Player")]
public class PlayerSO : ScriptableObject
{
    public string playerName;
    public int playerMaxHelth;
    public int playerDamage;
    public int playerSpeed;
}
