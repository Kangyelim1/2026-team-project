using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RadioMission : MonoBehaviour
{
    [Header("UI")]
    public GameObject missionCanvas;

    public RectTransform slider;

    public RectTransform targetPoint;

    [Header("씬 이동")]
    public string nextSceneName;

    [Header("판정")]
    public float successRange = 20f;

    private bool isMission;

    public void StartMission()
    {
        isMission = true;

        missionCanvas.SetActive(true);

        // 목표 위치 랜덤
        float randomX = Random.Range(-250f, 250f);

        targetPoint.anchoredPosition =
            new Vector2(randomX,
            targetPoint.anchoredPosition.y);
    }

    private void Update()
    {
        if (!isMission)
            return;

        MoveSlider();

        if (Input.GetKeyDown(KeyCode.E))
        {
            CheckMission();
        }
    }

    void MoveSlider()
    {
        float move =
            Input.GetAxisRaw("Horizontal") * 300f * Time.deltaTime;

        slider.anchoredPosition += new Vector2(move, 0f);
    }

    void CheckMission()
    {
        float distance =
            Mathf.Abs(
                slider.anchoredPosition.x
                - targetPoint.anchoredPosition.x);

        if (distance <= successRange)
        {
            MissionSuccess();
        }
    }

    void MissionSuccess()
    {
        Debug.Log("주파수 연결 성공");

        SceneManager.LoadScene(nextSceneName);
    }
}