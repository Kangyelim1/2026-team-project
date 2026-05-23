using UnityEngine;
using System.Collections;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    public Interactable currentInteractable;
    public EnemyInteractable currentEnemy;
    public QuestSystem _questSystem;
    public GameObject interactionUI;
    public CanvasGroup interactionCanvasGroup;
    public string QuestTargetEnemy;
    private Coroutine fadeCoroutine;

    public GameObject FunctionUI;
    public TextMeshProUGUI FunctionText;

    void Start()
    {
        
        interactionUI.SetActive(false);
    }

    void Update()
    {
        if (currentInteractable != null && Input.GetKeyDown(KeyCode.F))
        {
            currentInteractable.Interact();
        }

        if(Input.GetMouseButtonDown(0) && currentEnemy != null && !_questSystem.storySystem.isStory)
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

            interactionUI.SetActive(true);
            interactionCanvasGroup.alpha = 1f;
            if(fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            fadeCoroutine =
                StartCoroutine(FadeInteractionUI());
        }

        EnemyInteractable enemy = collision.GetComponent<EnemyInteractable>();
        if (enemy != null)
        {
            currentEnemy = enemy;
            if (_questSystem != null)
            {
                _questSystem.currentEnemy = enemy.enemyName;
            }
            else
            {
                Debug.LogWarning("라이프 타임 한 바뀌 돌고 가져올꺼임");
            }
            
            Debug.Log($"[PlayerInteraction] 공격 가능한 적 감지: {enemy.enemyName}");
            if (!_questSystem.storySystem.isStory)
            {
                FunctionText.text = $"{enemy.enemyName}과 싸우기 [마우스 좌클릭]";
                FunctionUI.gameObject.SetActive(true);
            }
            else
            {
                FunctionUI.gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Interactable interactable = collision.GetComponent<Interactable>();
      
        if (interactable != null && currentInteractable == interactable)
        {
            currentInteractable = null;
            Debug.Log("상호작용 대상 벗어남");
            interactionUI.SetActive(false);

        }

        EnemyInteractable enemy = collision.GetComponent<EnemyInteractable>();
        if (enemy != null && currentEnemy == enemy)
        {
            currentEnemy = null;
            Debug.Log("[PlayerInteraction] 적 범위 이탈");
        }
        FunctionUI.gameObject.SetActive(false);
    }
    IEnumerator FadeInteractionUI()
    {
        // 1초 대기
        yield return new WaitForSeconds(1f);

        // 천천히 사라지기
        float time = 0f;

        while (time < 1f)
        {
            time += Time.deltaTime;

            interactionCanvasGroup.alpha =
                Mathf.Lerp(1f, 0f, time);

            yield return null;
        }
    }
}
