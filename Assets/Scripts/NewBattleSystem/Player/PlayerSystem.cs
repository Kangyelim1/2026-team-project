using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerSystem : MonoBehaviour
{
    public PlayerSO playerSO; 
    public string player_Name;
    public int player_MaxHelth;
    public int player_Damage;
    public int player_Speed;
    public GameObject playerPrefab;
    public Animator playerAnimator;

    [Header("Helth")]
    public int player_CurrentHelth;
    public Slider playerHelthSlider;
    [SerializeField] private float smoothSpeed = 5f;

    private void Start()
    {
        player_Name = playerSO.playerName;
        player_MaxHelth = playerSO.playerMaxHelth;
        player_Damage = playerSO.playerDamage;
        player_Speed = playerSO.playerSpeed;

        player_CurrentHelth = player_MaxHelth;
    }

    private void Update()
    {
        playerHelthSlider.minValue = 0;
        playerHelthSlider.maxValue = player_MaxHelth;

        HelthUI();
    }

    private void HelthUI()
    {
        playerHelthSlider.value = Mathf.Lerp(player_CurrentHelth, player_CurrentHelth, smoothSpeed * Time.deltaTime);
    }
}
