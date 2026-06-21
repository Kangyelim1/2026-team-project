using UnityEngine;

public class GameSoundManager : MonoBehaviour
{
    public static GameSoundManager Instance;

    [Header("플레이어 사운드")]
    public AudioClip playerShootSound;       // 공격
    public AudioClip playerWalkSound;        // 걷기
    public AudioClip playerJumpSound;        // 점프
    public AudioClip playerParrySound;       // 패링
    public AudioClip playerReloadSound;      // 재장전
    public AudioClip playerMeleeSound;       // 근접공격

    [Header("적 사운드")]
    public AudioClip enemyWalkSound;         // 걷기

    [Header("보스 사운드")]
    public AudioClip bossAppearSound;        // 보스 등장
    public AudioClip bossHitSound;           // 보스 피격
    public AudioClip bossDeathSound;         // 보스 사망

    [Header("보스 패턴 사운드")]
    public AudioClip aimPatternSound;        // 조준 패턴
    public AudioClip laserPatternSound;      // 레이저 패턴
    public AudioClip laserStartSound;        // 레이저 시작
    public AudioClip objectDestroySound;     // 오브젝트 파괴
    public AudioClip missileLaunchSound;     // 미사일 발사
    public AudioClip targetMarkSound;        // 타겟 지정

    [Header("환경/UI 사운드")]
    public AudioClip boxBreakSound;          // 박스 파괴
    public AudioClip doorOpenSound;          // 문 열림
    public AudioClip itemPickupSound;        // 아이템 획득
    public AudioClip uiButtonSound;          // UI 버튼

    [Header("BGM")]
    public AudioClip bgm;

    private AudioSource sfxSource;
    private AudioSource bgmSource;

    private void Awake()
    {
        Instance = this;

        AudioSource[] sources = GetComponents<AudioSource>();
        sfxSource = sources.Length > 0 ? sources[0] : gameObject.AddComponent<AudioSource>();
        bgmSource = sources.Length > 1 ? sources[1] : gameObject.AddComponent<AudioSource>();

        bgmSource.loop = true;
        bgmSource.volume = 0.5f;
    }

    private void Start()
    {
        PlayBGM(bgm);
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;
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
            case "플레이어 공격":
                PlaySFX(playerShootSound);
                break;
            case "플레이어 걷기":
                PlaySFX(playerWalkSound);
                break;
            case "플레이어 점프":
                PlaySFX(playerJumpSound);
                break;
            case "패링":
                PlaySFX(playerParrySound);
                break;
            case "재장전":
                PlaySFX(playerReloadSound);
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
            case "적 걷기":
                PlaySFX(enemyWalkSound);
                break;
            default:
                Debug.Log("등록되지 않은 적 사운드: " + soundName);
                break;
        }
    }

    public void OnFindBossSound(string soundName)
    {
        switch (soundName)
        {
            case "보스 등장":
                PlaySFX(bossAppearSound);
                break;
            case "보스 피격":
                PlaySFX(bossHitSound);
                break;
            case "보스 사망":
                PlaySFX(bossDeathSound);
                break;
            case "조준 패턴":
                PlaySFX(aimPatternSound);
                break;
            case "레이저 패턴":
                PlaySFX(laserPatternSound);
                break;
            case "레이저 시작":
                PlaySFX(laserStartSound);
                break;
            case "오브젝트 파괴":
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

    public void OnFindEnvSound(string soundName)
    {
        switch (soundName)
        {
            case "박스 파괴":
                PlaySFX(boxBreakSound);
                break;
            case "문 열림":
                PlaySFX(doorOpenSound);
                break;
            case "아이템 획득":
                PlaySFX(itemPickupSound);
                break;
            case "UI 버튼":
                PlaySFX(uiButtonSound);
                break;
            default:
                Debug.Log("등록되지 않은 환경/UI 사운드: " + soundName);
                break;
        }
    }
}