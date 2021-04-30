/*****************************************************************************
// File Name :         ProjectorButton.cs
// Author :            TJ Caron
// Creation Date :     04/18/2021
//
// Brief Description : Changes the tutorial projector screen when clicked on
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectorButton : Interactable
{
    [Tooltip("Tutorial script")]
    public Tutorial tutorial;

    private bool offTutorial = false;

    [Tooltip("Buttons to hide when player hides tutorial")]
    public GameObject[] tutorialButtons;

    /// <summary>
    /// Currently showing tutorial panel
    /// </summary>
    private GameObject currentTutorialPanel;

    /// <summary>
    /// Currently showing text scroll
    /// </summary>
    private TextScroll currentTutorialTextScroll;

    /// <summary>
    /// Whether or not each tutorial button is on
    /// </summary>
    private bool[] tutorialButtonStates = new bool[3];

    [Tooltip("Screen to show when player clicks button to hide tutorial")]
    public GameObject chromeScreen;

    [Tooltip("Sound to play when player leaves tutorial")]
    public AudioClip leaveTutorialSound;

    public void Interact()
    {
        if (!revealed)
        {
            revealed = true;
            ChangeCursor(revealed);
        }
        sfxSource.clip = leaveTutorialSound;
        sfxSource.PlayScheduled(0);
        currentTutorialPanel = tutorial.GetCurrentPanel();
        currentTutorialTextScroll = tutorial.GetCurrentTextScroll();
        if (currentTutorialPanel != null)
        {
            offTutorial = !offTutorial;

            if (offTutorial)
            {
                currentTutorialTextScroll.ResetScroll();
                chromeScreen.SetActive(true);
                
                currentTutorialPanel.SetActive(false);

                // Stores which buttons were showing for when they need to be turned back on
                for (int i = 0; i < tutorialButtons.Length; i++)
                {
                    tutorialButtonStates[i] = tutorialButtons[i].activeInHierarchy;
                }

                // Hides buttons 
                foreach (GameObject obj in tutorialButtons)
                {
                    obj.SetActive(!offTutorial);
                }
            }
            else
            {
                chromeScreen.SetActive(false);
                currentTutorialPanel.SetActive(true);
                
                // Shows buttons that were showing when player left tutorial
                for (int i = 0; i < tutorialButtons.Length; i++)
                {
                    tutorialButtons[i].SetActive(tutorialButtonStates[i]);
                }
            }
        }
        else
        {
            Debug.LogError("Tutorial step too high");
        }
    }
}
