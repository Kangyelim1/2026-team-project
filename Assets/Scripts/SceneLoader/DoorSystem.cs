using UnityEngine;

public class DoorSystem : MonoBehaviour
{
    [Header("주파수 미션")]
    public RadioMission radioMission;

    [Header("키패드 미션")]
    public KeypadMission keypadMission;

    private bool canInteract;
    public GameObject interactionText;

    private void Update()
    {
        if (canInteract && Input.GetKeyDown(KeyCode.F))
        {
            // 주파수 미션이 연결되어 있으면 실행
            if (radioMission != null)
            {
                radioMission.StartMission();
            }

            // 키패드 미션이 연결되어 있으면 실행
            if (keypadMission != null)
            {
                keypadMission.StartMission();
            }
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