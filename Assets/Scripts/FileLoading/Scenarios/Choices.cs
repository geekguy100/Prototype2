/*
 * Data about an individual choice from a setup. Loaded from Scenarios.json, Class 3/3
 */
namespace FileLoading
{
    [System.Serializable]
    public class Choices
    {
        // Text of the choice
        public string Choice;
        
        // How the choice changes the stats
        public int Approval;
        public int Efficiency;
        public int Environment;
        public int Finance;
    }
}