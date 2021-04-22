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
using System.Collections;

public class Tutorial : MonoBehaviour
{
    [Tooltip("Tutorial roomba AnimationController")]
    public AnimationController roombaAC;

    [Tooltip("Current step of the tutorial")]
    [SerializeField] private int step = 0;

    [Tooltip("Step of the tutorial that contains the role select screen")]
    [SerializeField] private int roleSelectStep = 3;

    [Tooltip("Step of the tutorial that contains the role select screen")]
    [SerializeField] private int statsStep = 6;

    [Tooltip("Tutorial panels in order")]
    [SerializeField] private GameObject[] panels;

    [Tooltip("First tutorial text box for each panel in order")]
    [SerializeField] private TextScroll[] texts;

    [Tooltip("Gameobject holding the finance booklet link")]
    [SerializeField] private GameObject financeBooklet;

    [Tooltip("Gameobject holding the PR booklet link")]
    [SerializeField] private GameObject prBooklet;

    [Tooltip("Gameobject holding the PR booklet link")]
    [SerializeField] private GameObject tutorialStatsPanel;

    [Tooltip("Gameplay tutorial setup text")]
    [SerializeField] private TextMeshProUGUI[] setUpTexts;

    [Tooltip("Text Scroll scripts attached to the gameplay setup texts")]
    public TextScroll[] gameplayScrolls;

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

    [Tooltip("The role description panel")]
    [SerializeField] private GameObject roleDescriptionPanel;

    [Tooltip("The role description text")]
    [SerializeField] private TextMeshProUGUI roleDescriptionText;

    [Tooltip("Role descriptions")]
    public string[] roleDescriptions;

    [Tooltip("Buttons to track player choice")]
    public Button[] tutorialChoiceButtons;

    public TextScroll resultsScroll;

    [Tooltip("Text for tutorial step explaining timer")]
    [SerializeField]
    private string timerText = "Each scenario will have a time limit in which you need to decide how to respond. " +
                               "The timer in the upper right corner of the screen shows you how much time you have left for this scenario. " +
                               "The faster you respond to each scenario, the more effective your choice will be. " +
                               "Click one of the choice buttons below to continue.";

    [Tooltip("Text for tutorial step explaining choice selection")]
    [SerializeField]
    private string choiceSelectText = "To decide how you are going to respond to each scenario, you need to click two buttons. " +
                                  "First, you need to click on the choice button that you want to go with. " +
                                  "Then you click the confirm button in the bottom right of the screen to lock in that choice. " +
                                  "Select one of the choices below then confirm it to continue.";

    [Tooltip("Text for tutorial step explaining closing the stats panel")]
    [SerializeField]
    private string closeStatsText = "Press the Stats button or the X in the upper right corner of the Stats panel to close it. " +
                                    "Close the Stats panel to continue. ";

    [Tooltip("The GameObject holding all assets relating to the interactive tutorial.")]
    [SerializeField] private GameObject tutorialGameplayPanel;

    [Tooltip("The scale of the tutorial background during role select step")]
    [SerializeField] private Vector3 zoomedScale = new Vector3(1.2f, 1.2f);

    [Tooltip("The default scale of the tutorial background")]
    [SerializeField] private Vector3 originalScale = new Vector3(1f, 1f);

    [Tooltip("The speed of the tutorial zoom")]
    [SerializeField] private float zoomSpeed = .5f;

    [Tooltip("The tutorial ResultsHandler")]
    [SerializeField] private ResultsHandler resultsHandler;

    [Tooltip("SFX Audio Source")]
    public AudioSource sfxSource;

    [Tooltip("Timer warning audio clip")]
    public AudioClip timerWarning;

    [Tooltip("Volume timer warning should play at")]
    public float warningVolume = .3f;

    [Tooltip("Projector Button object")]
    public GameObject projectorButton;



    private string resultsString = "This screen will show the results of your choice. It will describe the effects of your decision and display how your stats have changed because of it." +
                                   "\n\nAlso, you can see how your answer compares to other groups that have played this game in the starburst on the left." +
                                   "\n\nYou are now ready to begin the game. Make sure all of your advisors are ready, then press the \"Start Game\" button below to begin.";

    [Tooltip("The tutorial stats (used for animating results screen)")]
    [SerializeField] private float[] stats;

    [Tooltip("The Chibi Character who walks the player through the tutorial.")]
    [SerializeField] private ChibiCharacter chibiCharacter;

    /// <summary>
    /// Step of the tutorial that explains the timer
    /// </summary>
    private int timerStep = -1;

    /// <summary>
    /// Step of the tutorial that explains closing the stats panel
    /// </summary>
    private int statsCloseStep = -1;

    /// <summary>
    /// Step of the tutorial that selecting a choice
    /// </summary>
    private int choiceSelectStep = -1;


    /// <summary>
    /// Whether or not the tutorial background has been zoomed in 
    /// </summary>
    private bool zoomed = false;

    [Tooltip("Color that the choice buttons change to when they are chosen")]
    public Color chosenColor;

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

    private int gameplayStep = 0;
    void Start()
    {
        // Activates the correct buttons for the beginning of the tutorial
        menuButton.gameObject.SetActive(true);
        nextButton.gameObject.SetActive(true);
        prevButton.gameObject.SetActive(false);
        panels[step].SetActive(true);

        statsCloseStep = statsStep + 1;
        timerStep = statsCloseStep + 1;
        choiceSelectStep = timerStep + 1;
    }


    /// <summary>
    /// Shows the next tutorial screen
    /// </summary>
    public void NextStep()
    {
        texts[step].StopTypeWriter();
        step++;

        // Shows start button instead of next button if its the last step
        // This will happen when the tutorial transitions to an explanation of the 
        // gameplay 
        if (step > panels.Length - 1)
        {
            Transition.instance.StartTransition(StartInteractiveTutorial, false);
            return;
        }
        // If we're not on the last step of the tutorial (before going to the interactive section), 
        // make sure to hide the previous slide and show the current one.
        else
        {
            
            if (step == roleSelectStep)
            {
                projectorButton.SetActive(false);
                StartCoroutine(ZoomIn());
            }
            else if (step == (roleSelectStep + 1))
            {
                projectorButton.SetActive(true);
                panels[step - 1].SetActive(false);
                StartCoroutine(ZoomOut());
            }
            else
            {
                panels[step - 1].SetActive(false);
                panels[step].SetActive(true);
                texts[step - 1].FinishScroll();
            }
        }


        // Shows previous button if it isn't showing
        if (!prevButton.gameObject.activeInHierarchy && step < panels.Length && step != roleSelectStep && step != roleSelectStep + 1)
        {
            prevButton.gameObject.SetActive(true);
        }
        // Hides next button if player is on role select panel
        if (step == roleSelectStep)
        {
            nextButton.gameObject.SetActive(false);
        }
    }


    private void StartInteractiveTutorial()
    {
        prevButton.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);
        panels[step - 1].SetActive(false);
        texts[step - 1].StopTypeWriter();
        tutorialBackground.SetActive(false);
        projector.SetActive(false);
        tutorialGameplayPanel.SetActive(true);
    }

    /// <summary>
    /// Switches to timer step of the tutorial
    /// </summary>
    private void OpenGameplayStep()
    {
        setUpTexts[gameplayStep].gameObject.SetActive(false);
        gameplayScrolls[gameplayStep].FinishScroll();
        roombaAC.StopRoombaSound();
        step++;
        gameplayStep++;
        setUpTexts[gameplayStep].gameObject.SetActive(true);
    }

    /// <summary>
    /// Switches to close stats step of the tutorial
    /// </summary>
    //private void OpenStatsCloseStep()
    //{        
    //    step++;
    //    setUpText.text = closeStatsText;
    //    gameplayScroll.NewScroll();
    //}

    /// <summary>
    /// Switches to choice select step of the tutorial
    /// </summary>
    //private void OpenChoiceSelectStep()
    //{
    //    step++;
    //    setUpText.text = choiceSelectText;
    //}

    /// <summary>
    /// Switches to results step of the tutorial
    /// </summary>
    private void OpenResultsStep()
    {
        step++;
        tutorialGameplayPanel.SetActive(false);
        gameplayScrolls[gameplayStep].FinishScroll();
        tutorialResults.SetActive(true);

        resultsHandler.Display(stats, resultsString);
        // Marks that the player has completed the tutorial - this is the last step
        gameManager.CompleteTutorial();
    }

    /// <summary>
    /// Shows the previous tutorial screen
    /// </summary>
    public void PreviousStep()
    {
        texts[step].ResetScroll();
        // Zooms out if coming from role select step
        if (step == roleSelectStep)
        {
            panels[step].SetActive(false);
            step--;
            texts[step].FinishScroll();
            StartCoroutine(ZoomOut());
        }
        else if (step == roleSelectStep + 1)
        {
            projectorButton.SetActive(false);
            panels[step].SetActive(false);
            step--;
            texts[step].FinishScroll();
            nextButton.gameObject.SetActive(false);
            StartCoroutine(ZoomIn());
        }
        // Hides old panel, decreases step, shows new panel
        else
        {
            panels[step].SetActive(false);
            step--;
            panels[step].SetActive(true);
        }


        // Hides previous button if its the first step
        if (step == 0)
        {
            prevButton.gameObject.SetActive(false);
        }
        else if (step == roleSelectStep + 1)
        {
            prevButton.gameObject.SetActive(false);
        }

        //// Shows next button if start button is showing instead
        //if (!nextButton.gameObject.activeInHierarchy && (step + 1) != roleSelectStep)
        //{
        //    nextButton.gameObject.SetActive(true);
        //}        
    }

    /// <summary>
    /// Zooms in on tutorial background to provide more space for the role select panel
    /// </summary>
    /// <returns></returns>
    private IEnumerator ZoomIn()
    {
        Vector3 newScale = new Vector3();
        float change = 0;
        panels[step - 1].SetActive(false);
        prevButton.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);
        
        while (!zoomed)
        {
            // Checks if it hit target scale
            if (tutorialBackground.transform.localScale.x >= zoomedScale.x)
            {
                tutorialBackground.transform.localScale = zoomedScale;
                zoomed = true;
            }
            else
            {
                // Increases scale over time
                change = Time.deltaTime * zoomSpeed;
                newScale = new Vector3(tutorialBackground.transform.localScale.x + change,
                                       tutorialBackground.transform.localScale.y + change);
                tutorialBackground.transform.localScale = newScale;
            }
            yield return 0;
        }
        // Shows new panel
        panels[step].SetActive(true);
        texts[step].gameObject.SetActive(true);
        prevButton.gameObject.SetActive(true);

        // Make the chibi character point to the projector screen.
        chibiCharacter.PointToProjector();
    }

    /// <summary>
    /// Zooms in on tutorial background to provide more space for the role select panel
    /// </summary>
    /// <returns></returns>
    private IEnumerator ZoomOut()
    {
        Vector3 newScale = new Vector3();
        float change = 0;

        // Hides buttons
        prevButton.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);

        while (zoomed)
        {
            // Checks if it hit original scale
            if (tutorialBackground.transform.localScale.x <= originalScale.x)
            {
                tutorialBackground.transform.localScale = originalScale;
                zoomed = false;
            }
            else
            {
                // Decreases scale over time
                change = Time.deltaTime * zoomSpeed;
                newScale = new Vector3(tutorialBackground.transform.localScale.x - change,
                                       tutorialBackground.transform.localScale.y - change);
                tutorialBackground.transform.localScale = newScale;
            }
            yield return 0;
        }
        // Shows new panel
        panels[step].SetActive(true);
        if (step < roleSelectStep)
        {
            prevButton.gameObject.SetActive(true);
        }
        projectorButton.SetActive(true);
        nextButton.gameObject.SetActive(true);
        chibiCharacter.StopPointing();
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
        prevButton.gameObject.SetActive(false);
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
                step++;
                break;
            // Player chose PR advisor, takes them to their booklet link
            case 2:
                playerRole = Role.PR;
                panels[step].SetActive(false);
                prBooklet.SetActive(true);
                step++;
                break;
            // Player chose efficiency advisor, takes them to their booklet link
            case 3:
                playerRole = Role.Efficiency;
                panels[step].SetActive(false);
                efficiencyBooklet.SetActive(true);
                step++;
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
            //OpenStatsCloseStep();
            OpenGameplayStep();
        }
        else if (step == statsCloseStep)
        {
            OpenGameplayStep();
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
        tutorialChoiceButtons[buttonNum].GetComponent<Image>().color = chosenColor;
        if (step == timerStep)
        {
            //gameplayScroll.FinishScroll();
            //OpenChoiceSelectStep();
            OpenGameplayStep();
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


    /// <summary>
    /// Opens the role panel when player hovers over the icon
    /// </summary>
    /// <param name="roleNum">The role the player selected
    ///                       0 = Manager
    ///                       1 = Finance
    ///                       2 = PR
    ///                       3 = Efficiency </param>
    public void OpenRolePanel(int role)
    {
        roleDescriptionPanel.SetActive(true);
        roleDescriptionText.text = roleDescriptions[role];
    }

    /// <summary>
    /// Closes the role panel when player's mouse leaves the icon
    /// </summary>
    public void CloseRolePanel()
    {
        roleDescriptionPanel.SetActive(false);
    }

    public void EndTutorial()
    {
        resultsScroll.FinishScroll();
    }

    public GameObject GetCurrentPanel()
    {
        if (step < panels.Length)
        {
            return panels[step];
        }
        else
        {
            return null;
        }
    }

    public TextScroll GetCurrentTextScroll()
    {
        if (step < texts.Length)
        {
            return texts[step];
        }
        else
        {
            return null;
        }
    }
}
