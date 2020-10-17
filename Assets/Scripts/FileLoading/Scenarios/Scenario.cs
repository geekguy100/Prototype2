namespace FileLoading
{
    [System.Serializable]
    public class Scenario
    {
        public Choices[] Decisions;
        public string Setup;

        public int ID;
        public string Icon;
        /*
        public string ChoiceA;
        public string ChoiceB;

        public int ApprovalA;
        public int EfficiencyA;
        public int EnvironmentA;
        public int CostA;
        
        public int ApprovalB;
        public int EfficiencyB;
        public int EnvironmentB;
        public int CostB;
        */
    }
}