/*****************************************************************************
// File Name :         MusicHandler.cs
// Author :            TJ Caron
// Creation Date :     03/24/2021
//
// Brief Description : Handles playing different music at different points in 
                       the game.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicHandler : MonoBehaviour
{
    /// <summary>
    /// The audio source that plays background music
    /// </summary>
    private AudioSource musicSource;

    /// <summary>
    /// Volume music should play at
    /// </summary>
    private float desiredVolume;

    [Tooltip("Amount of time it takes for volume to fade to desired volume")]
    public float volumeFadeTime = 1f;

    [Tooltip("Music to play on the loading screen")]
    public AudioClip loadingScreenMusic;

    [Tooltip("Music to play on the main menu")]
    public AudioClip menuMusic;

    [Tooltip("Music to play on the ending screens")]
    public AudioClip endingMusic;
    // Start is called before the first frame update
    void Start()
    {
        musicSource = GetComponent<AudioSource>();
        desiredVolume = musicSource.volume;
    }

    private IEnumerator FadeOutMusic(AudioClip newMusic)
    {
        //musicSource.volume = 0;
        Debug.Log("Starting music fade");
        float t = 0;

        while (t < volumeFadeTime)
        {
            t += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(desiredVolume, 0, t / volumeFadeTime);
            yield return null;
        }

        //for (float t = 0; t < volumeFadeTime; t += Time.deltaTime)
        //{          
        //    float normalized = t / volumeFadeTime;
        //    musicSource.volume = normalized * desiredVolume;
        //    musicSource.volume = Mathf.Clamp(musicSource.volume, 0, desiredVolume);
        //    yield return null;
        //}

        musicSource.clip = newMusic;
        musicSource.PlayScheduled(0);
        
        t = 0;

        while (t < volumeFadeTime)
        {
            t += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0, desiredVolume, t / volumeFadeTime);
            yield return null;
        }

        musicSource.volume = desiredVolume;
    }

    /// <summary>
    /// Changes music to given audio clip
    /// </summary>
    /// <param name="newMusic">Audio clip of music to play</param>
    public void ChangeMusic(AudioClip newMusic)
    {
        StartCoroutine(FadeOutMusic(newMusic));
    }

    /// <summary>
    /// Plays the loading screen music
    /// </summary>
    public void PlayLoadingScreen()
    {
        StartCoroutine(FadeOutMusic(loadingScreenMusic));
    }

    /// <summary>
    /// Plays the menu music
    /// </summary>
    public void PlayMenuMusic()
    {
        StartCoroutine(FadeOutMusic(menuMusic));
    }

    public void PlayEndingMusic()
    {
        StartCoroutine(FadeOutMusic(endingMusic));
    }
}
