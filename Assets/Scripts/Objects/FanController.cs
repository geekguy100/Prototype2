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

public class FanController : Interactable
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

    /// <summary>
    /// Turns the fan animation off if it is on, turns it on if it is off
    /// </summary>
    public void Interact()
    {
        if (isOn)
        {
            isOn = false;
            bladesAnim.SetBool("shouldRun", isOn);
            PlaySFX(offSound);
        }
        else
        {
            isOn = true;
            bladesAnim.SetBool("shouldRun", isOn);
            PlaySFX(onSound);
        }
    }
}
