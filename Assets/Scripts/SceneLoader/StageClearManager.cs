using UnityEngine;

public class StageClearManager : MonoBehaviour
{
    [Header("현재 스테이지 적 수")]
    public int enemyCount;

    [Header("문 오브젝트")]
    public GameObject doorObject;

    private void Start()
    {

        // 시작할 때 문 비활성화
        doorObject.SetActive(false);


        // 현재 씬의 모든 EnemySystem 찾기
        enemyCount = FindObjectsByType<EnemySystem>(FindObjectsSortMode.None).Length;

        Debug.Log("현재 적 수 : " + enemyCount);
    }


    // 적 죽을 때 호출될 함수
    public void EnemyDead()
    {
        enemyCount--;

        Debug.Log("남은 적 수 : " + enemyCount);

        // 적 전부 죽으면 문 활성화
        if (enemyCount <= 0)
        {
            Debug.Log("스테이지 클리어");

            // 타이머 정지
            StageTimer stageTimer = FindAnyObjectByType<StageTimer>();

            if (stageTimer != null)
            {
                stageTimer.StopTimer();
            }

            // 문 활성화
            doorObject.SetActive(true);
        }
    }
}
