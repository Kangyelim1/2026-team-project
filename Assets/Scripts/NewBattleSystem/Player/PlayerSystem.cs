using UnityEngine;

public class PlayerSystem : MonoBehaviour
{
    public PlayerSO playerSO; 
    public string player_Name;
    public int player_MaxHelth;
    public int player_CurrentHelth;
    public int player_Damage;
    public int player_Speed;
    public GameObject playerPrefab;
    public Animator playerAnimator;

    private void Start()
    {
        player_Name = playerSO.playerName;
        player_MaxHelth = playerSO.playerMaxHelth;
        player_Damage = playerSO.playerDamage;
        player_Speed = playerSO.playerSpeed;

        player_CurrentHelth = player_MaxHelth;
    }
}
