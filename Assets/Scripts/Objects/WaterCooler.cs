using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterCooler : Interactable
{
    [Tooltip("Sound to play when bubble animation starts")]
    public AudioClip bubbleSound;

    [Tooltip("Sound to play when bubble animation ends")]
    public AudioClip bubbleEndSound;

    /// <summary>
    /// Animator controlling bubble animation
    /// </summary>
    private Animator bubbleAnim;

    /// <summary>
    /// Whether or not animation is currently playing
    /// </summary>
    private bool isPlaying = true;

    private void Start()
    {
        bubbleAnim = GetComponent<Animator>();
        bubbleAnim.keepAnimatorControllerStateOnDisable = true;
    }
   
    public void Interact()
    {
        // Change cursor to revealed one if it wasn't already
        if (!revealed)
        {
            revealed = true;
            ChangeCursor(revealed);
        }
        // Play animation if stopped, stop animation if playing
        print("isPlaying = " + isPlaying);
        print("animRunning = " + bubbleAnim.GetBool("playBubbles"));
        isPlaying = !isPlaying;
        bubbleAnim.SetBool("playBubbles", isPlaying);

        if (isPlaying)
        {
            PlaySFX(bubbleSound);
        }
        else
        {
            PlaySFX(bubbleEndSound);
        }
    }

}
