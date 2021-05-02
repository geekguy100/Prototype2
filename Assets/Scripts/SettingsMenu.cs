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

    public Slider musicSlider;
    public Slider masterSlider;
    public Slider sfxSlider;
    public Toggle fullscreenToggle;

    public AudioMixer music;
    public AudioMixer ui;
    public TMP_Dropdown resolutionDropdown;

    public Vector2 previousResolution;
    public int previousValue = 0;


    public List<Vector2> options = new List<Vector2>();
    public List<string> optionsText = new List<string>();
    Resolution currentRes = new Resolution();
    public int defaultRefreshRate = 30;

    public GameObject areYouSureButton;

    public static float musicVolume = 0;
    public static float masterVolume = 1;
    public static float sfxVolume = 0;
    public static bool isFullSCREEN;

    // Start is called before the first frame update
    void Start()
    {
        resolutionDropdown.ClearOptions();



        /*
         4:3 aspect ratio resolutions: 1024×768, 1280×960, 1400×1050, 1440×1080, 1600×1200, 1856×1392, 1920×1440, and 2048×1536.
           16:10 aspect ratio resolutions: 1280×800, 1440×900, 1680×1050, and 1920×1200.
        16:9 aspect ratio resolutions: 1024×576, 1152×648, 1280×720 (HD), 1366×768, 1600×900, and 1920×1080 (FHD)
         
         */
        masterSlider.value = AudioListener.volume;
        music.GetFloat("MusicVolume", out musicVolume);
        ui.GetFloat("UIVolume", out sfxVolume);

        musicSlider.value = musicVolume;
        sfxSlider.value = sfxVolume;

        isFullSCREEN = Screen.fullScreen;
        fullscreenToggle.isOn = isFullSCREEN;
        currentRes = Screen.currentResolution;

        for(int i = 0; i < options.Count; i++)
        {
            if (currentRes.width == options[i].x && currentRes.height == options[i].y)
            {
                resolutionDropdown.value = i;
                break;
            }
        }

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
        masterVolume = newVolume;
    }

    //adjust ui volume
    public void AdjustUIVolume(float newVolume)
    {
        ui.SetFloat("UIVolume", newVolume);
        sfxVolume = newVolume;
    }

    //adjust music volume
    public void AdjustMusicVolume(float newVolume)
    {
        music.SetFloat("MusicVolume",newVolume);
        musicVolume = newVolume;
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        if(resolutionIndex != 0)
        {
            previousResolution = new Vector2(Screen.width, Screen.height);
            Screen.SetResolution((int)options[resolutionIndex].x, (int)options[resolutionIndex].y, Screen.fullScreen, defaultRefreshRate);
            areYouSureButton.SetActive(true);
        }

    }

    public void ResetResolutionButton()
    {
        Screen.SetResolution((int)previousResolution.x, (int)previousResolution.y, Screen.fullScreen, defaultRefreshRate);
        resolutionDropdown.value = previousValue;
        areYouSureButton.SetActive(false);
    }

    public void SettingsAreFine()
    {
        areYouSureButton.SetActive(false);
        previousValue = resolutionDropdown.value;
    }

    public void CheckEffectsVolume()
    {
        sfxSource.clip = ding;
        sfxSource.PlayScheduled(0);
    }
}
