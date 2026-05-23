using System.Collections.Generic;
using UnityEngine;

public class AudioSystem : MonoBehaviour
{
    [Header("오디오")]
    public AudioSource musicAudio;
    public AudioSource soundEffectAudio;

    [Header("오디오 클립 리스트")]
    public List<AudioClip> musicAudioClipLIst = new List<AudioClip>();
    public List<AudioClip> soundEffectAudioClipList = new List<AudioClip>();

    public void FindMusicAudioClip(string MusicName)
    {
        if (musicAudio == null) return;

        AudioClip findMusic = musicAudioClipLIst.Find(m => m.name == MusicName);

        if (findMusic != null)
            ChanageAudio(musicAudio, findMusic);
    }

    public void FinSoundEffectAudioClip(string SoundEffectName)
    {
        if (soundEffectAudio == null) return;

        AudioClip findSound = soundEffectAudioClipList.Find(s => s.name == SoundEffectName);

        if (findSound != null)
            ChanageAudio(soundEffectAudio, findSound);
    }

    void ChanageAudio(AudioSource audio, AudioClip sound)
    {
        audio.Stop();
        audio.clip = sound;
        audio.time = 0;
        audio.Play();
    }
}
