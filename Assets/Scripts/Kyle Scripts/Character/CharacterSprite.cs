/*****************************************************************************
// File Name :         CharacterSprite.cs
// Author :            Kyle Grenier
// Creation Date :     02/19/2021
//
// Brief Description : Controls the emotion associated with the sprite.
*****************************************************************************/
using UnityEngine;

public class CharacterSprite : MonoBehaviour
{
    public enum Emotion {ANY, DEFAULT, CONCERNED, HAPPY, SHOCKED, THINKING};

    // The emotion associated with this sprite.
    [SerializeField] private Emotion emotion = Emotion.DEFAULT;

    public Emotion GetEmotion()
    {
        return emotion;
    }
}