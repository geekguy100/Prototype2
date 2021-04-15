/*****************************************************************************
// File Name :         CharacterEmotionTester.cs
// Author :            Kyle Grenier
// Creation Date :     #CREATIONDATE#
//
// Brief Description : ADD BRIEF DESCRIPTION OF THE FILE HERE
*****************************************************************************/
using UnityEngine;

public class CharacterEmotionTester : MonoBehaviour
{
    [SerializeField] private CharacterSprite.Emotion emotion;
    [SerializeField] private Character character;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            character.SetEmotion(emotion);
    }
}