using System.Collections.Generic;
using System.Collections;
using System.IO;
using FileLoading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using LeaderboardInfo;


public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Holds whether or not the player has completed the tutorial in this play session
    /// </summary>
    public static bool completedTutorial = false;

    [Tooltip("All the text files for each setup, one per setup. The ID is the files index in the array")]
    private string[] scenarioFiles;

    public GameObject noSelectionPanel;

    [Tooltip("Panel that holds the stats - opened when player clicks stats button")]
    public GameObject statsPanel;

    [Tooltip("Color that the choice buttons change to when they are chosen")]
    public Color chosenColor;

    [Tooltip("Used for referencing the title animation sequence")]
    public GameObject animationHolder;

    #region Choice Tracking
    [Header("Tracking and changing how many choices the players have")]
    [Tooltip("What is the maximum number of choices the players will have")]
    public int maxChoices = 12;

    /// <summary>
    /// How many choices the players have made so far
    /// </summary>
    private int choicesMade;

    /// <summary>
    /// Holds choice that the player has clicked but not submitted
    /// </summary>
    private int currentSelection = -1;

    /// <summary>
    /// Paths to the different backgrounds for the approval endings
    /// </summary>
    private string[] approvalEndingBackgrounds =
        {"Endings/Backgrounds/ApprovalBad", "Endings/Backgrounds/ApprovalNeutral", "Endings/Backgrounds/ApprovalGood"};

    /// <summary>
    /// Paths to the different backgrounds for the efficiency endings
    /// </summary>
    private string[] efficiencyEndingBackgrounds =
    {
    "Endings/Backgrounds/EfficiencyBad", "Endings/Backgrounds/EfficiencyNeutral",
    "Endings/Backgrounds/EfficiencyGood"
    };

    /// <summary>
    /// Paths to the different backgrounds for the finance endings
    /// </summary>
    private string[] financeEndingBackgrounds =
        {"Endings/Backgrounds/FinanceBad", "Endings/Backgrounds/FinanceNeutral", "Endings/Backgrounds/FinanceGood"};

    /// <summary>
    /// Used to hold the information returned about the specific ending
    /// </summary>
    struct Ending
    {
        /// <summary>
        /// The text of the ending
        /// </summary>
        public string text;

        /// <summary>
        /// The path to the background of the ending 
        /// </summary>
        public string backgroundPath;
    }
    #endregion

    #region GameObjects
    [Header("GameObjects modified throughout the game")]
    [Tooltip("The text to show the current setup")]
    public TextMeshProUGUI setupText;

    [Tooltip("The text to show the stats change after a question setup")]
    public TextMeshProUGUI resultsText;

    [Tooltip("The text to show your impact from your choices over the entire game")]
    public TextMeshProUGUI endingText;

    [Tooltip("The parent of all the above objects. Used to turn them on and off")]
    public GameObject gameplayObject;

    [Tooltip("The first UI object visible, allows the user to configure the game")]
    public GameObject setupObject;

    [Tooltip("The parent object of the tutorial")]
    public GameObject tutorialObject;

    [Tooltip("The parent object of the ending")]
    public GameObject endingObject;

    [Tooltip("Text that displays all the options")]
    public Text choicesText;

    [Tooltip("The object that shows the scenario's icon")]
    public SpriteRenderer scenarioIcon;

    [Tooltip("Button for selecting the current choice")]
    public GameObject submitButton;



    [Tooltip("Sprite showing the current state of approval")]
    public SpriteRenderer approvalSprite;

    [Tooltip("Sprite showing the current state of effienency")]
    public SpriteRenderer efficiencySprite;

    [Tooltip("Sprite showing the current state of the environment")]
    public SpriteRenderer envrionmentSprite;

    [Tooltip("Sprite showing the current state of finance")]
    public SpriteRenderer financeSprite;

    [Tooltip("Parent object of all background sprites")]
    public GameObject backgroundStuff;

    [Tooltip("Sliders for tracking player values")]
    public Slider[] sliders;

    [Tooltip("A holder for all the sliders")]
    public GameObject sliderHolder;

    [Tooltip("Buttons to track player choice")]
    public Button[] choiceButtons;

    [Tooltip("Buttons to track player choice - only show if number of options is odd")]
    public Button[] middleChoiceButtons;

    [Tooltip("Objects holding each row of choice buttons")]
    public GameObject[] choiceButtonRows;

    [Tooltip("Text on each choice button")]
    public TextMeshProUGUI[] choiceTexts;

    [Tooltip("The leaderboard stamp to display percent of ppl who chose an answer to a question.")]
    public LeaderboardStamp leaderboardStamp;

    //Added by Kyle Grenier
    [Tooltip("The timer used to track time on each question.")]
    public Timer timer;

    [Tooltip("The on-screen character who interacts with the player's choices.")]
    public Character character;

    //Added by Ein
    public GameObject settingsPanel;

    /// <summary>
    /// The values at which the stat sliders change colors
    /// </summary>
    private float[] statSliderThresholds;



    #endregion

    #region Scenario and Setup management

    /// <summary>
    /// Object containing all the setups and their values
    /// </summary>
    private Scenarios currentScenario;

    /// <summary>
    /// Object containing the setup currently shown to the players
    /// </summary>
    private Scenario currentSetup;

    /// <summary>
    /// Keeps track of what choices are valid to ask by index. Once a choice has been asked it is
    /// removed from this list
    /// </summary>
    private List<int> validChoices = new List<int>();

    /// <summary>
    /// A collection of all the possible endings
    /// </summary>
    private Endings endings;

    /// <summary>
    /// Did the players get the godzilla setup
    /// </summary>
    private bool hadGodzilla = false;

    /// <summary>
    /// The ResultsHandler to manage displaying the results and updating values after each question.
    /// </summary>
    [SerializeField] private ResultsHandler resultsHandler = null;

    /// <summary>
    /// Whether or not there are an odd number of choices for this setup
    /// </summary>
    private bool oddNumChoices = false;
    #endregion

    #region Stat variables

    /// <summary>
    /// Stats are in the order approval, efficiency, envrionment, finance
    /// </summary>
    private float[] stats = { 50, 50, 50, 50 };
    private float[] statsDelta;
    [Header("Thresholds for various changes and backgrounds")]

    [Tooltip("Thresholds for the different backgrounds and endings")]
    public int[] thresholds;

    [Tooltip("Amount of stat loss if player runs out of time")]
    [SerializeField] private float statLoss = 5;

    #endregion

    /// <summary>
    /// Reference to script that handles displaying endings
    /// </summary>
    private EndingHandler endingHandler;


    /// <summary>
    /// Subscribe the ChoiceSelect event to OnTimerEnd, meaning ChoiceSelect() will be called once the timer ends.
    /// Added by Kyle Grenier
    /// </summary>
    private void Awake()
    {
        timer.OnTimerEnd += ChoiceSelect;
    }

    /// <summary>
    /// Unsubscribe to the OnTimerEnd event in the case GameManager is disabled or destroyed.
    /// Added by Kyle Grenier
    /// </summary>
    private void OnDisable()
    {
        timer.OnTimerEnd -= ChoiceSelect;
    }

    private void Start()
    {
        // The below code is for allowing the user to select a scenario file instead of defaulting to Scenarios.json
        //string[] scenarioArray = Directory.GetFiles("Assets/Resources/Scenarios", "*.json");
        //List<string> scenarioListTrimmed = new List<string>();
        //for (int i = 0; i < scenarioArray.Length; ++i)
        //{
        //    scenarioListTrimmed.Add(Path.GetFileNameWithoutExtension(scenarioArray[i]));
        //}

        // Default the game to loading "Scenarios.json"
        scenarioFiles = new string[] { "Scenarios_new" };

        // The below code is for allowing the user to select a scenario file instead of defaulting to Scenarios.json
        //scenarioFiles = scenarioListTrimmed.ToArray();
        //scenarioListTrimmed.Insert(0, "Random");
        //scenarioSelect.AddOptions(scenarioListTrimmed);
        // Load the endings from endings.json
        TextAsset endingsData = Resources.Load("Endings/endings") as TextAsset;
        endings = JsonUtility.FromJson<Endings>(endingsData.text);
        endingHandler = GetComponent<EndingHandler>();
        statSliderThresholds = resultsHandler.statThresholds;
        // Initialize the ResultsHandler's sliders to the correct starting values.
        resultsHandler.Init(stats);
    }


    /// <summary>
    /// Load a specific scenario and its corresponding setups. Only runs at the start.
    /// </summary>
    /// <param name="scenarioID">The ID of the scenario to load</param>
    public Scenarios LoadScenario(int scenarioID)
    {
        // Load the json file. scenarioID is the order of the file in the array, not used anywhere else except here
        TextAsset scenarioData = Resources.Load("Scenarios/" + scenarioFiles[scenarioID]) as TextAsset;
        Scenarios scenarioJson = JsonUtility.FromJson<Scenarios>(scenarioData.text);


        // Fill the valid choices with all the valid indecies 
        for (int i = 0; i < scenarioJson.Setups.Length; ++i)
        {
            validChoices.Add(i);
        }

        // Make sure we won't have more max choices than there actually are.
        if (maxChoices > validChoices.Count)
            maxChoices = validChoices.Count;

        return scenarioJson;
    }

    /// <summary>
    /// Go to the next setup
    /// </summary>
    public void NextSetup()
    {
        print("Next setup");
        //REDACTED by Ein
        // Pick a random choice from the valid ones left
        //int choiceIndex = Random.Range(0, validChoices.Count);

        //Added by Ein
        //select the bottom choice from the valid ones left
        int choiceIndex = 0;

        //Reset the timer. Added by Kyle Grenier
        //timer.Reset();

        //Hide the no selection panel - Kyle Grenier & TJ Caron
        noSelectionPanel.SetActive(false);

        // Set the current setup to the one chosen
        print("Valid choices count: " + validChoices.Count);
        currentSetup = currentScenario.Setups[validChoices[choiceIndex]];

        // If the godzilla setup occured, set the flag so the godzilla ending can occur
        if (currentSetup.ID == 7)
        {
            hadGodzilla = true;
        }

        // Remove the selected choice from the valid list
        validChoices.RemoveAt(choiceIndex);
    }

    /// <summary>
    /// Fired when a choice is made. Adjusts the stats and picks the next setup
    /// </summary>

    int decisionIndex;
    public void ChoiceSelect()
    {
        // Prevent any changes from happening once the max number of choices is reached
        if (choicesMade > maxChoices)
        {
            return;
        }

        // Which choice the players made
        //int decisionIndex = choiceSelect.value;
        decisionIndex = currentSelection;
        print("Decision Index: " + decisionIndex);

        if (decisionIndex < 0)
        {
            // Checks if timer has run out
            if (timer.GetTimeLeft() > 0)
            {
                print("Decision Index less than 0.");
                noSelectionPanel.SetActive(true);
                CancelInvoke("HideNoSelectionPanel");
                Invoke("HideNoSelectionPanel", 2.5f);
            }
            // If player ran out of time deducts stats and moves to next question - TJ
            else
            {

                for (int i = 1; i < stats.Length; i++)
                {
                    stats[i] -= statLoss;
                    stats[i] = Mathf.Clamp(stats[i], 0f, 100f);
                }


                foreach (Button b in choiceButtons)
                {
                    b.GetComponent<Image>().color = Color.white;
                }

                // TODO: Get rid of 4th stat on all scripts - keeping in 4th stat for now 
                // Approval is the average of the 3 other stats.
                stats[0] = (stats[1] + stats[2] + stats[3]) / 3;

                ++choicesMade;


                print("NO CHOICE");
                Transition.instance.StartTransition(NoChoiceSelected);
            }               
        }
        else
        {
            // Below line ties approval into the decision system directly
            //int approvalAdjust = currentSetup.Decisions[decisionIndex].Approval;
            // Set the adjustments for the stats
            float efficiencyAdjust = currentSetup.Decisions[decisionIndex].Efficiency * timer.GetStatMultiplier();
            float envrionmentAdjust = currentSetup.Decisions[decisionIndex].Environment * timer.GetStatMultiplier();
            float costAdjust = currentSetup.Decisions[decisionIndex].Finance * timer.GetStatMultiplier();

            // Keep track of our stats before changing them.
            float[] previousStats = stats.Clone() as float[];

            // Actually update the stats
            stats[1] += efficiencyAdjust;
            stats[2] += envrionmentAdjust;
            stats[3] += costAdjust;

            // Make sure our stats can't go over 100!
            stats[1] = Mathf.Clamp(stats[1], 0f, 100f);
            stats[2] = Mathf.Clamp(stats[2], 0f, 100f);
            stats[3] = Mathf.Clamp(stats[3], 0f, 100f);

            // Resets choice buttons
            // Resets button colors - change later for efficiency
            foreach (Button b in choiceButtons)
            {
                b.GetComponent<Image>().color = Color.white;
            }

            // Approval is the average of the 3 other stats.
            // Right now, we are ignoring the original approval stat in favor of using Environment as Public Approval.
            stats[0] = (stats[1] + stats[2] + stats[3]) / 3;

            // Keep track of the change in stats.
            // This will be sent to the character to judge their emotion.
            statsDelta = new float[stats.Length];
            for (int i = 0; i < statsDelta.Length; ++i)
                statsDelta[i] = stats[i] - previousStats[i];

            ++choicesMade;

            // Changes slider values and color if needed - TJ
            for(int i = 1; i < sliders.Length; i++) 
            {
                sliders[i].value = stats[i] / 100f;
                if (sliders[i].value < (statSliderThresholds[0] / 100f))
                {
                    sliders[i].fillRect.gameObject.GetComponent<Image>().color = Color.red;
                }
                else if (sliders[i].value < (statSliderThresholds[1] / 100f))
                {
                    sliders[i].fillRect.gameObject.GetComponent<Image>().color = Color.yellow;
                }
                else
                {
                    sliders[i].fillRect.gameObject.GetComponent<Image>().color = Color.green;
                }
            }

            
            LeaderboardHandler.instance.IncrementScore("Answer" + currentSetup.ID + ((char)('A' + currentSelection)));
            Transition.instance.StartTransition(FinishChoiceSelect);
        }
    }

    private void FinishChoiceSelect()
    {
        // Hide gameplay screen and display results screen.
        gameplayObject.SetActive(false);
        resultsHandler.gameObject.SetActive(true);
        resultsHandler.Display(stats, currentSetup.Decisions[decisionIndex].Result);

        if (LeaderboardHandler.instance.IsSetup())
        {
            leaderboardStamp.gameObject.SetActive(true);

            int setupID = currentSetup.ID;
            //print("STAMP: char is " + (char)('A' + setupID));
            leaderboardStamp.Display(setupID, "Answer" + setupID + ((char)('A' + currentSelection)));
        }

        // Set the character's emotion based on our current stats.
        character.SetEmotion(statsDelta);
    }

    private void NoChoiceSelected()
    {
        // Hide gameplay screen and display results screen.
        gameplayObject.SetActive(false);
        resultsHandler.gameObject.SetActive(true);
        resultsHandler.Display(stats, "No valid choice was selected!");

        // Make the character shocked because no choice was selected.
        character.SetEmotion(CharacterSprite.Emotion.SHOCKED);
    }


    /// <summary>
    /// Called when pressing the Continue button from the results screen.
    /// </summary>
    public void ContinueFromResults()
    {
        // Disable the results screen and reenable the gameplay screen.
        resultsHandler.gameObject.SetActive(false);

        if (LeaderboardHandler.instance.IsSetup())
            leaderboardStamp.gameObject.SetActive(false);

        gameplayObject.SetActive(true);
        timer.Reset();

        // Sets currentSelection to -1 to make sure player makes a selection before submitting
        currentSelection = -1;

        // If all choices have been made, end the game
        //Kyle Grenier
        if (choicesMade < maxChoices)
        {
            NextSetup();
            UpdateText();
        }
        else
        {
            EndGame();
        }
    }

    /// <summary>
    /// Hides the no selection panel.
    /// </summary>
    private void HideNoSelectionPanel()
    {
        noSelectionPanel.SetActive(false);
    }

    /// <summary>
    /// Update the UI text objects to the current setup
    /// </summary>
    private void UpdateText()
    {
        // Clear old question and choices
        choicesText.text = "";

        // Update the question ID
        setupText.text = "ID: " + currentSetup.Name + "\n" + currentSetup.Setup;

        // Load the background image (the red ones)
        scenarioIcon.sprite = Resources.Load<Sprite>("Icons/" + currentSetup.Icon);

        // Set up a char to increment. By adding 1 to a char, it moves to the next letter (A -> B -> C etc...)
        char currentLetter = 'A';
        int currentText = 0;
        foreach (GameObject obj in choiceButtonRows)
        {
            obj.SetActive(true);
        }

        middleChoiceButtons[0].gameObject.transform.parent.gameObject.SetActive(false);
        middleChoiceButtons[1].gameObject.transform.parent.gameObject.SetActive(false);
        oddNumChoices = false;

        foreach (var choice in currentSetup.Decisions)
        {
            // Set the text with the proper letter prefix
            //choicesText.text += currentLetter + ": " + choice.Choice + "\n";
            choiceTexts[currentText].text = currentLetter + ": " + choice.Choice;

            // Activates any choice buttons that are inactive and will be used
            if (!choiceTexts[currentText].transform.parent.gameObject.activeInHierarchy)
            {
                choiceTexts[currentText].transform.parent.gameObject.SetActive(true);
            }


            // Increment the prefix
            ++currentText;
            ++currentLetter;
        }

        // Checks if there are an odd number of choices
        if (currentText % 2 == 1)
        {
            oddNumChoices = true;

            // Decrements variables to stay on last choice
            currentText--;
            currentLetter--;
            
            // Finds which middle choice button is needed
            int index = (currentText / 2) - 1;
            middleChoiceButtons[index].gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = currentLetter + ": " + currentSetup.Decisions[currentText].Choice;
            middleChoiceButtons[index].gameObject.transform.parent.gameObject.SetActive(true);
            
            // Deactivates the row the middle choice button is replacing
            choiceButtonRows[index + 1].SetActive(false);

            // Deactivates the next row if there are only 3 choices
            if (index + 2 < choiceButtonRows.Length)
            {
                choiceButtonRows[index + 2].SetActive(false);
            }           
        }

        // Sets all unused choice buttons to inactive
        //Kyle Grenier - changed from a hardcoded to value to something more modular.
        while (currentText < choiceTexts.Length)
        {
            choiceTexts[currentText].transform.parent.gameObject.SetActive(false);
            currentText++;
        }

        // Update the stat sliders to show the proper value
        // 0 to 4 is approval, efficiency, envrionment, finance
        //sliders[0].value = stats[0] / 100f;   // At the moment, approval is not being used, and we are replacing environment with public approval.
        sliders[1].value = stats[1] / 100f;
        sliders[2].value = stats[2] / 100f;
        sliders[3].value = stats[3] / 100f;
    }

    /// <summary>
    /// Compare the values against specified thresholds and give an ending
    /// </summary>
    private void EndGame()
    {
        // Turn on the end panel and off the game panel
        endingObject.SetActive(true);
        gameplayObject.SetActive(false);

        // Set the text and sprites to the first ending screen
        endingButton();
    }

    /// <summary>
    /// Load a specific scenario and run it
    /// </summary>
    /// <param name="scenario">The scenario to load. If passed a negative, will load a random scenario</param>
    private void ScenarioSelect(int scenario)
    {
        // If an invalid scenario was passed, pick a random one
        if (scenario < 0)
        {
            scenario = Random.Range(0, scenarioFiles.Length);
        }
        // Load the scenario passed
        currentScenario = LoadScenario(scenario);

        // Ensure the right UI objects are visible
        setupObject.SetActive(false);
        gameplayObject.SetActive(true);

        // Load and display the first setup from the scenario
        NextSetup();
        UpdateText();
    }

    /// <summary>
    /// Fired when the button to select the scenario is pressed. Starts the loading scenario chain
    /// </summary>
    public void ConfirmScenarioSelection(bool fromTitle)
    {
        if (!fromTitle)
        {
            Transition.instance.StartTransition(AfterConfirmScenarioSelection);
        }
        else
        {
            animationHolder.GetComponent<AnimationController>().TitleSequence();
            Invoke("TitleAnimation", 1.5f);
        }
    }

    public void TitleAnimation()
    {
        Transition.instance.StartTransition(AfterConfirmScenarioSelection);
    }

    /// <summary>
    /// Open Up SettingsMenu
    /// Added by Ein
    /// </summary>
    public void SettingMenuToggle()
    {
        settingsPanel.SetActive(true);
 
    }

    /// <summary>
    /// Close SettingsMenu
    /// Added by Ein
    /// </summary>
    public void ReturnToSetupMenuFromSettings()
    {
        settingsPanel.SetActive(false);

    }

    private void AfterConfirmScenarioSelection()
    {
        // Subtracting 1 because Random is the 0-th element
        // Below line is if the user selects the secnario file instead of autoloading Scenarios.json
        //scenarioSelect.value - 1;
        // Autoload Scenarios.json (the first one found)
        if (completedTutorial)
        {
            if (tutorialObject.activeInHierarchy)
            {
                tutorialObject.SetActive(false);
            }
            int selected = 0;
            ScenarioSelect(selected);
        }
        else
        {
            setupObject.SetActive(false);
            tutorialObject.SetActive(true);
        }
    }

    /// <summary>
    /// Loads a scene with the given name
    /// </summary>
    /// <param name="sceneName">Name of the scene to load</param>
    public void LoadScene(string sceneName)
    {
        // This scene has not has godzilla
        hadGodzilla = false;
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Loads the current scene - useful for going back to menu without needing scene name
    /// </summary>
    public void LoadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Quits the game
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Determine the correct ending text and background based on the passed stat
    /// </summary>
    /// <param name="stat">The stat to determine the ending of</param>
    /// <param name="endings">The list of endings to choose from</param>
    /// <param name="backgroundPaths">The list of backgrounds to choose from</param>
    /// <returns>Data about the correct ending for the stat</returns>
    private Ending TestEnding(float stat, string[] endings, string[] backgroundPaths)
    {
        Ending ending;
        // Default to the worst text and background
        ending.text = endings[0];
        ending.backgroundPath = backgroundPaths[0];

        // Temporary alpha code - good ending shows at 50 or above, bad ending at 49 or below - TJ
        if (stat <= 49)
        {
            ending.backgroundPath = backgroundPaths[0];
            ending.text = endings[0];
        }
        else
        {
            ending.backgroundPath = backgroundPaths[2];
            ending.text = endings[2];
        }
        // If the stat is higher than the threshold, set the ending
        // text and background to the one for that threshold
        //for (int i = 0; i < thresholds.Length; i += 3)
        //{
        //    if (stat >= thresholds[i])
        //    {
        //        // Making sure not to go over the ending's length
        //        ending.text = endings[i / 3];
        //        ending.backgroundPath = backgroundPaths[i / 3];
        //    }
        //}

        return ending;
    }
    int endingsSeen = 0;

    /// <summary>
    /// Runs to view the next ending in the sequence. Updates all the text and sprites to the correct ending
    /// </summary>
    public void endingButton()
    {
        //List<string> allEndings = new List<string>();

        // The ending currently being shown
        Ending switcher;
        // Keep track of which endings have been seen already
        if (endingsSeen == 0) // Efficiency
        {
            switcher = TestEnding(stats[1], endings.Efficiency, efficiencyEndingBackgrounds);

        }
        else if (endingsSeen == 1) // Approval
        {
            switcher = TestEnding(stats[2], endings.Approval, approvalEndingBackgrounds);

        }
        else // Finance
        {
            switcher = TestEnding(stats[3], endings.Finance, financeEndingBackgrounds);
            // Last ending - restart/main menu buttons here
        }

        // Display ending
        Sprite endSprite = Resources.Load<Sprite>(switcher.backgroundPath);

        endingHandler.StartNewEnding(switcher.text, endSprite);

        // Increment the number of endings seen
        ++endingsSeen;
        // Add the path to a list. Used for the rare Godzilla ending
        //allEndings.Add(switcher.backgroundPath);
        // If the godzilla setup occured
        //if (hadGodzilla)
        //{
        //    // Add the godzilla ending path to the list. Inserts at index 1 every time
        //    allEndings.Add("Endings/Backgrounds/GodzillaEnd");
        //    // Add the regular path 9 more times, making Godzilla a 1/11 chance
        //    for (int i = 0; i < 9; i++)
        //    {
        //        allEndings.Add(switcher.backgroundPath);
        //    }
        //}

        // Pick what ending to be shown randomly. If godzilla did not appear, this line is redundant
        //int randZilla = Random.Range(0, allEndings.Count);
        // Load the sprite picked above
        //backgroundRenderer.sprite = Resources.Load<Sprite>(allEndings[randZilla]);

        // If godzilla was picked, set the flag to false so he cannot appear again.
        //if (randZilla == 1)
        //{
        //    hadGodzilla = false;
        //}


    }

    /// <summary>
    /// Holds the index of the choice and changes the color of the button when the player
    /// clicks one of the choice buttons.
    /// </summary>
    /// <param name="index">Index of the player's choice</param>
    public void HoldSelection(int index)
    {
        //TODO: Do not let the player select another option if the timer has run out. Added by Kyle Grenier
        //if (timer.Completed)
        //    return;
        //    return;

        //// Hides no selection panel
        //if (noSelectionPanel.activeInHierarchy)
        //{
        //    noSelectionPanel.SetActive(false);
        //}

        // Stores index of player's current selection
        currentSelection = index;

        // Sets all buttons except the one clicked to white, one clicked goes to yellow
        foreach (Button b in choiceButtons)
        {
            b.GetComponent<Image>().color = Color.white;
        }
        foreach(Button b in middleChoiceButtons)
        {
            b.GetComponent<Image>().color = Color.white;
        }

        if (!oddNumChoices)
        {
            choiceButtons[index].GetComponent<Image>().color = chosenColor;
        }
        else
        {
            // First middle button was pressed
            if (index == 2)
            {                
                middleChoiceButtons[0].GetComponent<Image>().color = chosenColor;
            }
            // Second middle button was pressed
            else if (index == 4)
            {
                middleChoiceButtons[1].GetComponent<Image>().color = chosenColor;
            }
            // Neither middle button was pressed
            else
            {
                choiceButtons[index].GetComponent<Image>().color = chosenColor;
            }
        }
        
    }

    /// <summary>
    /// opens stats panel, called when player clicks stat button / Also closes stats panel if it is already open - tj
    /// </summary>
    public void OpenStatsPanel()
    {
        if (!statsPanel.activeInHierarchy)
        {
            statsPanel.SetActive(true);
        }
        else
        {
            statsPanel.SetActive(false);
        }
    }

    /// <summary>
    /// closes stats panel, called when player hits the x button in the stats panel
    /// </summary>
    public void CloseStatsPanel()
    {
        if (statsPanel.activeInHierarchy)
        {
            statsPanel.SetActive(false);
        }
    }

    public void CompleteTutorial()
    {
        completedTutorial = true;
    }
}
