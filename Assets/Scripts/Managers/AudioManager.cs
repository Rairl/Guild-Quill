using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Components")]
    public AudioSource bgmSource;

    [Header("BGM Tracks")]
    public AudioClip bgm1;
    public AudioClip bgm2;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Start with BGM1 at 40% volume
        SetBGMVolume(0.2f);
        PlayBGM1();
    }

    // Plays the first BGM
    public void PlayBGM1()
    {
        PlayBGM(bgm1);
    }

    // Plays the second BGM
    public void PlayBGM2()
    {
        PlayBGM(bgm2);
    }

    // Internal BGM playback method
    private void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;

        if (bgmSource.clip == clip && bgmSource.isPlaying)
            return;

        bgmSource.Stop();
        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    // Stop the current BGM
    public void StopBGM()
    {
        bgmSource.Stop();
    }

    // Set volume (0.0 to 1.0)
    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = Mathf.Clamp01(volume);
    }
}
