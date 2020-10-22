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
    
    [Tooltip("What is the maximum number of choices the players will have")]
    public int maxChoices = 12;

    /// <summary>
    /// How many choices the players have made so far
    /// </summary>
    private int choicesMade;
    
    #endregion
    
    #region GameObjects

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
    public Sprite approvalSprite;
    
    [Tooltip("Sprite showing the current state of effienency")]
    public Sprite efficiencySprite;
    
    [Tooltip("Sprite showing the current state of the environment")]
    public Sprite envrionmentSprite;
    
    [Tooltip("Sprite showing the current state of finance")]
    public Sprite financeSprite;
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
    #endregion
    
    #region Stat variables

    private int approval = 50;
    private int efficiency = 50;
    private int environment = 50;
    private int cost = 50;

    [Tooltip("Stats above this give the good ending")]
    public int goodThreshold = 75;

    [Tooltip("Stats above this, but below the good threshold, are given the neutral ending")]
    public int neutralThreshold = 25;
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
        
        
        efficiency += efficiencyAdjust;
        environment += envrionmentAdjust;
        cost += costAdjust;
        approval += (efficiency + environment + cost) / 3;
        
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

        approvalSprite = UpdateBackground(approval, approvalBackgrounds);
        efficiencySprite = UpdateBackground(efficiency, efficiencyBackgrounds);
        envrionmentSprite = UpdateBackground(environment, environmentBackgrounds);
        financeSprite = UpdateBackground(cost, financeBackgrounds);
        
        choiceSelect.AddOptions(availableChoices);
        /*
        choiceAText.text = currentSetup.ChoiceA;
        choiceBText.text = currentSetup.ChoiceB;
        */
    }

    /// <summary>
    /// Compare the values against specified thresholds and give an ending
    /// </summary>
    private void EndGame()
    {
        string endingText = "";
        List<string> endingBackgrounds = new List<string>();
        // I have it, make it better
        // TODO: Better way of doing this. Maybe reformat json?
        if (approval > goodThreshold)
        {
            endingText += endings.Good[0].Approval + "\n\n";
            endingBackgrounds.Add("Endings/Backgrounds/ApprovalGood");
        } 
        else if (approval > neutralThreshold)
        {
            endingText += endings.Neutral[0].Approval + "\n\n";
            endingBackgrounds.Add("Endings/Backgrounds/ApprovalNeutral");
        }
        else
        {
            endingText += endings.Bad[0].Approval + "\n\n";
            endingBackgrounds.Add("Endings/Backgrounds/ApprovalBad");
        }
        
        if (efficiency > goodThreshold)
        {
            endingText += endings.Good[0].Efficiency + "\n\n";
            endingBackgrounds.Add("Endings/Backgrounds/EfficiencyGood");
        } 
        else if (approval > neutralThreshold)
        {
            endingText += endings.Neutral[0].Efficiency + "\n\n";
            endingBackgrounds.Add("Endings/Backgrounds/EfficiencyNeutral");
        }
        else
        {
            endingText += endings.Bad[0].Efficiency + "\n\n";
            endingBackgrounds.Add("Endings/Backgrounds/EfficiencyBad");
        }

        if (environment > goodThreshold)
        {
            endingText += endings.Good[0].Environment + "\n\n";
            endingBackgrounds.Add("Endings/Backgrounds/EnvironmentGood");
        } 
        else if (approval > neutralThreshold)
        {
            endingText += endings.Neutral[0].Environment + "\n\n";
            endingBackgrounds.Add("Endings/Backgrounds/EnvironmentNeutral");
        }
        else
        {
            endingText += endings.Bad[0].Environment + "\n\n";
            endingBackgrounds.Add("Endings/Backgrounds/EnvironmentBad");
        }
        
        if (cost > goodThreshold)
        {
            endingText += endings.Good[0].Finance + "\n\n";
            endingBackgrounds.Add("Endings/Backgrounds/FinanceGood");
        } 
        else if (approval > neutralThreshold)
        {
            endingText += endings.Neutral[0].Finance + "\n\n";
            endingBackgrounds.Add("Endings/Backgrounds/FinanceNeutral");
        }
        else
        {
            endingText += endings.Bad[0].Finance + "\n\n";
            endingBackgrounds.Add("Endings/Backgrounds/FinanceBad");
        }

        int spriteIndex = Random.Range(0, endingBackgrounds.Count - 1);
        Debug.Log($"Loading background {endingBackgrounds[spriteIndex]}");
        backgroundRenderer.sprite = Resources.Load<Sprite>(endingBackgrounds[spriteIndex]);
        
        choicesText.text = "";
        scenarioIcon.gameObject.SetActive(false);
        choiceSelect.gameObject.SetActive(false);
        choiceButton.SetActive(false);
        restartButton.SetActive(true);
        setupText.alignment = TextAnchor.UpperLeft;
        setupText.text = endingText;
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
        Sprite background;
        if (stat > goodThreshold)
        {
            background = sprites[2];
        } else if (stat > neutralThreshold)
        {
            background = sprites[1];
        }
        else
        {
            background = sprites[0];
        }

        return background;
    }
}
