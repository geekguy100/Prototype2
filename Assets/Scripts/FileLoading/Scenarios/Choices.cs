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
        public float Approval;
        public float Efficiency;
        public float Environment;
        public float Finance;
    }
}