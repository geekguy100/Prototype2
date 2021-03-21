/*****************************************************************************
// File Name :         CharacterFactory.cs
// Author :            Kyle Grenier
// Creation Date :     #CREATIONDATE#
//
// Brief Description : ADD BRIEF DESCRIPTION OF THE FILE HERE
*****************************************************************************/
using UnityEngine;

public static class CharacterFactory
{
    public static GameObject GetCharacter(string characterName)
    {
        GameObject prefab = null;

        // Load the requested character.
        switch (characterName)
        {
            case "Robot":
                prefab = Resources.Load("Characters/Character_Robot") as GameObject;
                break;
            case "Tycoon":
                prefab = Resources.Load("Characters/Character_Tycoon") as GameObject;
                break;
            case "Marketer":
                prefab = Resources.Load("Characters/Character_Marketer") as GameObject;
                break;
            case "Committee":
                prefab = Resources.Load("Characters/Character_Committee") as GameObject;
                break;
            case "Worker":
                prefab = Resources.Load("Characters/Character_Worker") as GameObject;
                break;
            default:
                Debug.LogWarning("CharacterFactory: Character name \'" + characterName + "/' does not exist. Type?");
                return null;
        }

        return prefab;
    }
}