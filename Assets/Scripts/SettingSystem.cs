using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // 추가

public class SettingSystem : MonoBehaviour
{
    [Header("사운드")]
    public GameSoundManager gameSoundManager;

    [Header("사운드 버튼")]
    public Image soundButtonImage;

    public Sprite soundOnSprite;
    public Sprite soundOffSprite;

    private bool isMute = false;

    [Header("해상도")]
    public TMP_Dropdown resolutionDropdown;

    [Header("볼륨 슬라이더")]
    public Slider volumeSlider;

    private float lastVolume = 1f;

    private bool isChangingByMuteButton = false;

    Resolution[] resolutions;

    private void Start()
    {
        
        LoadSoundSetting();

        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        var options = new System.Collections.Generic.List<string>();

        int currentIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option =
                resolutions[i].width +
                " x " +
                resolutions[i].height;

            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);

        resolutionDropdown.value = currentIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int index)
    {
        Resolution resolution = resolutions[index];

        Screen.SetResolution(
            resolution.width,
            resolution.height,
            FullScreenMode.Windowed);
    }

    public void ToggleSound()
    {
        Debug.Log("음소거 버튼 클릭");

        isChangingByMuteButton = true;

        isMute = !isMute;

        soundButtonImage.sprite =
            isMute ? soundOffSprite : soundOnSprite;

        if (isMute)
        {
            lastVolume = volumeSlider.value;

            volumeSlider.value = 0f;

            AudioListener.volume = 0f;
        }
        else
        {
            volumeSlider.value = lastVolume;

            AudioListener.volume = lastVolume;
        }

        PlayerPrefs.SetInt("Mute", isMute ? 1 : 0);
    }

    
    private void LoadSoundSetting()
    {
        isMute = PlayerPrefs.GetInt("Mute", 0) == 1;

        AudioListener.volume = isMute ? 0f : 1f;

        soundButtonImage.sprite =
            isMute ? soundOffSprite : soundOnSprite;
    }

    public void GoTutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void SetVolume(float value)
    {
        gameSoundManager.SetMasterVolume(value);

        if (isChangingByMuteButton)
            return;

        // 음소거 상태였다면 해제
        if (value > 0)
        {
            isMute = false;

            soundButtonImage.sprite = soundOnSprite;

            PlayerPrefs.SetInt("Mute", 0);
        }
    }
}