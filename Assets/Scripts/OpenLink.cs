/*****************************************************************************
// File Name :         OpenLink.cs
// Author :            Kyle Grenier
// Creation Date :     03/04/2021
//
// Brief Description : Opens the player's web browser and goes to the specified URL.
*****************************************************************************/
using UnityEngine;

public class OpenLink : MonoBehaviour
{
    public void OpenURL(string url)
    {
        Application.OpenURL(url);
    }
}