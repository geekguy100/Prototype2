/*****************************************************************************
// File Name :         LeaderboardStamp.cs
// Author :            Kyle Grenier
// Creation Date :     03/18/2021
//
// Brief Description : Displays the percent of players who chose a specific answer for the current question.
*****************************************************************************/
using UnityEngine;
using LeaderboardInfo;
using TMPro;

public class LeaderboardStamp : MonoBehaviour
{
    [SerializeField] private LeaderboardHandler leaderboard;
    private TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    /// <summary>
    /// Displays the percent of players who chose a specific answer for the given question.
    /// </summary>
    /// <param name="questionNumber">The question number.</param>
    /// <param name="answerName">The name of the answer selected in the following format:
    /// Answer#$, where # is the question number and $ is an answer option, typically a letter A - F.</param>
    public void Display(int questionNumber, string answerName)
    {
        float percent = leaderboard.PercentChosen(questionNumber, answerName);
        string tempText = string.Empty;

        if (percent == 100)
            tempText = "You're the first to choose this answer!";
        else
            tempText = percent + "% of people chose this answer!";

        text.text = tempText;
    }
}