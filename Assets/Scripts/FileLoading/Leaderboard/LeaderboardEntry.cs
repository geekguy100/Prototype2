/*****************************************************************************
// File Name :         LeaderboardEntry.cs
// Author :            Kyle Grenier
// Creation Date :     03/11/2021
//
// Brief Description : A single entry on the leaderboard.
*****************************************************************************/

namespace LeaderboardInfo
{
    [System.Serializable]
    public class LeaderboardEntry
    {
        /// <summary>
        /// The name of the answer (Answer1D, Answer3C, etc.)
        /// </summary>
        public string name;

        /// <summary>
        /// The number of people who chose this answer.
        /// </summary>
        public int score;
    }
}