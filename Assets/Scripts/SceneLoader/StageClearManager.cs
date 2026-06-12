using UnityEngine;

public class StageClearManager : MonoBehaviour
{
    [Header("현재 스테이지 적 수")]
    public int enemyCount;

    [Header("문 오브젝트")]
    public GameObject doorObject;

    private void Start()
    {
        doorObject.SetActive(false);

        enemyCount =
            FindObjectsByType<EnemySystem>(
                FindObjectsSortMode.None).Length;

        Debug.Log("현재 적 수 : " + enemyCount);
    }

    public void EnemyDead()
    {
        enemyCount--;

        Debug.Log("남은 적 수 : " + enemyCount);

        if (enemyCount <= 0)
        {
            Debug.Log("스테이지 클리어");

            doorObject.SetActive(true);
        }
    }
}