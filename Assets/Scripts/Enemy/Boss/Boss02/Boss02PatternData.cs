using Unity.Cinemachine;
using UnityEngine;

public class Boss02PatternData : MonoBehaviour
{
    public Boss02System bossSystem;
    public EnemyHelthSystem enemyHelthSystem;
    public EnemySystem enemySystem;

    public PlayerHelthSystem playerHelthSystem;
    public PlayerSystem playerSystem;

    public CinemachineCamera playerCamera;

    public float BossPatternTime;

    private void Start()
    {
        bossSystem = FindAnyObjectByType<Boss02System>();
        enemyHelthSystem = FindAnyObjectByType<EnemyHelthSystem>();
        enemySystem = FindAnyObjectByType<EnemySystem>();
    }
}
