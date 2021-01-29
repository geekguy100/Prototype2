/*
    Used to store data loaded from the json file (Scenarios.json). 
    This exists because Unity is slightly dumb with its json support.
    Loads the array that contains the further information about the questions (setups)
    Class 1/3
*/
namespace FileLoading
{
    [System.Serializable]
    public class Scenarios
    {
        public Scenario[] Setups;
    }
}