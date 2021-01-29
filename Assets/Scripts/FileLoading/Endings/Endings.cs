/*
 * Loaded from endings.json
 * Contains the text for the 4 different endings, one per stat.
 */
namespace FileLoading
{
    [System.Serializable]
    public class Endings
    {
        // Endings in json stored in the order, top to bottom: bad, neutral, good
        // This corresponds to indecies 0, 1, and 2 respectivly
        public string[] Approval;
        public string[] Efficiency;
        public string[] Envrionment;
        public string[] Finance;
        
    }
}