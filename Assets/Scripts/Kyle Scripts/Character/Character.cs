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


    [Header("Expression GameObjects attached to the Character.")]
    [Tooltip("The character's face.")]
    [SerializeField] private GameObject face = null;

    [Tooltip("The character's left arm.")]
    [SerializeField] private GameObject leftArm = null;

    [Tooltip("The character's right arm.")]
    [SerializeField] private GameObject rightArm = null;

    [Tooltip("The character's antenna.")]
    [SerializeField] private GameObject antenna = null;


    private void Start()
    {
        SetEmotion(CharacterSprite.Emotion.DEFAULT);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SetEmotion(CharacterSprite.Emotion.ANY);
    }

    /// <summary>
    /// Updates the character's sprites to those of the given emotion.
    /// </summary>
    /// <param name="emotion">The emotion of the character.</param>
    public void SetEmotion(CharacterSprite.Emotion emotion)
    {
        UpdateSprite(emotion, ref face, faces);
        UpdateSprite(emotion, ref leftArm, leftArms);
        UpdateSprite(emotion, ref rightArm, rightArms);
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