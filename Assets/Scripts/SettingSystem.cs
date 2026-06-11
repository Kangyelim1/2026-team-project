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
        isMute = !isMute;

        AudioListener.volume = isMute ? 0f : 1f;

        soundButtonImage.sprite =
            isMute ? soundOffSprite : soundOnSprite;

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
}