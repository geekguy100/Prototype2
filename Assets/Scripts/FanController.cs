/*****************************************************************************
// File Name :         FanController.cs
// Author :            TJ Caron
// Creation Date :     03/18/2021
//
// Brief Description : Turns the fan animation on/off when clicked on
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanController : MonoBehaviour
{
    /// <summary>
    /// Whether or not the fan animation is currently running
    /// </summary>
    private bool isOn = true;

    [Tooltip("Animator on the fan blades object")]
    public Animator bladesAnim;

    [Tooltip("Sound that plays when fan gets turned on")]
    public AudioClip onSound;

    [Tooltip("Sound that plays when fan gets turned off")]
    public AudioClip offSound;

    [Tooltip("SFX audiosource")]
    public AudioSource sfxSource;

    [Tooltip("Question Mark cursor texture")]
    public Texture2D newCursor;

    /// <summary>
    /// Changes the cursor sprite
    /// </summary>
    /// <param name="entering">Whether or not the cursor is entering the fan</param>
    public void ChangeCursor(bool entering)
    {
        if (entering)
        {
            Cursor.SetCursor(newCursor, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }

    /// <summary>
    /// Turns the fan animation off if it is on, turns it on if it is off
    /// </summary>
    public void ClickFan()
    {
        if (isOn)
        {
            isOn = false;
            bladesAnim.SetBool("shouldRun", isOn);
            sfxSource.PlayOneShot(offSound);
        }
        else
        {
            isOn = true;
            bladesAnim.SetBool("shouldRun", isOn);
            sfxSource.PlayOneShot(onSound);
        }
    }
}
