using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RadioMission : MonoBehaviour
{
    [Header("UI")]
    public GameObject missionCanvas;

    public RectTransform slider;

    public RectTransform targetPoint;

    [Header("슬라이더 이동 범위")]
    public float minX = -250f;
    public float maxX = 250f;

    [Header("씬 이동")]
    public string nextSceneName;

    [Header("판정")]
    public float successRange = 20f;

    private bool isMission;

    public void StartMission()
    {
        isMission = true;

        missionCanvas.SetActive(true);

        // 목표 위치 랜덤 생성
        float randomX = Random.Range(minX, maxX);

        targetPoint.anchoredPosition =
            new Vector2(
                randomX,
                targetPoint.anchoredPosition.y
            );
    }

    private void Update()
    {
        if (!isMission)
            return;

        // 마우스로 슬라이더 이동
        MoveSliderWithMouse();

        // 마우스 클릭으로 판정
        if (Input.GetMouseButtonDown(0))
        {
            CheckMission();
        }
    }

    void MoveSliderWithMouse()
    {
        // 화면 마우스 좌표 가져오기
        Vector2 mousePos = Input.mousePosition;

        // Canvas 기준 좌표로 변환
        RectTransform canvasRect =
            missionCanvas.GetComponent<RectTransform>();

        Vector2 localPoint;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            mousePos,
            null,
            out localPoint
        );

        // X축만 사용
        float x = Mathf.Clamp(localPoint.x, minX, maxX);

        slider.anchoredPosition =
            new Vector2(
                x,
                slider.anchoredPosition.y
            );
    }

    void CheckMission()
    {
        float distance =
            Mathf.Abs(
                slider.anchoredPosition.x -
                targetPoint.anchoredPosition.x
            );

        if (distance <= successRange)
        {
            MissionSuccess();
        }
        else
        {
            Debug.Log("주파수 불일치");
        }
    }

    void MissionSuccess()
    {
        Debug.Log("주파수 연결 성공");

        SceneManager.LoadScene(nextSceneName);
    }
}