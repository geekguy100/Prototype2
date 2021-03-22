/*****************************************************************************
// File Name :         PauseManager.cs
// Author :            TJ Caron
// Creation Date :     03/13/2021
//
// Brief Description : Pauses the game when player is in gameplay/results screen 
                       and presses Escape.
*****************************************************************************/
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [Tooltip("Canvas object containing results screen")]
    public GameObject resultsObj;

    [Tooltip("Canvas object containing gameplay screen")]
    public GameObject gameplayObj;

    [Tooltip("Canvas object containing the character")]
    public GameObject characterObj;

    [Tooltip("Canvas object containing pause panel")]
    public GameObject pausePanel;

    [Tooltip("Canvas object containing tutorial panel")]
    public GameObject tutorialPanel;

    [Tooltip("Canvas object containing settings panel")]
    public GameObject settingsPanel;

    [Tooltip("Transition panel object")]
    public Image transitionImage;

    [Tooltip("Audio source for tutorial typing")]
    public AudioSource typeSource;

    [Tooltip("Whether or not the game is paused")]
    [SerializeField]private bool paused = false;

    [Tooltip("Whether or not the game can be paused")]
    public bool canPause = false;

    /// <summary>
    /// whether or not the typing source was playing when player paused
    /// </summary>
    private bool wasPlaying = false;

    // Update is called once per frame
    void Update()
    {
        if (canPause)
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                print("canPause = " + canPause);
                // Makes sure player is at results/gameplay screen
                if (resultsObj.activeInHierarchy || gameplayObj.activeInHierarchy || tutorialPanel.activeInHierarchy)
                {
                    // Checks if it is unpaused currently
                    if (!paused)
                    {
                        // Pauses game
                        Time.timeScale = 0;
                        paused = true;
                        pausePanel.SetActive(true);
                        if (transitionImage.raycastTarget)
                        {
                            transitionImage.raycastTarget = false;
                        }
                        if (tutorialPanel.activeInHierarchy)
                        {
                            if (typeSource.isPlaying)
                            {
                                typeSource.Stop();
                                wasPlaying = true;
                            }
                        }
                        //characterObj.SetActive(false);
                    }
                    else
                    {
                        // Unpauses game
                        Time.timeScale = 1;
                        paused = false;
                        pausePanel.SetActive(false);
                        if (settingsPanel.activeInHierarchy)
                        {
                            settingsPanel.SetActive(false);
                        }
                        if (tutorialPanel.activeInHierarchy)
                        {
                            if (wasPlaying)
                            {
                                typeSource.Play();
                                wasPlaying = false;
                            }
                        }
                        //characterObj.SetActive(true);
                    }
                }
            }
        }  
    }

    /// <summary>
    /// Sets timescale back to normal and resets paused variable to false
    /// </summary>
    public void Unpause()
    {
        // Unpauses game
        Time.timeScale = 1;
        paused = false;
        if (settingsPanel.activeInHierarchy)
        {
            settingsPanel.SetActive(false);
        }
        if (tutorialPanel.activeInHierarchy)
        {
            if (wasPlaying)
            {
                typeSource.Play();
                wasPlaying = false;
            }
        }
        //characterObj.SetActive(true);
    }
}
