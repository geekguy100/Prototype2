using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class SettingsMenu : MonoBehaviour
{
    public AudioSource sfxSource;
    public AudioClip ding;

    public AudioMixer music;
    public AudioMixer ui;
    public TMP_Dropdown resolutionDropdown;
    public List<Vector2> options = new List<Vector2>();
    public List<string> optionsText = new List<string>();
    public int defaultRefreshRate = 30;

    // Start is called before the first frame update
    void Start()
    {
        resolutionDropdown.ClearOptions();




        /*
         4:3 aspect ratio resolutions: 1024×768, 1280×960, 1400×1050, 1440×1080, 1600×1200, 1856×1392, 1920×1440, and 2048×1536.
           16:10 aspect ratio resolutions: 1280×800, 1440×900, 1680×1050, and 1920×1200.
        16:9 aspect ratio resolutions: 1024×576, 1152×648, 1280×720 (HD), 1366×768, 1600×900, and 1920×1080 (FHD)
         
         */
        optionsText[0] = "";

        for (int i = 1;i<options.Count; i++)
        {
            optionsText[i] = ((int)options[i].x +"  x  "+ (int)options[i].y).ToString();
        }

        resolutionDropdown.AddOptions(optionsText);
        resolutionDropdown.RefreshShownValue();
    }

        // Update is called once per frame
        public void AdjustMasterVolume(float newVolume)
    {
        AudioListener.volume = newVolume;
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

    public void SetResolution(int resolutionIndex)
    {
 
        Screen.SetResolution((int)options[resolutionIndex].x,(int)options[resolutionIndex].y,Screen.fullScreen,defaultRefreshRate);
    }

    public void CheckEffectsVolume()
    {
        sfxSource.clip = ding;
        sfxSource.PlayScheduled(0);
    }
}
