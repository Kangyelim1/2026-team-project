using System.Collections.Generic;
using UnityEngine;

public class GameSoundManager : MonoBehaviour
{
    public AudioSource playerSound;
    public AudioSource enemySound;

    public List<AudioClip> playerSoundList = new List<AudioClip>();
    public List<AudioClip> enemySoundList = new List<AudioClip>();

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
}

