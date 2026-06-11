using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSoundManager : MonoBehaviour
{
    public AudioSource playerSound;
    public AudioSource enemySound;

    public List<AudioClip> playerSoundList = new List<AudioClip>();
    public List<AudioClip> enemySoundList = new List<AudioClip>();

    private bool isMute = false;



    private void Start()
    {
        float volume = PlayerPrefs.GetFloat("MasterVolume", 1f);

        playerSound.volume = volume;
        enemySound.volume = volume;
    }
    public void OnFindPlayerSound(string soundName)
    {
        AudioClip playerSoundClip = playerSoundList.Find(ps => ps.name == soundName);

        if(playerSoundClip != null)
        {
            PlayAudio(playerSound, playerSoundClip);
        }
    }

    public void OnFindEnemySound(string soundName)
    {
        AudioClip enemySoundClip = enemySoundList.Find(es => es.name == soundName);

        if (enemySoundClip != null)
        {
            PlayAudio(enemySound, enemySoundClip);
        }
    }

    void PlayAudio(AudioSource audio, AudioClip audioClip)
    {
        audio.Stop();
        audio.clip = null;
        audio.time = 0;
        audio.clip = audioClip;
        audio.Play();
    }

    public void SetMasterVolume(float value)
    {
        playerSound.volume = value;
        enemySound.volume = value;

        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    public void ToggleMute()
    {
        isMute = !isMute;

        playerSound.mute = isMute;
        enemySound.mute = isMute;
    }
}

