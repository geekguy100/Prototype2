using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacetrackController : Interactable
{
    [Tooltip("Audio clip to play when bad approval poster animates")]
    public AudioClip playClip;

    Animator anim;
    public void Interact()
    {
        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }
        // Change cursor to revealed one if it wasn't already
        if (!revealed)
        {
            revealed = true;
            ChangeCursor(revealed);
        }
        PlaySFX(playClip);
        anim.SetTrigger("ShouldRun");
    }
}
