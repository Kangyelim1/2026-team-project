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
    public Sprite skill01;
    public Sprite skill02;
    public Sprite skill03;
    public Sprite skill04;

    [Header("Helth")]
    public int player_CurrentHelth;
    public Slider playerHelthSlider;
    [SerializeField] private float smoothSpeed = 5f;

    private void Awake()
    {
        player_Name = playerSO.playerName;
        player_MaxHelth = playerSO.playerMaxHelth;
        player_Damage = playerSO.playerDamage;
        player_Speed = playerSO.playerSpeed;
        skill01 = playerSO.Skill01Image;
        skill02 = playerSO.Skill02Image;
        skill03 = playerSO.Skill03Image;
        skill04 = playerSO.Skill04Image;

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
