/*****************************************************************************
// File Name :         Character.cs
// Author :            Kyle Grenier
// Creation Date :     02/19/2021
//
// Brief Description : Controls the emotion of the on screen character.
*****************************************************************************/
using UnityEngine;
using System.Collections.Generic;



public class Character : MonoBehaviour
{
    [Header("Expression Prefabs")]
    [Tooltip("The faces to display on the character.")]
    [SerializeField] private CharacterSprite[] faces = null;

    [Tooltip("The left arms to display on the character.")]
    [SerializeField] private CharacterSprite[] leftArms = null;

    [Tooltip("The right arms to display on the character.")]
    [SerializeField] private CharacterSprite[] rightArms = null;

    [Tooltip("The antennas to display on the character.")]
    [SerializeField] private CharacterSprite[] antennas = null;

    [Tooltip("The bows to display on the character.")]
    [SerializeField] private CharacterSprite[] bows = null;


    [Header("Expression GameObjects attached to the Character.")]
    [Tooltip("The character's face.")]
    [SerializeField] private GameObject face = null;

    [Tooltip("The character's left arm.")]
    [SerializeField] private GameObject leftArm = null;

    [Tooltip("The character's right arm.")]
    [SerializeField] private GameObject rightArm = null;

    [Tooltip("The character's antenna.")]
    [SerializeField] private GameObject antenna = null;

    [Tooltip("The character's bow")]
    [SerializeField] private GameObject bow = null;


    [Header("Emotion Selection Algorithm")]
    [SerializeField] private EMOTION_ALGORITHM selectedAlgorithm = EMOTION_ALGORITHM.STAT_BIAS;
    private enum EMOTION_ALGORITHM { STAT_BIAS, CONSIDER_ALL, MIX };

    [Header("CONSIDER_ALL Emotion Settings")]
    [Tooltip("Used with the CONSIDER_ALL or MIX algorithm option. This value determine by how much" +
        " a stat must increase or decrease for the character to be happy or shocked respectively.")]
    [SerializeField] private int majorEmotionThreshold = 6;

    [Tooltip("Used with the random emotion-selection algorithm option. If the majorEmotionThreshold is not reached, this " +
        "value determines how many stats must decrease in order for the character to be CONCERNED.")]
    [SerializeField] private int statDecreaseThreshold = 2;

    [Header("STAT_BIAS Emotion Settings")]
    [Tooltip("The index of stat that this character looks for changes in.")]
    [Range(1, 3)]
    [SerializeField] private int associatedEmotionIndex;

    [SerializeField] private int happyThreshold = 8;
    [SerializeField] private int shockedThreshold = -8;

    [Tooltip("The max amount a stat needs to increase or decrease by for the character to be thinking.")]
    [SerializeField] private int thinkingThreshold = 4;


    // DEFAULT, HAPPY, CONCERNED, SHOCKED, THINKING
    // When I say 'change' I mean the absolute value of the change in the stat.
    // If the change is between 0 and thinkingThreshold, make the character THINKING.
    // If the change is between (thinkingThreshold + 1) and (happyShockedThreshold - 1), make the character CONCERNED.
    // If the change is greater than happyShockedThreshold, make the character HAPPY or SHOCKED (depending on a negative or positive change).
    private void Start()
    {
        SetEmotion(CharacterSprite.Emotion.DEFAULT);
    }

    /// <summary>
    /// Sets the character's emotion based on the player's change in stats.
    /// </summary>
    /// <param name="statsDelta">The change is stats since the last scenario was completed.</param>
    public void SetEmotion(float[] statsDelta)
    {
        if (selectedAlgorithm == EMOTION_ALGORITHM.CONSIDER_ALL)
        {
            SetEmotion_OLD(statsDelta);
            return;
        }

        // Decide at runtime which algorithm to use to set the emotion of the character.
        // If we're using a random algorithm, that is.
        if (selectedAlgorithm == EMOTION_ALGORITHM.MIX)
        {
            int randomNum = Random.Range(1, 3);
            if (randomNum == 1)
            {
                SetEmotion_OLD(statsDelta);
                return;
            }
        }

        float delta = statsDelta[associatedEmotionIndex];
        //print("STAT DELTA INDEX: " + associatedEmotionIndex + ": CHANGE = " + delta);

        if (delta < 0)
        {
            if (delta <= shockedThreshold)
                SetEmotion(CharacterSprite.Emotion.SHOCKED);
            else if (delta < -thinkingThreshold)
                SetEmotion(CharacterSprite.Emotion.CONCERNED);
            else
                SetEmotion(CharacterSprite.Emotion.THINKING);
        }
        else
        {
            if (delta >= happyThreshold)
                SetEmotion(CharacterSprite.Emotion.HAPPY);
            else if (delta > thinkingThreshold)
                SetEmotion(CharacterSprite.Emotion.CONCERNED);
            else
                SetEmotion(CharacterSprite.Emotion.THINKING);
        }
    }

    /// <summary>
    /// Sets the character's emotion based on the player's change in stats.
    /// This algorithm looks at all of the stats to determine an emotion.
    /// </summary>
    /// <param name="statsDelta">The change is stats since the last scenario was completed.</param>
    public void SetEmotion_OLD(float[] statsDelta)
    {
        int statsDecreased = 0;

        for (int i = 1; i < statsDelta.Length; ++i)
        {
            // If any of the stats hit the threshold, set the appropriate emotion and return.
            if (statsDelta[i] <= -majorEmotionThreshold)
            {
                print("CHARACTER: " + "Hit SHOCKED threshold, meaning a stat decreased by " + majorEmotionThreshold + " or more.");
                SetEmotion(CharacterSprite.Emotion.SHOCKED);
                return;
            }
            else if (statsDelta[i] >= majorEmotionThreshold)
            {
                print("CHARACTER: " + "Hit HAPPY threshold, meaning a stat increased by " + majorEmotionThreshold + " or more.");
                SetEmotion(CharacterSprite.Emotion.HAPPY);
                return;
            }
            else if (statsDelta[i] < 0)
                ++statsDecreased;
        }

        // If we get to this point, there were no drastic stat changes.


        // If no stats decreased, pick the HAPPY emotion.
        if (statsDecreased == 0)
        {
            print("CHARACTER: " + "0 stats decreased, so character is HAPPY.");
            SetEmotion(CharacterSprite.Emotion.HAPPY);
        }

        // If there are more than or an equal amount of stats decreased
        // compared to our statDecreased threshold, pick the CONCERNED emotion.
        else if (statsDecreased >= 2)
        {
            print("CHARACTER: " + "2 or more stats decreased, so character is CONCERNED.");
            SetEmotion(CharacterSprite.Emotion.CONCERNED);
        }

        // If some stats decreased, pick the THINKING emotion.
        else
        {
            print("CHARACTER: " + " a stat decreased, so character is THINKING.");
            SetEmotion(CharacterSprite.Emotion.THINKING);
        }
    }

    /// <summary>
    /// Updates the character's emotion to that of the given emotion.
    /// </summary>
    /// <param name="emotion">The emotion of the character.</param>
    public void SetEmotion(CharacterSprite.Emotion emotion)
    {
        if (bow != null)
            UpdateSprite(emotion, ref bow, bows);
        if (face != null)
            UpdateSprite(emotion, ref face, faces);
        if (leftArm != null)
            UpdateSprite(emotion, ref leftArm, leftArms);
        if (rightArm != null)
            UpdateSprite(emotion, ref rightArm, rightArms);
        if (antenna != null)
            UpdateSprite(emotion, ref antenna, antennas);
    }

    /// <summary>
    /// Updates a given sprite to a new one from a list of possibilities.
    /// </summary>
    /// <param name="emotion">The desired emotion of the given sprite.</param>
    /// <param name="expressionContainer">The GameObject holding the sprite to update.</param>
    /// <param name="possibilities">An array of sprites. A sprite from this array will be chosen if it matches the given emotion.</param>
    private void UpdateSprite(CharacterSprite.Emotion emotion, ref GameObject expressionContainer, CharacterSprite[] possibilities)
    {
        GameObject chosen = null;


        if (emotion == CharacterSprite.Emotion.ANY)
        {
            chosen = possibilities[Random.Range(0, possibilities.Length)].gameObject;
        }
        else
        {
            // Compile a list of faces with the desired emotion.
            List<CharacterSprite> chosenExperssions = new List<CharacterSprite>();
            foreach (CharacterSprite possibility in possibilities)
            {
                if (possibility.GetEmotion() == emotion || possibility.GetEmotion() == CharacterSprite.Emotion.ANY)
                    chosenExperssions.Add(possibility);
            }

            chosen = chosenExperssions[Random.Range(0, chosenExperssions.Count)].gameObject;
        }

        Destroy(expressionContainer);
        expressionContainer = Instantiate(chosen, transform);
    }
}

