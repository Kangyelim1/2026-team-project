using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManger : MonoBehaviour
{
    public static GameManger Instance;
    public FadeManager fadeManger;

    public bool isDiePlayer;
    private bool isReloading;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        fadeManger = Object.FindAnyObjectByType<FadeManager>();
    }

    private void Start()
    {
        // ★ 씬 시작 시 BGM 자동 적용
        ApplySceneBGM(SceneManager.GetActiveScene().name);
    }

    private void Update()
    {
        if (isDiePlayer && !isReloading)
        {
            StartCoroutine(DiePlayer());
        }
    }

    private IEnumerator DiePlayer()
    {
        isReloading = true;
        isDiePlayer = false;

        if (fadeManger == null)
            fadeManger = Object.FindAnyObjectByType<FadeManager>();

        if (fadeManger != null)
            fadeManger.StartFadeOut(0.5f);

        yield return new WaitForSecondsRealtime(3f);

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);

        yield return new WaitForSecondsRealtime(0.1f);

        fadeManger = Object.FindAnyObjectByType<FadeManager>();

        if (fadeManger != null)
            fadeManger.StartFadeIn(0.5f);

        isReloading = false;
    }

    private void ApplySceneBGM(string sceneName)
    {
        if (GameSoundManager.Instance == null) return;

        switch (sceneName)
        {
            case "MainMenu":
                GameSoundManager.Instance.PlayBGM(GameSoundManager.Instance.menuBGM);
                break;
            case "Stage01":
                GameSoundManager.Instance.PlayBGM(GameSoundManager.Instance.stage01BGM);
                break;
            case "Stage02":
                GameSoundManager.Instance.PlayBGM(GameSoundManager.Instance.stage02BGM);
                break;
            default:
                GameSoundManager.Instance.StopBGM();
                break;
        }
    }
}