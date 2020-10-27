using System.Collections.Generic;
using System.IO;
using FileLoading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Tooltip("All the text files for each setup, one per setup. The ID is the files index in the array")]
    private string[] scenarioFiles;

    #region Choice Tracking
    [Header("Tracking and changing how many choices the players have")]
    [Tooltip("What is the maximum number of choices the players will have")]
    public int maxChoices = 12;

    /// <summary>
    /// How many choices the players have made so far
    /// </summary>
    private int choicesMade;

    /// <summary>
    /// Paths to the different backgrounds for the approval endings
    /// </summary>
    private string[] approvalEndingBackgrounds = 
        {"Endings/Backgrounds/ApprovalBad", "Endings/Backgrounds/ApprovalNeutral", "Endings/Backgrounds/ApprovalGood"};

    /// <summary>
    /// Paths to the different backgrounds for the efficiency endings
    /// </summary>
    private string[]efficiencyEndingBackgrounds = 
    {
        "Endings/Backgrounds/EfficiencyBad", "Endings/Backgrounds/EfficiencyNeutral",
        "Endings/Backgrounds/EfficiencyGood"
    };

    /// <summary>
    /// Paths to the different backgrounds for the envrionment endings
    /// </summary>
    private string[] envrionmentEndingBackgrounds = 
    {
        "Endings/Backgrounds/EnvironmentBad", "Endings/Backgrounds/EnvironmentNeutral",
        "Endings/Backgrounds/EnvironmentGood"
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
    public Text setupText;
    
    [Tooltip("The text to show option A")]
    public Text choiceAText;
    
    [Tooltip("The text to show option B")]
    public Text choiceBText;
    
    [Tooltip("The parent of all the above objects. Used to turn them on and off")]
    public GameObject gameplayObject;

    public GameObject setupObject;
    
    [Tooltip("Dropdown to select what scenario to play")]
    public Dropdown scenarioSelect;

    [Tooltip("Dropdown that lets the players choose their option")]
    public Dropdown choiceSelect;

    [Tooltip("Text that displays all the options")]
    public Text choicesText;
    
    [Tooltip("The object that shows the scenario's icon")]
    public Image scenarioIcon;

    [Tooltip("The SpriteRenderer for the background")]
    public SpriteRenderer backgroundRenderer;

    [Tooltip("Button that shows on the game over screen")]
    public GameObject restartButton;

    [Tooltip("Button for selecting the current choice")]
    public GameObject choiceButton;
    
    [Tooltip("Backgrounds for approval, lower indecies are worse")]
    public List<Sprite> approvalBackgrounds = new List<Sprite>();
    
    [Tooltip("Backgrounds for efficnency, lower indecies are worse")]
    public List<Sprite> efficiencyBackgrounds = new List<Sprite>();
    
    [Tooltip("Backgrounds for envrionment, lower indecies are worse")]
    public List<Sprite> environmentBackgrounds = new List<Sprite>();
    
    [Tooltip("Backgrounds for finance, lower indecies are worse")]
    public List<Sprite> financeBackgrounds = new List<Sprite>();
    
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
    #endregion

    #region Stat variables

    /*
    private int approval = 50;
    private int efficiency = 50;
    private int environment = 50;
    private int finance = 50;
    */
    private int[] stats = { 50, 50, 50, 50 };
    [Header("Thresholds for various changes and backgrounds")]

    [Tooltip("Thresholds for the different backgrounds and endings")]
    public int[] thresholds;
    
    #endregion

    private void Start()
    {
        //string[] scenarioArray = Directory.GetFiles("Assets/Resources/Scenarios", "*.json");
        //List<string> scenarioListTrimmed = new List<string>();
        //for (int i = 0; i < scenarioArray.Length; ++i)
        //{
        //    scenarioListTrimmed.Add(Path.GetFileNameWithoutExtension(scenarioArray[i]));
        //}
        
        scenarioFiles = new string[] {"Scenarios"};
        
        //scenarioFiles = scenarioListTrimmed.ToArray();
        //scenarioListTrimmed.Insert(0, "Random");
        //scenarioSelect.AddOptions(scenarioListTrimmed);
        TextAsset endingsData = Resources.Load("Endings/endings") as TextAsset;
        endings = JsonUtility.FromJson<Endings>(endingsData.text);
    }
    
    
    /// <summary>
    /// Load a specific scenario and its corresponding setups
    /// </summary>
    /// <param name="scenarioID">The ID of the scenario to load</param>
    public Scenarios LoadScenario(int scenarioID)
    {
        TextAsset scenarioData = Resources.Load("Scenarios/" + scenarioFiles[scenarioID]) as TextAsset;
        Scenarios scenarioJson = JsonUtility.FromJson<Scenarios>(scenarioData.text);
        
        
        // Fill the valid choices with all the valid indecies 
        for (int i = 0; i < scenarioJson.Setups.Length; ++i)
        {
            validChoices.Add(i);
        }
        
        return scenarioJson;
    }

    public void NextSetup()
    {
        // Pick a random choice from the valid ones left
        int choiceIndex = Random.Range(0, validChoices.Count);

        currentSetup = currentScenario.Setups[validChoices[choiceIndex]];

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
    public void ChoiceSelect()
    {
        // Prevent any changes from happening once the max number of choices is reached
        if (choicesMade > maxChoices)
        {
            return;
        }

        int decisionIndex = choiceSelect.value;
        //int approvalAdjust = currentSetup.Decisions[decisionIndex].Approval;
        int efficiencyAdjust = currentSetup.Decisions[decisionIndex].Efficiency;
        int envrionmentAdjust = currentSetup.Decisions[decisionIndex].Environment;
        int costAdjust = currentSetup.Decisions[decisionIndex].Finance;
        
        
        stats[1] += efficiencyAdjust;
        stats[2] += envrionmentAdjust;
        stats[3] += costAdjust;
        stats[0] = (stats[1] + stats[2] + stats[3]) / 3;
        
        ++choicesMade;
        if (choicesMade <= maxChoices)
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
    /// Update the UI text objects to the current setup
    /// </summary>
    private void UpdateText()
    {
        choicesText.text = "";
        choiceSelect.ClearOptions();
        setupText.text = "ID: " + currentSetup.ID + "\n" + currentSetup.Setup;
        scenarioIcon.sprite = Resources.Load<Sprite>("Icons/" + currentSetup.Icon);
        char currentLetter = 'A';
        List<string> availableChoices = new List<string>();
        foreach (var choice in currentSetup.Decisions)
        {
            choicesText.text += currentLetter + ": " + choice.Choice + "\n";
            availableChoices.Add(currentLetter.ToString());
            ++currentLetter;
        }

        approvalSprite.sprite = UpdateBackground(stats[0], approvalBackgrounds);
        efficiencySprite.sprite = UpdateBackground(stats[1], efficiencyBackgrounds);
        envrionmentSprite.sprite = UpdateBackground(stats[2], environmentBackgrounds);
        financeSprite.sprite = UpdateBackground(stats[3], financeBackgrounds);
        
        choiceSelect.AddOptions(availableChoices);
        /*
        choiceAText.text = currentSetup.ChoiceA;
        choiceBText.text = currentSetup.ChoiceB;
        */
        sliders[0].value = stats[0]/100f;
        sliders[1].value = stats[1]/100f;
        sliders[2].value = stats[2]/100f;
        sliders[3].value = stats[3]/100f;
    }

    /// <summary>
    /// Compare the values against specified thresholds and give an ending
    /// </summary>
    private void EndGame()
    {
        /*
        string endingText = "";
        List<string> endingBackgrounds = new List<string>();
        
        // TODO: Don't repeat the same code with small changes
        Ending approvalEnd = TestEnding(stats[0], endings.Approval, approvalEndingBackgrounds);
        Ending efficiencyEnd = TestEnding(stats[1], endings.Efficiency, efficiencyEndingBackgrounds);
        Ending environmentEnd = TestEnding(stats[2], endings.Envrionment, envrionmentEndingBackgrounds);
        Ending financeEnd = TestEnding(stats[3], endings.Finance, financeEndingBackgrounds);

        endingText += approvalEnd.text + "\n\n";
        endingText += efficiencyEnd.text + "\n\n";
        endingText += environmentEnd.text + "\n\n";
        endingText += financeEnd.text;
        
        endingBackgrounds.Add(approvalEnd.backgroundPath);
        endingBackgrounds.Add(efficiencyEnd.backgroundPath);
        endingBackgrounds.Add(environmentEnd.backgroundPath);
        endingBackgrounds.Add(financeEnd.backgroundPath);
        if (hadGozilla)
        {
            endingBackgrounds.Add("Endings/Backgrounds/GodzillaEnd");
        }
        int spriteIndex = Random.Range(0, endingBackgrounds.Count);
        Debug.Log($"Loading background {endingBackgrounds[spriteIndex]}");
        backgroundRenderer.sprite = Resources.Load<Sprite>(endingBackgrounds[spriteIndex]);
        */
        choicesText.text = "";
        scenarioIcon.gameObject.SetActive(false);
        choiceSelect.gameObject.SetActive(false);
        choiceButton.SetActive(false);
        restartButton.SetActive(true);
        backgroundStuff.SetActive(false);
        sliderHolder.SetActive(false);
        setupText.alignment = TextAnchor.UpperLeft;
        //setupText.text = endingText;
        endingButton();
    }

    /// <summary>
    /// Load a specific scenario and run it
    /// </summary>
    /// <param name="scenario">The scenario to load. If passed a negative, will load a random scenario</param>
    private void ScenarioSelect(int scenario)
    {
        if (scenario < 0)
        {
            scenario = Random.Range(0, scenarioFiles.Length);
        }
        currentScenario = LoadScenario(scenario);
        setupObject.SetActive(false);
        gameplayObject.SetActive(true);
        NextSetup();
        UpdateText();
    }

    /// <summary>
    /// Fired when the button to select the scenario is pressed. Starts the loading scenario chain
    /// </summary>
    public void ConfirmScenarioSelection()
    {
        // Subtracting 1 because Random is the 0-th element
        int selected = 0;//scenarioSelect.value - 1;
        ScenarioSelect(selected);
    }

    /// <summary>
    /// Loads a scene with the given name
    /// </summary>
    /// <param name="sceneName">Name of the scene to load</param>
    public void LoadScene(string sceneName)
    {
        hadGodzilla = false;
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Quits the game
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
    }

    private Sprite UpdateBackground(int stat, List<Sprite> sprites)
    {
        Sprite background = sprites[0];

        for (int i = 0; i < sprites.Count; ++i)
        {
            if (stat >= thresholds[i])
            {
                background = sprites[i];
            }
        }

        return background;
    }

    private Ending TestEnding(int stat, string[] endings, string[] backgroundPaths)
    {
        Ending ending;
        ending.text = endings[0];
        ending.backgroundPath = backgroundPaths[0];

        for (int i = 0; i < thresholds.Length; i += 3)
        {
            if (stat >= thresholds[i])
            {
                // Making sure not to go over the ending's length
                ending.text = endings[i / 3];
                ending.backgroundPath = backgroundPaths[i / 3];
            }
        }

        return ending;
    }
    int endingsSeen = 0;

    public void endingButton()
    {
        List<string> allEndings = new List<string>();
        //set setup text to text that needs to be shown, and set ending background to background that needs to be shown
        if(endingsSeen > 3)
        {
            LoadScene("SampleScene");
        }
        Ending switcher;
        if (endingsSeen == 0)//approval
        {
            switcher = TestEnding(stats[0], endings.Approval, approvalEndingBackgrounds);
        }
        else if(endingsSeen == 1)//efficiency
        {
            switcher = TestEnding(stats[1], endings.Efficiency, efficiencyEndingBackgrounds);
        }
        else if(endingsSeen == 2)//environment
        {
            switcher = TestEnding(stats[2], endings.Envrionment, envrionmentEndingBackgrounds);
        }
        else//finance
        {
            switcher = TestEnding(stats[3], endings.Finance, financeEndingBackgrounds);
            restartButton.transform.GetChild(0).GetComponent<Text>().text = "Restart Game";
        }
        setupText.text = switcher.text;
        ++endingsSeen;
        allEndings.Add(switcher.backgroundPath);
        if(hadGodzilla)
        {
            allEndings.Add("Endings/Backgrounds/GodzillaEnd");
           for(int i = 0; i < 4; i ++)
            {
                allEndings.Add(switcher.backgroundPath);
            }
        }

        int randZilla = Random.Range(0, allEndings.Count);
        backgroundRenderer.sprite = Resources.Load<Sprite>(allEndings[randZilla]);
        if(randZilla == 1)
        {
            hadGodzilla = false;
        }
       
        
    }
}
