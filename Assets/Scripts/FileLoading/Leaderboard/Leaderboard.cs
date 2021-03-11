/*****************************************************************************
// File Name :         Leaderboard.cs
// Author :            Kyle Grenier
// Creation Date :     03/11/2021
//
// Brief Description : Manages getting information from and pushing information to the dreamlo leaderboard.
*****************************************************************************/

using System;
using System.Linq;

namespace LeaderboardInfo
{
    [System.Serializable]
    public class Leaderboard
    {
        public LeaderboardEntry[] entry;
    }
}