using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Boss02System : MonoBehaviour
{
    public EnemySystem enemySystem;
    public EnemyHelthSystem enemyHelthSystem;
    public Boss02PatternData bossPatternSystem;
    public BossPatternSO bossPatternData;

    [Header("현실과 꿈 전환 관련")]
    public int patternCount;
    public int changeFageCount = 5;
    public bool isDreamFage = false;

    [Header("현실 관련 오브젝트")]
    public GameObject RealityBackGround;
    public GameObject RealityGround;

    [Header("꿈 관련 오브젝트")]
    public GameObject DreamBackGround;
    public GameObject DreamGround;

    [Header("비디오")]
    public GameObject videoImage;
    public VideoPlayer videoPlayer;
    public VideoClip OpningVideoClip;


    private void Start()
    {
        enemySystem = FindAnyObjectByType<EnemySystem>();
        enemyHelthSystem = FindAnyObjectByType<EnemyHelthSystem>();
        bossPatternSystem = FindAnyObjectByType<Boss02PatternData>();

        StartCoroutine(Opening());
        SetFage(true); 
    }
    IEnumerator Opening()
    {
        videoImage.SetActive(true);
        Debug.Log("코루틴 실행");
        videoPlayer.clip = OpningVideoClip;
        yield return new WaitForSeconds(0.1f);
        videoPlayer.time = 0;
        videoPlayer.Play();

        while (videoPlayer.isPlaying)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);
        videoPlayer.Stop();
        videoImage.SetActive(false);

        yield return new WaitForSeconds(1.5f);
        
        BossRandomPattern();
    }

    public void BossRandomPattern()
    {
        if (enemySystem.playerSystem == null) return;
        Debug.Log("보스 패턴 뽑기");

        int randomIndex = Random.Range(0, bossPatternData.bossPatternDataList.Count);
        BossPatternData currentPattern = bossPatternData.bossPatternDataList[randomIndex];

        SelectSkill(enemySystem.enemyName, currentPattern.BossPatternName, currentPattern.currentPage);
        AddPatternCount();
    }

    void SelectSkill(string BossName, string Pattern, PatternPage page)
    {
        switch (BossName, Pattern, page)
        {
            case ("페이지", "페이지 패턴01", PatternPage.EveryPage):
                Debug.Log("패이지 패턴01");
                break;
            case ("페이지", "페이지 패턴02", PatternPage.EveryPage):
                Debug.Log("패이지 패턴02");
                break;
            default:
                BossRandomPattern();
                break;
        }
    }

    public void AddPatternCount()
    {
        patternCount++;
        Debug.Log("현재 패턴 카운트 : " + patternCount);

        if (patternCount >= changeFageCount)
        {
            ChangeFage();
        }
    }

    public void ChangeFage()
    {
        patternCount = 0;

        isDreamFage = !isDreamFage;

        SetFage(isDreamFage);

        if (isDreamFage)
        {
            Debug.Log("꿈 페이지로 전환");
        }
        else
        {
            Debug.Log("현실 페이지로 전환");
        }
    }

    public void SetFage(bool dream)
    {
        RealityBackGround.SetActive(!dream);
        RealityGround.SetActive(!dream);

        DreamBackGround.SetActive(dream);
        DreamGround.SetActive(dream);
    }
}
