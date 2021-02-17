using System.Collections.Generic;
using System.IO;
using FileLoading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Ein
{
    public class GameManager : MonoBehaviour
    {

        [Tooltip("All the text files for each setup, one per setup. The ID is the files index in the array")]
        private string[] scenarioFiles;

        public GameObject noSelectionPanel;

        public GameObject endPanel;
        public GameObject gamePanel;

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

        [Tooltip("The first UI object visible, allows the user to configure the game")]
        public GameObject setupObject;

        [Tooltip("Dropdown to select what scenario to play")]
        public Dropdown scenarioSelect;

        [Tooltip("Dropdown that lets the players choose their option")]
        public Dropdown choiceSelect;

        [Tooltip("Text that displays all the options")]
        public Text choicesText;

        [Tooltip("The object that shows the scenario's icon")]
        public SpriteRenderer scenarioIcon;

        [Tooltip("The SpriteRenderer for the background")]
        public SpriteRenderer backgroundRenderer;

        [Tooltip("Button that shows on the game over screen")]
        public GameObject restartButton;

        [Tooltip("Button for selecting the current choice")]
        public GameObject submitButton;

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

        [Tooltip("Buttons to track player choice")]
        public Button[] choiceButtons;

        [Tooltip("Text on each choice button")]
        public Text[] choiceTexts;

        //Added by Kyle Grenier
        [Tooltip("The timer used to track time on each question.")]
        public Timer timer;


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

        /// <summary>
        /// Stats are in the order approval, efficiency, envrionment, finance
        /// </summary>
        private float[] stats = { 50, 50, 50, 50 };
        [Header("Thresholds for various changes and backgrounds")]

        [Tooltip("Thresholds for the different backgrounds and endings")]
        public int[] thresholds;

        #endregion

        /// <summary>
        /// Subscribe the ChoiceSelect event to OnTimerEnd, meaning ChoiceSelect() will be called once the timer ends.
        /// Added by Kyle Grenier
        /// </summary>
        private void Awake()
        {
            //timer.OnTimerEnd += ChoiceSelect;
        }

        /// <summary>
        /// Unsubscribe to the OnTimerEnd event in the case GameManager is disabled or destroyed.
        /// Added by Kyle Grenier
        /// </summary>
        private void OnDisable()
        {
            //timer.OnTimerEnd -= ChoiceSelect;
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
            scenarioFiles = new string[] { "Scenarios" };

            // The below code is for allowing the user to select a scenario file instead of defaulting to Scenarios.json
            //scenarioFiles = scenarioListTrimmed.ToArray();
            //scenarioListTrimmed.Insert(0, "Random");
            //scenarioSelect.AddOptions(scenarioListTrimmed);
            // Load the endings from endings.json
            TextAsset endingsData = Resources.Load("Endings/endings") as TextAsset;
            endings = JsonUtility.FromJson<Endings>(endingsData.text);
        }


        /// <summary>
        /// Load a specific scenario and its corresponding setups
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

            //Reset the timer. Added by Kyle Grenierf
            timer.Reset();

            //Hide the no selection panel - Kyle Grenier & TJ Caron
            noSelectionPanel.SetActive(false);

            // Set the current setup to the one chosen
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
        public void ChoiceSelect()
        {
            // Prevent any changes from happening once the max number of choices is reached
            if (choicesMade > maxChoices)
            {
                return;
            }

            // Which choice the players made
            //int decisionIndex = choiceSelect.value;
            int decisionIndex = currentSelection;

            if (decisionIndex < 0)
            {
                noSelectionPanel.SetActive(true);
                CancelInvoke("HideNoSelectionPanel");
                Invoke("HideNoSelectionPanel", 2.5f);
            }
            else
            {
                // Below line ties approval into the decision system directly
                //int approvalAdjust = currentSetup.Decisions[decisionIndex].Approval;
                // Set the adjustments for the stats
                float efficiencyAdjust = currentSetup.Decisions[decisionIndex].Efficiency;
                float envrionmentAdjust = currentSetup.Decisions[decisionIndex].Environment;
                float costAdjust = currentSetup.Decisions[decisionIndex].Finance;


                // Actually update the stats
                stats[1] += efficiencyAdjust;
                stats[2] += envrionmentAdjust;
                stats[3] += costAdjust;

                // Resets choice variables/buttons
                // Sets currentSelection to -1 to make sure player makes a selection before submitting
                currentSelection = -1;
                // Resets button colors - change later for efficiency
                foreach (Button b in choiceButtons)
                {
                    b.GetComponent<Image>().color = Color.white;
                }

                // Approval is the average of the 3 other stats.
                stats[0] = (stats[1] + stats[2] + stats[3]) / 3;

                ++choicesMade;
                // If all choices have been made, end the game
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
            choiceSelect.ClearOptions();

            // Update the question ID
            setupText.text = "ID: " + currentSetup.ID + "\n" + currentSetup.Setup;

            // Load the background image (the red ones)
            scenarioIcon.sprite = Resources.Load<Sprite>("Icons/" + currentSetup.Icon);

            // Set up a char to increment. By adding 1 to a char, it moves to the next letter (A -> B -> C etc...)
            char currentLetter = 'A';
            int currentText = 0;
            List<string> availableChoices = new List<string>();
            // Load the dropdown with the choices

            foreach (var choice in currentSetup.Decisions)
            {
                // Set the text with the proper letter prefix
                //choicesText.text += currentLetter + ": " + choice.Choice + "\n";
                choiceTexts[currentText].GetComponent<Text>().text = currentLetter + ": " + choice.Choice;

                // Activates any choice buttons that are inactive and will be used
                if (!choiceTexts[currentText].transform.parent.gameObject.activeInHierarchy)
                {
                    choiceTexts[currentText].transform.parent.gameObject.SetActive(true);
                }

                // Add the choice to the list to be added to the dropdown
                availableChoices.Add(currentText.ToString());
                // Increment the prefix
                ++currentText;
                ++currentLetter;
            }

            // Sets all unused choice buttons to inactive
            while (currentText <= 3)
            {
                choiceTexts[currentText].transform.parent.gameObject.SetActive(false);
                currentText++;
            }


            // Change the persistant background (black one) depending on the values of the stats
            approvalSprite.sprite = UpdateBackground(stats[0], approvalBackgrounds);
            efficiencySprite.sprite = UpdateBackground(stats[1], efficiencyBackgrounds);
            envrionmentSprite.sprite = UpdateBackground(stats[2], environmentBackgrounds);
            financeSprite.sprite = UpdateBackground(stats[3], financeBackgrounds);

            // Add the choices loaded above to the dropdown
            choiceSelect.AddOptions(availableChoices);

            // Update the stat sliders to show the proper value
            // 0 to 4 is approval, efficiency, envrionment, finance
            sliders[0].value = stats[0] / 100f;
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
            endPanel.SetActive(true);
            gamePanel.SetActive(false);
            timer.gameObject.SetActive(false); //Added by Kyle Grenier

            // Turn off all the gameplay UI objects
            choicesText.text = "";
            foreach (Button b in choiceButtons)
            {
                b.gameObject.SetActive(false);
            }
            scenarioIcon.gameObject.SetActive(false);
            choiceSelect.gameObject.SetActive(false);
            submitButton.SetActive(false);
            backgroundStuff.SetActive(false);
            sliderHolder.SetActive(false);

            // Turn on the restart button
            restartButton.SetActive(true);
            // Set the text alignment so it does not run offscreen
            setupText.alignment = TextAnchor.UpperLeft;

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
        public void ConfirmScenarioSelection()
        {
            // Subtracting 1 because Random is the 0-th element
            // Below line is if the user selects the secnario file instead of autoloading Scenarios.json
            //scenarioSelect.value - 1;
            // Autoload Scenarios.json (the first one found)
            int selected = 0;
            ScenarioSelect(selected);
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
        /// Quits the game
        /// </summary>
        public void ExitGame()
        {
            Application.Quit();
        }

        /// <summary>
        /// Find the correct background for the given stat value
        /// </summary>
        /// <param name="stat">Which stat to find the background for</param>
        /// <param name="sprites">The sprites to select a background from</param>
        /// <returns>The appropriate background for the stat</returns>
        private Sprite UpdateBackground(float stat, List<Sprite> sprites)
        {
            // Default to the background to the lowest index, the worst one
            Sprite background = sprites[0];

            // Compare the stat to the designated thresholds. If above, change the background
            // to be shown to the one just checked
            for (int i = 0; i < sprites.Count; ++i)
            {
                if (stat >= thresholds[i])
                {
                    background = sprites[i];
                }
            }

            return background;
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

            // If the stat is higher than the threshold, set the ending
            // text and background to the one for that threshold
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

        /// <summary>
        /// Runs to view the next ending in the sequence. Updates all the text and sprites to the correct ending
        /// </summary>
        public void endingButton()
        {
            List<string> allEndings = new List<string>();
            //set setup text to text that needs to be shown, and set ending background to background that needs to be shown
            if (endingsSeen > 3)
            {
                LoadScene("SampleScene");
            }

            // The ending currently being shown
            Ending switcher;
            // Keep track of which endings have been seen already
            if (endingsSeen == 0)//approval
            {
                switcher = TestEnding(stats[0], endings.Approval, approvalEndingBackgrounds);
            }
            else if (endingsSeen == 1)//efficiency
            {
                switcher = TestEnding(stats[1], endings.Efficiency, efficiencyEndingBackgrounds);
            }
            else if (endingsSeen == 2)//environment
            {
                switcher = TestEnding(stats[2], endings.Envrionment, envrionmentEndingBackgrounds);
            }
            else//finance
            {
                switcher = TestEnding(stats[3], endings.Finance, financeEndingBackgrounds);
                restartButton.transform.GetChild(0).GetComponent<Text>().text = "Restart Game";
            }
            setupText.text = switcher.text;

            // Increment the number of endings seen
            ++endingsSeen;
            // Add the path to a list. Used for the rare Godzilla ending
            allEndings.Add(switcher.backgroundPath);
            // If the godzilla setup occured
            if (hadGodzilla)
            {
                // Add the godzilla ending path to the list. Inserts at index 1 every time
                allEndings.Add("Endings/Backgrounds/GodzillaEnd");
                // Add the regular path 9 more times, making Godzilla a 1/11 chance
                for (int i = 0; i < 9; i++)
                {
                    allEndings.Add(switcher.backgroundPath);
                }
            }

            // Pick what ending to be shown randomly. If godzilla did not appear, this line is redundant
            int randZilla = Random.Range(0, allEndings.Count);
            // Load the sprite picked above
            backgroundRenderer.sprite = Resources.Load<Sprite>(allEndings[randZilla]);

            // If godzilla was picked, set the flag to false so he cannot appear again.
            if (randZilla == 1)
            {
                hadGodzilla = false;
            }


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

            Debug.Log(index);
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
            choiceButtons[index].GetComponent<Image>().color = Color.yellow;
        }


    }
}
