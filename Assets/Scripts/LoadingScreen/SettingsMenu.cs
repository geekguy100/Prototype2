using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class SettingsMenu : MonoBehaviour
{
    Resolution[] resolutions;
    public AudioMixer music;
    public AudioMixer ui;
    public TMP_Dropdown resolutionDropdown;
    public List<string> options = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
   
        resolutionDropdown.ClearOptions();
        resolutions = Screen.resolutions.Distinct().OrderBy(x => x.width).ThenBy(x => x.height).ThenBy(x => x.refreshRate).ToArray();
        
        int currentResolutionIndex = 0;


        foreach (var res in resolutions)
        {
            Debug.Log(res.width + "x" + res.height + " : " + res.refreshRate);
        }

        System.Collections.Generic.List<Resolution> list = new System.Collections.Generic.List<Resolution>(resolutions);
        list.RemoveAt(0);
        list.RemoveAt(0);
        list.RemoveAt(0);
        list.RemoveAt(0);
        resolutions = list.ToArray();


        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                currentResolutionIndex = i;
        }

        resolutionDropdown.AddOptions(options);
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
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width,
                  resolution.height, Screen.fullScreen);
    }
}
