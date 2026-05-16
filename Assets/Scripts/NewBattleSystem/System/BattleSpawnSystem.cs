using System.Collections.Generic;
using UnityEngine;

public class BattleSpawnSystem : MonoBehaviour
{
    public List<EnemySystem> Enemys = new List<EnemySystem>();
    public List<PlayerSystem> Players = new List<PlayerSystem>();

    public QuestSystem _questSystem;
    public NewBattleManager _battleManager;


    private void Start()
    {
        _questSystem = Object.FindAnyObjectByType<QuestSystem>();
        _battleManager = Object.FindAnyObjectByType<NewBattleManager>();

        FIndEnemy();
    }

    public void FIndEnemy()
    {
        if (_questSystem == null) return;

        EnemySystem currentEnemy = Enemys.Find(ce => ce.Enemy_Name == _questSystem.currentEnemy);

        Debug.Log(currentEnemy);

        if (currentEnemy != null)
        {
            _battleManager.enemyData = currentEnemy;
            _battleManager.SpawnEnemy();
            Debug.Log($"{currentEnemy.Enemy_Name} 소환 완료");
        }
        else
            Debug.Log($"{currentEnemy} 가 존재하지 않음");
    }

}
