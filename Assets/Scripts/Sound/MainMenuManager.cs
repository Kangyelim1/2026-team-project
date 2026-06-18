using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    private void Start()
    {
        GameSoundManager.Instance?.PlayBGM(GameSoundManager.Instance.menuBGM);
    }
}