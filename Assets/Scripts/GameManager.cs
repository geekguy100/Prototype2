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
    /// Buttons to transition when opening the gameplay screen 
    /// </summary>
    private List<Button> buttonsToHide = new List<Button>();

    [Tooltip("Transition time for gameplay buttons")]
    public float transitionTime = 3f;

    [Tooltip("How long gameplay transition should wait before beginning")]
    public float transitionWaitTime = 3f;

    [Tooltip("Gameplay stats button")]
    public Button statsButton;

    [Tooltip("Highest the stats could add up to")]
    public float maxStats = 300;
    /// <summary>
    /// Holds whether or not the player has completed the tutorial in this play session
    /// </summary>
    public static bool completedTutorial = false;

    [Tooltip("Height used for large choice buttons")]
    public float largeButtonHeight = 68;

    /// <summary>
    /// Y scale used for normal choice buttons
    /// </summary>
    private float normalButtonHeight = 60;

    [Tooltip("Width used for large choice buttons")]
    public float largeButtonWidth = 68;

    /// <summary>
    /// Y scale used for normal choice buttons
    /// </summary>
    private float normalButtonWidth = 280;

    [Tooltip("All the text files for each setup, one per setup. The ID is the files index in the array")]
    private string[] scenarioFiles;

    // The character GameObject currently on the screen.
    private GameObject character;

    // The script used to control the character.
    private Character characterScript;

    public GameObject noSelectionPanel;

    [Tooltip("Panel that holds the stats - opened when player clicks stats button")]
    public GameObject statsPanel;

    [Tooltip("Color that the choice buttons change to when they are chosen")]
    public Color chosenColor;

    [Tooltip("Used for referencing the title animation sequence")]
    public GameObject animationHolder;

    [Tooltip("Reference to script that handles the background music")]
    public MusicHandler musicHandler;

    [Tooltip("The loading menu")]
    public GameObject loadingMenu;

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
    /// Paths to the different backgrounds for the finance endings
    /// </summary>
    private string[] managerEndingBackgrounds =
        {"Endings/Backgrounds/ManagerBad", "Endings/Backgrounds/ManagerGood"};

    private bool threeChoices = false;

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
    public Button submitButton;



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

    [Tooltip("Buttons to track player choice - only show if there are 3 answer")]
    public Button[] threeChoiceButtons;

    [Tooltip("Objects holding each row of choice buttons")]
    public GameObject[] choiceButtonRows;

    [Tooltip("Text on each choice button")]
    public TextMeshProUGUI[] choiceTexts;

    [Tooltip("The leaderboard stamp to display percent of ppl who chose an answer to a question.")]
    public LeaderboardStamp leaderboardStamp;

    //Added by Kyle Grenier
    [Tooltip("The timer used to track time on each question.")]
    public Timer timer;

    [Tooltip("The parent that will hold the characters.")]
    public Transform characterParent;

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
    private float[] stats = { 0, 50, 50, 50 };
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
        Application.targetFrameRate = 300;
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
        // Default the game to loading "Scenarios.json"
        scenarioFiles = new string[] { "Scenarios_new" };

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

        if (character != null)
            Destroy(character);
        else if (characterParent.transform.GetChild(0) != null)
            Destroy(characterParent.transform.GetChild(0).gameObject);
        // Set the current setup to the one chosen
        print("Valid choices count: " + validChoices.Count);
        currentSetup = currentScenario.Setups[validChoices[choiceIndex]];
        string characterName = currentSetup.CharacterName;
        InstantiateCharacter(characterName);

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
                // Adding necessary objects to lists
                foreach (Button b in buttonsToHide)
                {
                    b.interactable = false;
                }

                submitButton.interactable = false;
                statsButton.interactable = false;

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
            // Adding necessary objects to lists
            foreach (Button b in buttonsToHide)
            {
                b.interactable = false;
            }

            submitButton.interactable = false;
            statsButton.interactable = false;

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
        if (characterScript != null)
            characterScript.SetEmotion(statsDelta);
    }

    private void NoChoiceSelected()
    {
        // Hide gameplay screen and display results screen.
        gameplayObject.SetActive(false);
        resultsHandler.gameObject.SetActive(true);
        resultsHandler.Display(stats, "No valid choice was selected!");

        // Make the character shocked because no choice was selected.
        if (character != null)
            characterScript.SetEmotion(CharacterSprite.Emotion.SHOCKED);
    }

    private void InstantiateCharacter(string characterName)
    {
        GameObject prefab = CharacterFactory.GetCharacter(characterName);
        character = Instantiate(prefab, characterParent);
        characterScript = character.GetComponent<Character>();
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
        UpdateMusic(choicesMade < maxChoices);
    }

    /// <summary>
    /// Hides the no selection panel.
    /// </summary>
    private void HideNoSelectionPanel()
    {
        noSelectionPanel.SetActive(false);
    }


    private void UpdateMusic(bool moreQuestions)
    {
        if (moreQuestions)
        {
            AudioClip newMusic = Resources.Load<AudioClip>("Music/" + currentSetup.Music);
            musicHandler.ChangeMusic(newMusic);
        }
        else
        {
            musicHandler.PlayEndingMusic();
        }        
    }

    private void UpdateMusic()
    {
        AudioClip newMusic = Resources.Load<AudioClip>("Music/" + currentSetup.Music);
        musicHandler.ChangeMusic(newMusic);
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


        ScaleButtons(currentSetup.TallButtons, currentSetup.WideButtons);

        if (currentSetup.WideButtons)
        {
            threeChoices = true;
        }
        else
        {
            threeChoices = false;
            foreach (Button b in threeChoiceButtons)
            {
                b.gameObject.transform.parent.gameObject.SetActive(false);
            }
        }
        
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
            if (!threeChoices)
            {
                // Set the text with the proper letter prefix
                //choicesText.text += currentLetter + ": " + choice.Choice + "\n";
                choiceTexts[currentText].text = currentLetter + ": " + choice.Choice;

                // Activates any choice buttons that are inactive and will be used
                if (!choiceTexts[currentText].transform.parent.gameObject.activeInHierarchy)
                {
                    choiceTexts[currentText].transform.parent.gameObject.SetActive(true);
                }

                buttonsToHide.Add(choiceTexts[currentText].transform.parent.gameObject.GetComponent<Button>());
                // Increment the prefix
                ++currentText;
                ++currentLetter;
            }
            else
            {
                foreach (Button b in middleChoiceButtons)
                {
                    b.gameObject.transform.parent.gameObject.SetActive(false);
                }
                foreach (GameObject obj in choiceButtonRows)
                {
                    obj.SetActive(false);
                }
                foreach (Button b in threeChoiceButtons)
                {
                    b.gameObject.transform.parent.gameObject.SetActive(true);
                    buttonsToHide.Add(b.gameObject.GetComponent<Button>());
                }
                threeChoiceButtons[currentText].gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = currentLetter + ": " + choice.Choice;

                // Increment the prefix
                ++currentText;
                ++currentLetter;
            }

        }

        // Checks if there are an odd number of choices
        if (currentText % 2 == 1 && !threeChoices)
        {
            oddNumChoices = true;

            // Decrements variables to stay on last choice
            currentText--;
            currentLetter--;
            
            // Finds which middle choice button is needed
            int index = (currentText / 2) - 1;
            middleChoiceButtons[index].gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = currentLetter + ": " + currentSetup.Decisions[currentText].Choice;
            middleChoiceButtons[index].gameObject.transform.parent.gameObject.SetActive(true);
            buttonsToHide.Add(middleChoiceButtons[index].gameObject.GetComponent<Button>());

            // Deactivates the row the middle choice button is replacing
            choiceButtonRows[index + 1].SetActive(false);

            // Deactivates the next row if there are only 3 choices
            if (index + 2 < choiceButtonRows.Length)
            {
                choiceButtonRows[index + 2].SetActive(false);
            }           
        }
        print(currentText);

        // Turns off last button row if it is unnecessary
        if (currentText < 5)
        {
            choiceButtonRows[choiceButtonRows.Length - 1].SetActive(false);
        }

        // Sets all unused choice buttons to inactive
        //Kyle Grenier - changed from a hardcoded to value to something more modular.
        while (currentText < choiceTexts.Length)
        {
            choiceTexts[currentText].transform.parent.gameObject.SetActive(false);
            currentText++;            
        }

        StartCoroutine(GameplayTransition());

        // Update the stat sliders to show the proper value
        // 0 to 4 is approval, efficiency, envrionment, finance
        //sliders[0].value = stats[0] / 100f;   // At the moment, approval is not being used, and we are replacing environment with public approval.
        sliders[1].value = stats[1] / 100f;
        sliders[2].value = stats[2] / 100f;
        sliders[3].value = stats[3] / 100f;
    }


    private IEnumerator GameplayTransition()
    {
        timer.PauseTimer();
        CanvasGroup timerCG =  timer.gameObject.GetComponent<CanvasGroup>();
        List<Image> imagesToTransition = new List<Image>();
        List<TextMeshProUGUI> textToTransition = new List<TextMeshProUGUI>();

        // Adding necessary objects to lists
        foreach (Button b in buttonsToHide)
        {
            imagesToTransition.Add(b.GetComponent<Image>());
            textToTransition.Add(b.transform.GetChild(0).GetComponent<TextMeshProUGUI>());
            b.interactable = false;
        }
        submitButton.interactable = false;
        statsButton.interactable = false;
        imagesToTransition.Add(statsButton.gameObject.GetComponent<Image>());
        textToTransition.Add(statsButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>());
        imagesToTransition.Add(setupText.transform.parent.GetComponent<Image>());
        textToTransition.Add(setupText);
        imagesToTransition.Add(submitButton.gameObject.GetComponent<Image>());
        textToTransition.Add(submitButton.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>());

        // Making objects transparent
        foreach (Image img in imagesToTransition)
        {
            img.color = Color.clear;
        }
        foreach (TextMeshProUGUI tmp in textToTransition)
        {
            tmp.color = Color.clear;
        }
        timerCG.alpha = 0;

        yield return new WaitForSeconds(transitionWaitTime);

        for (float i = 0; i < transitionTime; i += Time.deltaTime)
        {
            float normalized = i / transitionTime;
            foreach (Image img in imagesToTransition)
            {
                img.color = Color.Lerp(Color.clear, Color.white, normalized);
            }
            foreach(TextMeshProUGUI tmp in textToTransition)
            {
                tmp.color = Color.Lerp(Color.clear, Color.black, normalized);
            }
            timerCG.alpha = Mathf.Lerp(0, 1, normalized);
            yield return null;
        }
        foreach (Image img in imagesToTransition)
        {
            img.color = Color.white;
        }
        foreach (TextMeshProUGUI tmp in textToTransition)
        {
            tmp.color = Color.black;
        }
        foreach (Button b in buttonsToHide)
        {
            b.interactable = true;
        }

        print("TRUE");
        statsButton.interactable = true;
        submitButton.interactable = true;
        timerCG.alpha = 1;
        timer.UnpauseTimer();

        //foreach()
    }
    /// <summary>
    /// Scales choice buttons to match needs for this question
    /// </summary>
    /// <param name="isTall">If the buttons need to be tall</param>
    /// <param name="isWide">If the buttons need to be wide</param>
    private void ScaleButtons(bool isTall, bool isWide)
    {
        // Scales up choice buttons if current setup uses tall choice buttons
        if (isTall)
        {
            foreach (Button obj in choiceButtons)
            {
                obj.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, largeButtonHeight);
            }
        }
        else
        {
            foreach (Button obj in choiceButtons)
            {
                obj.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, normalButtonHeight);
            }
        }

        // Scales up choice buttons if current setup uses tall choice buttons
        if (isWide)
        {
            foreach (Button obj in middleChoiceButtons)
            {
                obj.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, largeButtonWidth);
            }
        }
        else
        {
            foreach (Button obj in middleChoiceButtons)
            {
                obj.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, normalButtonWidth);
            }
        }
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
        UpdateMusic();
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
        Transition.instance.StartTransition(AfterConfirmScenarioSelection, false);
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
        // Default to the best text and background
        ending.text = endings[2];
        ending.backgroundPath = backgroundPaths[2];

        
        for (int i = 0; i < 2; i++)
        {
            if (stat < resultsHandler.statThresholds[i])
            {
                ending.backgroundPath = backgroundPaths[i];
                ending.text = endings[i];
                break;
            }
        }
        //if (stat < resultsHandler.statThresholds[0])
        //{
        //    ending.backgroundPath = backgroundPaths[0];
        //    ending.text = endings[0];
        //}
        //else if (stat < resultsHandler.statThresholds[1])
        //{
        //    ending.backgroundPath = backgroundPaths[1];
        //    ending.text = endings[1];
        //}
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
        else if (endingsSeen == 2)// Finance
        {
            switcher = TestEnding(stats[3], endings.Finance, financeEndingBackgrounds);
            // Last ending - restart/main menu buttons here
        }
        else
        {
            switcher = FinalEnding(stats, endings.Manager, managerEndingBackgrounds);
        }

        // Display ending
        Sprite endSprite = Resources.Load<Sprite>(switcher.backgroundPath);

        endingHandler.StartNewEnding(switcher.text, endSprite);

        // Increment the number of endings seen
        ++endingsSeen;
    }

    private Ending FinalEnding(float[] stats, string[] endings, string[] backgroundPaths)
    {
        Ending ending;
        // Default to the best text and background
        ending.text = endings[1];
        ending.backgroundPath = backgroundPaths[1];

        // adds up all the stats
        float statsTotal = 0;
        foreach(float stat in stats)
        {
            print("stat = " + stat);
            statsTotal += stat;
        }

        // Good ending if player has greater than or equal to half of max possible stats, bad if player has less than half
        if (statsTotal >= (maxStats / 2))
        {
            print("good manager - " + statsTotal);
            ending.text = endings[1];
            ending.backgroundPath = backgroundPaths[1];
        }
        else
        {
            print("bad manager - " + statsTotal);
            ending.text = endings[0];
            ending.backgroundPath = backgroundPaths[0];
        }
        return ending;
    }

    /// <summary>
    /// Holds the index of the choice and changes the color of the button when the player
    /// clicks one of the choice buttons.
    /// </summary>
    /// <param name="index">Index of the player's choice</param>
    public void HoldSelection(int index)
    {
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

        if (threeChoices)
        {
            foreach (Button b in threeChoiceButtons)
            {
                b.GetComponent<Image>().color = Color.white;
            }
            threeChoiceButtons[index].GetComponent<Image>().color = chosenColor;
        }
        else
        {
            if (!oddNumChoices)
            {
                choiceButtons[index].GetComponent<Image>().color = chosenColor;
            }
            else
            {
                // First middle button was pressed
                if (index == 2 && middleChoiceButtons[0].gameObject.activeInHierarchy)
                {
                    middleChoiceButtons[0].GetComponent<Image>().color = chosenColor;
                }
                // Second middle button was pressed
                else if (index == 4 && middleChoiceButtons[1].gameObject.activeInHierarchy)
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

    public void ActivateLoadingMenu()
    {
        Transition.instance.StartTransition(() => {
            loadingMenu.SetActive(true);
            musicHandler.PlayLoadingScreen();
        }, false);
    }
}
