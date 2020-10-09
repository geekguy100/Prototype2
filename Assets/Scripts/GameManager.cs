using System.Collections.Generic;
using System.IO;
using FileLoading;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Tooltip("All the text files for each setup, one per setup. The ID is the files index in the array")]
    public string[] scenarioFiles;

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
    
    #endregion
    
    #region Stat variables

    private int approval = 50;
    private int efficiency = 50;
    private int environment = 50;
    private int cost = 50;

    #endregion

    private void Start()
    {
        string[] scenarioArray = Directory.GetFiles("Assets/Resources/Scenarios", "*.json");
        List<string> scenarioListTrimmed = new List<string>();
        for (int i = 0; i < scenarioArray.Length; ++i)
        {
            scenarioListTrimmed.Add(Path.GetFileNameWithoutExtension(scenarioArray[i]));
        }

        scenarioFiles = scenarioListTrimmed.ToArray();
        scenarioListTrimmed.Insert(0, "Random");
        scenarioSelect.AddOptions(scenarioListTrimmed);

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
    /// <param name="isA">True if choice A was selected, false otherwise</param>
    public void ChoiceSelect(bool isA)
    {
        int approvalAdjust = currentSetup.ApprovalB;
        int efficiencyAdjust = currentSetup.EfficiencyB;
        int envrionmentAdjust = currentSetup.EnvironmentB;
        int costAdjust = currentSetup.CostB;
        
        if (isA)
        {
            approvalAdjust = currentSetup.ApprovalA;
            efficiencyAdjust = currentSetup.EfficiencyA;
            envrionmentAdjust = currentSetup.EnvironmentA;
            costAdjust = currentSetup.CostA;
        }

        approval += approvalAdjust;
        efficiency += efficiencyAdjust;
        environment += envrionmentAdjust;
        cost += costAdjust;
        
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
        setupText.text = currentSetup.Setup;
        choiceAText.text = currentSetup.ChoiceA;
        choiceBText.text = currentSetup.ChoiceB;
    }

    /// <summary>
    /// Compare the values against specified thresholds and give an ending
    /// TODO: Actually put in proper ending logic
    /// </summary>
    private void EndGame()
    {
        Debug.Log(
            $"Ending approval: {approval}\nEnding efficiency: {efficiency}\nEnding envrionment: {environment}\nEnding cost: {cost}");
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
        int selected = scenarioSelect.value - 1;
        ScenarioSelect(selected);
    }
}
