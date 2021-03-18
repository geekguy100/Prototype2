/*****************************************************************************
// File Name :         LeaderboardHandler.cs
// Author :            Kyle Grenier
// Creation Date :     03/11/2021
//
// Brief Description : Handles obtaining the leaderboard data from Dreamlo and acts as an interface to 
                       obtain information from the leaderboard.
*****************************************************************************/
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Linq;

namespace LeaderboardInfo
{
    public class LeaderboardHandler : MonoBehaviour
    {
        private LeaderboardParent leaderboardParent = null;

        // The URLs to get and set the leaderboard data.
        private const string DREAMLO_JSON_URL = "http://dreamlo.com/lb/6047df3d8f40bcbd0c98abd1/json";
        private const string DREAMLO_UPLOAD_URL = "http://dreamlo.com/lb/0wVN5OvU0kGNYvjswMb4IwZcUws04Z70meMj0R4wXT-A/add/";
        private const string DREAMLO_REMOVE_URL = "http://dreamlo.com/lb/0wVN5OvU0kGNYvjswMb4IwZcUws04Z70meMj0R4wXT-A/delete/";

        // Start by getting the leaderboard data.
        private void Start()
        {
            StartCoroutine(GetData());
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                IncrementScore("Answer1A");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                IncrementScore("Answer1B");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                IncrementScore("Answer1C");
            }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                StartCoroutine(PushScore("Answer1A"));
            }
                //Debug.Log("Sum of question 1: " + ParticipantsForQuestion(1));
            else if (Input.GetKeyDown(KeyCode.U))
            {
                StartCoroutine(PushScore("Answer1A"));
            }
            else if (Input.GetKeyDown(KeyCode.B))
            {
                Debug.Log((PercentChosen(1, "Answer1B")) + "%");
            }
        }

        #region ---- Leaderboard Getters ----
        /// <summary>
        /// Populates the Leaderboard with all available leaderboard entries.
        /// </summary>
        private IEnumerator GetData()
        {
            // Start a web request to retrieve the JSON leaderboard data.
            UnityWebRequest www = new UnityWebRequest(DREAMLO_JSON_URL);
            www.downloadHandler = new DownloadHandlerBuffer();

            yield return www.SendWebRequest();

            // If an error occured, log it.
            if (www.isNetworkError || www.isHttpError)
                Debug.LogError(www.error);

            // No error occured, so assign our Dreamlo object.
            else
            {
                Debug.Log("LEADERBOARD: Successfully received data!");
                leaderboardParent = JsonUtility.FromJson<LeaderboardParent>(www.downloadHandler.text);

                if (leaderboardParent == null)
                    Debug.LogError("LEADERBOARD: Could not assign leaderboard data to the leaderboardParent object.");
            }
        }


        /// <summary>
        /// Get all leaderboard entries sorted in alphabetical order by name (e.g., Answer1A, Answer1B, etc).
        /// </summary>
        /// <returns>An array of LeaderboardEntry objects sorted in alphabetical order by name.</returns>
        public LeaderboardEntry[] GetSortedEntries()
        {
            if (leaderboardParent == null)
                return null;

            return leaderboardParent.dreamlo.leaderboard.entry
                .OrderBy(t => t.name)
                .ToArray();
        }

        /// <summary>
        /// Get all leaderboard entries as they appear in dreamlo.
        /// </summary>
        /// <returns>An array of LeaderboardEntry objects in no particular order.</returns>
        public LeaderboardEntry[] GetEntries()
        {
            if (leaderboardParent == null)
                return null;
            return leaderboardParent.dreamlo.leaderboard.entry;
        }

        /// <summary>
        /// Gets the specific leaderboard entry with the given name.
        /// </summary>
        /// <param name="answerName">The name of the leaderboard entry to retrieve.</param>
        /// <returns>The LeaderboardEntry object with the given name.</returns>
        public LeaderboardEntry GetEntryByName(string answerName)
        {
            if (leaderboardParent == null)
                return null;

            return leaderboardParent.dreamlo.leaderboard.entry
                .Where(t => t.name == answerName)
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets the total number of people who answered the given question.
        /// </summary>
        /// <param name="questionNumber">The question number.</param>
        /// <returns>The total number of people who answered the given question.</returns>
        public int ParticipantsForQuestion(int questionNumber)
        {
            // First, we get all of the entries that contain the 
            // question number we are looking for.
            // Then we sum the scores and return the sum.
            int sum = GetEntries()
                .Where(t => t.name.Contains(questionNumber.ToString()))
                .Sum(t => t.score);

            return sum;
        }

        /// <summary>
        /// Gets the percent of people who chose the given answer.
        /// </summary>
        /// <param name="questionNumber">The question number.</param>
        /// <param name="answerName">The name of the answer selected in the following format:
        /// Answer#$, where # is the question number and $ is an answer option, typically a letter A - F.</param>
        public float PercentChosen(int questionNumber, string answerName)
        {
            LeaderboardEntry entry = GetEntryByName(answerName);
            if (entry == null)
            {
                Debug.LogWarning("LEADERBOARD: Cannot get the percent chosen because answer '" + answerName + "' is non-existant...");
                return 0;
            }

            float participantsForQuestion = ParticipantsForQuestion(questionNumber);
            float participantsForAnswer = entry.score;
           

            // The percent of people who chose the same answer as the player.
            float percentChosen = Mathf.Round((participantsForAnswer / participantsForQuestion) * 100f);
            return percentChosen;
        }

        #endregion

        #region ---- Leaderboard Setters ----

        /// <summary>
        /// Increments the score of an answer by 1;
        /// Occurs when a player has selected this answer.
        /// </summary>
        /// <param name="answerName">The name of the answer selected in the following format:
        /// Answer#$, where # is the question number and $ is an answer option, typically a letter A - F.</param>
        public void IncrementScore(string answerName)
        {
            if (leaderboardParent == null)
                return;

            LeaderboardEntry entry = GetEntryByName(answerName);

            if (entry == null)
            {
                Debug.LogWarning("LEADERBOARD: Cannot increment the score of entry '" + answerName + "' for it does not exist...");
                return;
            }
            else
            {
                ++entry.score;
                Debug.Log("LEADERBOARD: Entry '" + entry.name + "' score changed from " + (entry.score - 1) + " to " + entry.score);
                StartCoroutine(PushScore(answerName));
            }
 
        }

        /// <summary>
        /// Pushes the score of the leaderboard entry with the given name to the dreamlo server. 
        /// Only pre-existing scores will be updated, meaning you must provide an existing answer name.
        /// </summary>
        /// <param name="answerName">The name of the leaderboard entry in the following format:
        /// Answer#$, where # is the question number and $ is an answer option, typically a letter A - F.</param>
        public IEnumerator PushScore(string answerName)
        {
            LeaderboardEntry entry = GetEntryByName(answerName);

            // Make sure we have a valid answerName so we don't add a new score to the leaderboard;
            // we only want to be updating existing ones.
            if (entry == null)
            {
                Debug.LogWarning("LEADERBOARD: Cannot push a score with an answerName of '" + answerName + "'" +
                    "for it does not already exist on the leaderboard.");

                yield break;
            }

            // Start a web request to retrieve the JSON leaderboard data.
            UnityWebRequest www = new UnityWebRequest(DREAMLO_UPLOAD_URL + entry.name + "/" + entry.score);
            www.downloadHandler = new DownloadHandlerBuffer();

            yield return www.SendWebRequest();

            // If an error occured, log it.
            if (www.isNetworkError || www.isHttpError)
                Debug.LogError(www.error);

            // No error occured, meaning we successfully pushed our score.
            else
                Debug.Log("LEADERBOARD: Successfully pushed leaderboard entry: " + entry.name + " -> " + entry.score);
        }

        /// <summary>
        /// Mainly used for debugging purposes. Resets a score to 0 by deleting it and re-adding it to the leaderboard.
        /// </summary>
        /// <param name="answerName">The name of the leaderboard entry in the following format:
        /// Answer#$, where # is the question number and $ is an answer option, typically a letter A - F.</param>
        public IEnumerator ResetScore(string answerName)
        {
            LeaderboardEntry entry = GetEntryByName(answerName);

            // Make sure we have a valid answerName so we don't add a new score to the leaderboard;
            // we only want to be updating existing ones.
            if (entry == null)
            {
                Debug.LogWarning("LEADERBOARD: Cannot remove a score with an answerName of '" + answerName + "'" +
                    "for it does not already exist on the leaderboard.");

                yield break;
            }

            // Start a web request to retrieve the JSON leaderboard data.
            UnityWebRequest www = new UnityWebRequest(DREAMLO_REMOVE_URL + entry.name);
            www.downloadHandler = new DownloadHandlerBuffer();

            yield return www.SendWebRequest();

            // If an error occured, log it and break out of this coroutine.
            if (www.isNetworkError || www.isHttpError)
                Debug.LogError(www.error);

            // If we successfully deleted that account, 
            // let's first set the entry's score to 0, then we'll re-add it to the 
            // online leaderboard so we can use it again.
            else
            {
                Debug.Log("LEADERBOARD: Successfully removed entry '" + entry.name + "'. Reseting score to 0 and pushing...");
                entry.score = 0;
                StartCoroutine(PushScore(entry.name));
            }
        }

        #endregion
    }
}