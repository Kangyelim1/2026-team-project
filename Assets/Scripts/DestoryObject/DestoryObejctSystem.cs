using UnityEngine;

public class DestoryObejctSystem : MonoBehaviour
{
    public GameSoundManager gameSoundManager;

    private void Start()
    {
        gameSoundManager = FindAnyObjectByType<GameSoundManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            gameSoundManager.OnFindEnemySound("오브젝트 추락");
            Destroy(gameObject, 1);
        }
    }
}
