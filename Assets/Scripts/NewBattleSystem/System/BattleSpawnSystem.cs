using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    }

    public void FIndEnemy()
    {
        if (_questSystem == null) return;

        EnemySystem currentEnemy = Enemys.Find(ce => ce.Enemy_Name == _questSystem.currentQuestEnemyNPC);

        if (currentEnemy == null) return;
        _battleManager.enemyData = currentEnemy;
        Debug.Log($"{currentEnemy.Enemy_Name} 소환 완료");
    }

}
