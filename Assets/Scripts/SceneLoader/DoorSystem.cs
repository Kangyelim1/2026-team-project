using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorSystem : MonoBehaviour
{
    [Header("이동할 다음 씬 이름")]
    public string nextSceneName;

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
