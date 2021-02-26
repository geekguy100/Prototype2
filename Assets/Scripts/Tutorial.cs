/*****************************************************************************
// File Name :         Tutorial.cs
// Author :            TJ Caron
// Creation Date :     02/25/2021
//
// Brief Description : Runs the tutorial when the player first starts the game.
*****************************************************************************/
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [Tooltip("Current step of the tutorial")]
    [SerializeField] private int step = 0;

    [Tooltip("Tutorial panels in order")]
    [SerializeField] private GameObject[] panels;

    [Tooltip("The tutorial Previous Button")]
    [SerializeField] private Button prevButton;

    [Tooltip("The tutorial Next Button")]
    [SerializeField] private Button nextButton;

    [Tooltip("The tutorial Menu Button")]
    [SerializeField] private Button menuButton;

    [Tooltip("The tutorial Start Button")]
    [SerializeField] private Button startButton;

    [Tooltip("The Game Manager script")]
    [SerializeField] private GameManager gameManager;

    
    void Start()
    {
        // Activates the correct buttons for the beginning of the tutorial
        menuButton.gameObject.SetActive(true);
        nextButton.gameObject.SetActive(true);
        prevButton.gameObject.SetActive(false);
        startButton.gameObject.SetActive(false);
    }


    /// <summary>
    /// Shows the next tutorial screen
    /// </summary>
    public void NextStep()
    {
        // Hides old panel, increases step, shows new panel
        panels[step].SetActive(false);
        step++;
        panels[step].SetActive(true);

        // Shows start button instead of next button if its the last step
        if (step == panels.Length - 1)
        {
            nextButton.gameObject.SetActive(false);
            startButton.gameObject.SetActive(true);
            gameManager.CompleteTutorial();
        }

        // Shows previous button if menu button is showing instead
        if (!prevButton.gameObject.activeInHierarchy)
        {
            prevButton.gameObject.SetActive(true);
            menuButton.gameObject.SetActive(false);
        }

    }

    /// <summary>
    /// Shows the previous tutorial screen
    /// </summary>
    public void PreviousStep()
    {
        // Hides old panel, decreases step, shows new panel
        panels[step].SetActive(false);
        step--;
        panels[step].SetActive(true);

        // Shows menu button instead of previous button if its the first step
        if (step == 0)
        {
            prevButton.gameObject.SetActive(false);
            menuButton.gameObject.SetActive(true);          
        }

        // Shows next button if start button is showing instead
        if (!nextButton.gameObject.activeInHierarchy)
        {
            nextButton.gameObject.SetActive(true);
            startButton.gameObject.SetActive(false);
        }        
    }

}
