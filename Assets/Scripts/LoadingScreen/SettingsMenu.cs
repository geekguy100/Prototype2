using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer music;
    public AudioMixer ui;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //adjust ui volume
    public void AdjustUIVolume(float newVolume)
    {
        ui.SetFloat("UIVolume", newVolume);
    }

    //adjust music volume
    public void AdjustMusicVolume(float newVolume)
    {
        music.SetFloat("MusicVolume",newVolume);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}
