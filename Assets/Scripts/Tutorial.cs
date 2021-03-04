/*****************************************************************************
// File Name :         Tutorial.cs
// Author :            TJ Caron
// Creation Date :     02/25/2021
//
// Brief Description : Runs the tutorial when the player first starts the game.
*****************************************************************************/
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tutorial : MonoBehaviour
{
    [Tooltip("Current step of the tutorial")]
    [SerializeField] private int step = 0;

    [Tooltip("Step of the tutorial that contains the role select screen")]
    [SerializeField] private int roleSelectStep = 3;

    [Tooltip("Step of the tutorial that contains the role select screen")]
    [SerializeField] private int statsStep = 6;

    [Tooltip("Tutorial panels in order")]
    [SerializeField] private GameObject[] panels;

    [Tooltip("Gameobject holding the finance booklet link")]
    [SerializeField] private GameObject financeBooklet;

    [Tooltip("Gameobject holding the PR booklet link")]
    [SerializeField] private GameObject prBooklet;

    [Tooltip("Gameobject holding the PR booklet link")]
    [SerializeField] private GameObject tutorialStatsPanel;

    [Tooltip("Gameplay tutorial setup text")]
    [SerializeField] private TextMeshProUGUI setUpText;

    [Tooltip("Gameobject holding the Efficiency booklet link")]
    [SerializeField] private GameObject efficiencyBooklet;

    [Tooltip("The projector object")]
    [SerializeField] private GameObject projector;

    [Tooltip("The tutorial background")]
    [SerializeField] private GameObject tutorialBackground;

    [Tooltip("The tutorial results screen")]
    [SerializeField] private GameObject tutorialResults;

    [Tooltip("The tutorial Previous Button")]
    [SerializeField] private Button prevButton;

    [Tooltip("The tutorial Next Button")]
    [SerializeField] private Button nextButton;

    [Tooltip("The tutorial Menu Button")]
    [SerializeField] private Button menuButton;

    [Tooltip("The Game Manager script")]
    [SerializeField] private GameManager gameManager;

    [Tooltip("Buttons to track player choice")]
    public Button[] tutorialChoiceButtons;

    [Tooltip("Gameobject holding the PR booklet link")]
    [SerializeField]
    private string timerText = "Press the Stats button or the X in the upper right corner of the Stats panel to close it. " +
                               "Each scenario will have a time limit in which you need to decide how to respond. " +
                               "The timer in the upper right corner of the screen shows you how much time you have left for this scenario. " +
                               "The faster you respond to each scenario, the more effective your choice will be. " +
                               "Click one of the choice buttons below to continue.";

    [Tooltip("Gameobject holding the PR booklet link")]
    [SerializeField]
    private string choiceSelectText = "To decide how you are going to respond to each scenario, you need to click two buttons. " +
                                  "First, you need to click on the choice button that you want to go with. " +
                                  "Then you click the confirm button in the bottom right of the screen to lock in that choice. " +
                                  "Select one of the choices below then confirm it to continue.";

    [Tooltip("The GameObject holding all assets relating to the interactive tutorial.")]
    [SerializeField] private GameObject tutorialGameplayPanel;

    /// <summary>
    /// Step of the tutorial that explains the timer
    /// </summary>
    private int timerStep = -1;

    /// <summary>
    /// Step of the tutorial that selecting a choice
    /// </summary>
    private int choiceSelectStep = -1;

    /// <summary>
    /// Holds which role the player selects
    /// </summary>
    public enum Role
    {
        Manager, Finance, PR, Efficiency, Unset
    }

    /// <summary>
    /// Role the player selects
    /// </summary>
    private Role playerRole = Role.Unset;

    
    void Start()
    {
        // Activates the correct buttons for the beginning of the tutorial
        menuButton.gameObject.SetActive(true);
        nextButton.gameObject.SetActive(true);
        prevButton.gameObject.SetActive(false);

        panels[step].SetActive(true);

        timerStep = statsStep + 1;
        choiceSelectStep = timerStep + 1;
    }


    /// <summary>
    /// Shows the next tutorial screen
    /// </summary>
    public void NextStep()
    {
        step++;

        // Shows start button instead of next button if its the last step
        // This will happen when the tutorial transitions to an explanation of the 
        // gameplay 
        if (step >= panels.Length - 1)
        {
            Transition.instance.StartTransition(StartInteractiveTutorial);
            return;
        }
        // If we're not on the last step of the tutorial (before going to the interactive section), 
        // make sure to hide the previous slide and show the current one.
        else
        {
            panels[step - 1].SetActive(false);
            panels[step].SetActive(true);
        }


        // Shows previous button if it isn't showing
        if (!prevButton.gameObject.activeInHierarchy && step < panels.Length)
        {
            prevButton.gameObject.SetActive(true);
        }
        // Hides next button if player is on role select panel
        if (step == roleSelectStep)
        {
            nextButton.gameObject.SetActive(false);
            prevButton.gameObject.SetActive(true);
        }
    }

    private void StartInteractiveTutorial()
    {
        prevButton.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);
        panels[step - 1].SetActive(false);
        tutorialBackground.SetActive(false);
        projector.SetActive(false);
        tutorialGameplayPanel.SetActive(true);
    }

    /// <summary>
    /// Opens the final step of the tutorial, completes the tutorial so that if
    /// player goes back to main menu they don't have to do it again.
    /// </summary>
    private void OpenFinalStep()
    {            
        //startButton.gameObject.SetActive(true);
        gameManager.CompleteTutorial();       
    }

    /// <summary>
    /// Switches to timer step of the tutorial
    /// </summary>
    private void OpenTimerStep()
    {
        step++;
        setUpText.text = timerText;
    }

    /// <summary>
    /// Switches to choice select step of the tutorial
    /// </summary>
    private void OpenChoiceSelectStep()
    {
        step++;
        setUpText.text = choiceSelectText;

    }

    /// <summary>
    /// Switches to results step of the tutorial
    /// </summary>
    private void OpenResultsStep()
    {
        step++;
        tutorialGameplayPanel.SetActive(false);
        tutorialResults.SetActive(true);

        // Marks that the player has completed the tutorial - this is the last step
        gameManager.CompleteTutorial();
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

        // Hides previous button if its the first step
        if (step == 0)
        {
            prevButton.gameObject.SetActive(false);
        }

        // Shows next button if start button is showing instead
        if (!nextButton.gameObject.activeInHierarchy)
        {
            nextButton.gameObject.SetActive(true);
        }        
    }

    /// <summary>
    /// Called when the player selects which role they are playing.
    /// </summary>
    /// <param name="roleNum">The role the player selected
    ///                       0 = Manager
    ///                       1 = Finance
    ///                       2 = PR
    ///                       3 = Efficiency </param>
    public void SelectRole(int chosenRole)
    {
        switch (chosenRole)
        {
            // Player chose manager, keeps tutorial going
            case 0:
                playerRole = Role.Manager;
                nextButton.gameObject.SetActive(true);
                NextStep();
                break;
            // Player chose finance advisor, takes them to their booklet link
            case 1:
                playerRole = Role.Finance;
                panels[step].SetActive(false);
                financeBooklet.SetActive(true);
                prevButton.gameObject.SetActive(false);
                break;
            // Player chose PR advisor, takes them to their booklet link
            case 2:
                playerRole = Role.PR;
                panels[step].SetActive(false);
                prBooklet.SetActive(true);
                prevButton.gameObject.SetActive(false);
                break;
            // Player chose efficiency advisor, takes them to their booklet link
            case 3:
                playerRole = Role.Efficiency;
                panels[step].SetActive(false);
                efficiencyBooklet.SetActive(true);
                prevButton.gameObject.SetActive(false);
                break;
            // Shouldnt get here but moves forward with tutorial as normal
            default:
                playerRole = Role.Unset;
                nextButton.gameObject.SetActive(true);
                NextStep();
                break;
        }
    }

    /// <summary>
    /// Opens/closes tutorial stats panel based on whether it is already open or not
    /// </summary>
    public void TutorialStatsPanel()
    {
        print(step + "  " + statsStep);
        if (step == statsStep)
        {
            OpenTimerStep();
        }
        if (!tutorialStatsPanel.activeInHierarchy)
        {
            tutorialStatsPanel.SetActive(true);
        }
        else
        {
            tutorialStatsPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Highlights button player selected 
    /// </summary>
    public void HoldSelection(int buttonNum)
    {
        foreach (Button b in tutorialChoiceButtons)
        {
            b.GetComponent<Image>().color = Color.white;
        }
        tutorialChoiceButtons[buttonNum].GetComponent<Image>().color = Color.yellow;
        if (step == timerStep)
        {
            OpenChoiceSelectStep();
        }
    }

    /// <summary>
    /// Moves from the tutorial to starting the game. Happens when player presses confirm with a choice selected.
    /// </summary>
    public void TutorialConfirm() 
    { 
        if (step == choiceSelectStep)
        {
            OpenResultsStep();
        }
    }
}
