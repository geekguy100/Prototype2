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

    [Tooltip("Sound that plays when fan breaks")]
    public AudioClip breakSound;

    [Tooltip("How long fan will track interactions to determine if it breaks")]
    public float interactTimer = 5f;

    [Tooltip("How long fan will be broken")]
    public float breakTime = 2f;

    [Tooltip("Number of times the fan must be clicked in breakTime for fan to break")]
    public int breakCount = 5;

    [Tooltip("Particles to play when fan breaks")]
    public GameObject brokenParticles;


    /// <summary>
    /// Whether or not the fan is currently counting number of interactions
    /// </summary>
    private bool counting = false;

    /// <summary>
    /// Whether or not the fan is currently broken
    /// </summary>
    private bool broken = false;

    /// <summary>
    /// Number of times player has clicked fan during current breakTime timer
    /// </summary>
    [SerializeField]private int numInteractions = 0;



    private void Start()
    {
        bladesAnim.keepAnimatorControllerStateOnDisable = true;
    }

    /// <summary>
    /// Turns the fan animation off if it is on, turns it on if it is off
    /// </summary>
    public void Interact() 
    {
        // Checks if fan is currently broken
        if (!broken)
        {
            // If fan hasnt started counting interactions, start counting
            if (!counting)
            {
                counting = true;
                StartCoroutine(CountInteractions());
                numInteractions = 1;
            }
            // If it already started, count this interaction
            else
            {
                numInteractions++;
            }
            // Change cursor to revealed one if it wasn't already
            if (!revealed)
            {
                revealed = true;
                ChangeCursor(revealed);
            }
            // Turn fan animation on/off
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
    

    private IEnumerator CountInteractions()
    {
        float timer = 0;
        while (counting)
        {
            // Check if timer should be counting
            if (timer < breakTime)
            {
                timer += Time.deltaTime;
            }
            else
            {
                counting = false;
            }
            // Checks if fan should be broken because player clicked on it too much
            if (numInteractions > breakCount)
            {
                // Turns fan off if it is on
                if (isOn)
                {
                    isOn = false;
                    bladesAnim.SetBool("shouldRun", isOn);
                }
                counting = false;
                broken = true;
                numInteractions = 0;
                StartCoroutine(BreakFan());
            }
            yield return null;
        }
    }
    private IEnumerator BreakFan()
    {
        // Plays break sound and spawns particles
        PlaySFX(breakSound);
        float timer = 0;
        GameObject particles = Instantiate(brokenParticles, bladesAnim.gameObject.transform);
        while (broken)
        {
            // Fan will be broken for however long breakTime is
            if (timer < breakTime)
            {
                timer += Time.deltaTime;
            }
            else
            {
                broken = false;
            }
            yield return null;
        }
        // Destroys particles when fan is no longer broken
        Destroy(particles);
    }
}
