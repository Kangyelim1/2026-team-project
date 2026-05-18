using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private Interactable currentInteractable;

    private EnemyInteractable currentEnemy;
    public QuestSystem _questSystem;


    void Update()
    {
        if (currentInteractable != null && Input.GetKeyDown(KeyCode.F))
        {
            currentInteractable.Interact();
        }

        if(Input.GetMouseButtonDown(0) && currentEnemy != null)
        {
            _questSystem.currentEnemy = currentEnemy.enemyName;
            currentEnemy.OnAttacked(transform.position);
        }

        if(_questSystem == null)
            _questSystem = Object.FindAnyObjectByType<QuestSystem>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Interactable interactable = collision.GetComponent<Interactable>();

        if (interactable != null)
        {
            currentInteractable = interactable;
            Debug.Log("상호작용 가능 대상 진입");
        }

        EnemyInteractable enemy = collision.GetComponent<EnemyInteractable>();
        if (enemy != null)
        {
            currentEnemy = enemy;
            _questSystem.currentEnemy = enemy.enemyName;
            Debug.Log($"[PlayerInteraction] 공격 가능한 적 감지: {enemy.enemyName}");
          
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Interactable interactable = collision.GetComponent<Interactable>();
      
        if (interactable != null && currentInteractable == interactable)
        {
            currentInteractable = null;
            Debug.Log("상호작용 대상 벗어남");
        }

        EnemyInteractable enemy = collision.GetComponent<EnemyInteractable>();
        if (enemy != null && currentEnemy == enemy)
        {
            currentEnemy = null;
            Debug.Log("[PlayerInteraction] 적 범위 이탈");
        }
    }
}
