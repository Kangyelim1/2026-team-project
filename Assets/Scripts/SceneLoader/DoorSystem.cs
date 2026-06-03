using UnityEngine;

public class DoorSystem : MonoBehaviour
{
    [Header("주파수 미션")]
    public RadioMission radioMission;

    private bool canInteract;
    public GameObject interactionText;

    private void Update()
    {
        // 플레이어가 문 근처에 있고 F를 눌렀을 때
        if (canInteract && Input.GetKeyDown(KeyCode.F))
        {
            radioMission.StartMission();
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