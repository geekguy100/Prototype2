/*
    Used to store data loaded from the json file (Scenarios.json). These names
    must match EXACTLY to the fields in the json file otherwise Unity will not load
    the data correctly. Class 2/3
*/
namespace FileLoading
{
    [System.Serializable]
    public class Scenario
    {
        // What options the players will have
        public Choices[] Decisions;
        
        // The question to ask
        public string Setup;

        // Question ID. Used to track what questions have been asked already
        public int ID;
        
        // What icon/background art (ones in red) to show while the question is onscreen
        public string Icon;

    }
}