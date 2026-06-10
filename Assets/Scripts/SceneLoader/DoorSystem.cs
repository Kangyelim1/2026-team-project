using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorSystem : MonoBehaviour
{
    [Header("이동할 다음 씬 이름")]
    public string nextSceneName;

    [Header("상호작용 UI")]
    public GameObject interactionText; 

    private bool canInteract; 

    private void Start()
    {
        // 시작 시 텍스트 숨김
        if (interactionText != null)
            interactionText.SetActive(false);
    }

    private void Update()
    {
        // 문 근처에서 F 누르면 씬 이동
        if (canInteract && Input.GetKeyDown(KeyCode.F))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canInteract = true;

            if (interactionText != null)
                interactionText.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canInteract = false;

            if (interactionText != null)
                interactionText.SetActive(false);
        }
    }
}