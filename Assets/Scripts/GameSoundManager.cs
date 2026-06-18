using UnityEngine;

public class GameSoundManager : MonoBehaviour
{
    public static GameSoundManager Instance;

    [Header("플레이어 사운드")]
    public AudioClip playerShootSound;
    public AudioClip playerRapidShootSound;
    public AudioClip playerDashSound;
    public AudioClip playerJumpSound;
    public AudioClip playerDoubleJumpSound;
    public AudioClip playerDeathSound;
    public AudioClip playerLandSound;
    public AudioClip playerParrySound;      // 패리

    [Header("적 사운드")]
    public AudioClip enemyHitSound;
    public AudioClip enemyDeathSound;
    public AudioClip enemyShootSound;
    public AudioClip enemyChargeSound;
    public AudioClip explodeSound;

    [Header("보스 사운드")]
    public AudioClip bossAppearSound;
    public AudioClip bossHitSound;
    public AudioClip bossDeathSound;

    [Header("보스 패턴 사운드")]
    public AudioClip aimPatternSound;  
    public AudioClip laserPatternSound; 
    public AudioClip laserStartSound;  
    public AudioClip objectDestroySound; 
    public AudioClip missileLaunchSound;
    public AudioClip targetMarkSound;

    [Header("환경/UI 사운드")]
    public AudioClip boxBreakSound;
    public AudioClip itemPickupSound;
    public AudioClip stageClearSound;
    public AudioClip gameOverSound;

    [Header("BGM")]
    public AudioClip menuBGM;
    public AudioClip stage01BGM;
    public AudioClip stage02BGM;
    public AudioClip stageBGM;
    public AudioClip bossBGM;

    private AudioSource sfxSource;
    private AudioSource bgmSource;

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

        AudioSource[] sources = GetComponents<AudioSource>();
        sfxSource = sources.Length > 0 ? sources[0] : gameObject.AddComponent<AudioSource>();
        bgmSource = sources.Length > 1 ? sources[1] : gameObject.AddComponent<AudioSource>();

        bgmSource.loop = true;
        bgmSource.volume = 0.5f;
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null || bgmSource.clip == clip) return;
        bgmSource.Stop();
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void SetMasterVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    public void OnFindPlayerSound(string soundName)
    {
        switch (soundName)
        {
            case "플레이어 기본공격":
                PlaySFX(playerShootSound);
                break;
            case "플레이어 연사":
                PlaySFX(playerRapidShootSound);
                break;
            case "플레이어 대시":
                PlaySFX(playerDashSound);
                break;
            case "플레이어 점프":
                PlaySFX(playerJumpSound);
                break;
            case "플레이어 더블점프":
                PlaySFX(playerDoubleJumpSound);
                break;
            case "플레이어 사망":
                PlaySFX(playerDeathSound);
                break;
            case "패리":
                PlaySFX(playerParrySound);
                break;
            default:
                Debug.Log("등록되지 않은 플레이어 사운드: " + soundName);
                break;
        }
    }

    public void OnFindEnemySound(string soundName)
    {
        switch (soundName)
        {
            case "조준 패턴":
                PlaySFX(aimPatternSound);
                break;
            case "레이저 패턴":
                PlaySFX(laserPatternSound);
                break;
            case "레이저 시작 사운드":
                PlaySFX(laserStartSound);
                break;
            case "오브젝트 파괴 소리":
                PlaySFX(objectDestroySound);
                break;
            case "미사일 발사":
                PlaySFX(missileLaunchSound);
                break;
            case "타겟 지정":
                PlaySFX(targetMarkSound);
                break;
            default:
                Debug.Log("등록되지 않은 보스 사운드: " + soundName);
                break;
        }
    }
}