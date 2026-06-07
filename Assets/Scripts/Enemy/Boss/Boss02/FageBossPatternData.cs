using Unity.Cinemachine;
using UnityEngine;

public class FageBossPatternData : MonoBehaviour
{
    public BossSystem bossSystem;
    public EnemyHelthSystem enemyHelthSystem;
    public EnemySystem enemySystem;

    public PlayerHelthSystem playerHelthSystem;
    public PlayerSystem playerSystem;

    public CinemachineCamera playerCamera;

    public float BossPatternTime;
}
