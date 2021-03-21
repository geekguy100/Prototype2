/*****************************************************************************
// File Name :         AnimationController.cs
// Author :            Ashley Wielgos
// Creation Date :     03/06/2021
//
// Brief Description : Holder for animation scripts.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    Animator animator;
    public int roombaPath;

    [Header("Animators")]
    [Tooltip("Animator for title transparency")]
    public Animator titleTransparency;

    [Tooltip("Animator for title zoom")]
    public Animator titleZoom;

    [Tooltip("Animator for title sides")]
    public Animator titleSides;

    [Tooltip("Animator for title start button")]
    public Animator titleStart;

    [Tooltip("Animator for title settings button")]
    public Animator titleSettings;

    [Tooltip("Animator for title credits button")]
    public Animator titleCredits;

    [Tooltip("Animator for title exit button")]
    public Animator titleExit;

    [Tooltip("Animator for title character movement")]
    public Animator titleCharacter;

    [Tooltip("Animator for title character movement")]
    public Animator titleText;

    [Tooltip("Animator for title start button")]
    public Animator titleStartText;

    [Tooltip("Animator for title settings button")]
    public Animator titleSettingsText;

    [Tooltip("Animator for title credits button")]
    public Animator titleCreditsText;

    [Tooltip("Animator for title exit button")]
    public Animator titleExitText;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        if (roombaPath == 1)
        {
            animator.SetBool("Tutorial", true);
        }
        else if (roombaPath == 2)
        {
            animator.SetBool("Conference", true);
        }
    }

    public void TitleSequence()
    {
        titleZoom.SetBool("TitleZoom", true);
        titleSides.SetBool("TitleSides", true);
        titleTransparency.SetBool("TitleTransparent", true);
        //titleCharacter.SetBool("TitleCharacter", true);
        titleStart.SetBool("TitleButton", true);
        titleSettings.SetBool("TitleButton", true);
        titleCredits.SetBool("TitleButton", true);
        titleExit.SetBool("TitleButton", true);
        titleText.SetBool("TitleText", true);
        titleStartText.SetBool("ButtonText", true);
        titleSettingsText.SetBool("ButtonText", true);
        titleCreditsText.SetBool("ButtonText", true);
        titleExitText.SetBool("ButtonText", true);
    }
}
