using UnityEngine;
using UnityEngine.UI;

public class EnemySystem : MonoBehaviour
{
    public EnemySO enemySO;

    public string Enmey_Name;
    public int Enemy_Levle;
    public int Enemy_MaxHelth;
    public int Enemy_CurrentHelth;
    public int Enemy_Damage;
    public int Enemy_Speed;
    public GameObject EnemyPrefab;

    public Button ThisEnemyButton;
    public PlayerBattleSystem _playerBattleSystem;

    private void Start()
    {
        Enmey_Name = enemySO.EnemyName;
        Enemy_Levle = enemySO.EnemyLevel;
        Enemy_MaxHelth = enemySO.EnemyMaxHelth;
        Enemy_Damage = enemySO.EnemyDamage;
        Enemy_Speed = enemySO.EnemySpeed;

        Enemy_CurrentHelth = Enemy_MaxHelth;

        _playerBattleSystem = Object.FindAnyObjectByType<PlayerBattleSystem>();
    }
}
