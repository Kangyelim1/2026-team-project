using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private Interactable currentInteractable;

    private EnemyInteractable currentEnemy;

    void Update()
    {
        if (currentInteractable != null && Input.GetKeyDown(KeyCode.F))
        {
            currentInteractable.Interact();
        }

        if(Input.GetMouseButtonDown(0) && currentEnemy != null)
        {
            currentEnemy.OnAttacked(transform.position);
        }
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
