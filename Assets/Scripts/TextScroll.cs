/*****************************************************************************
// File Name :         TextScroll.cs
// Author :            TJ Caron
// Creation Date :     03/07/2021
//
// Brief Description : Has text slowly appear over time for tutorial
*****************************************************************************/
using System.Collections;
using UnityEngine;
using TMPro;

public class TextScroll : MonoBehaviour
{
    public Tutorial tutorial;

    public bool isGameplayTutorial = false;

    public bool toGameplay = false;
    /// <summary>
    /// Number of characters per second that show up on screen
    /// </summary>
    private float charPerSeconds = 16;

    [Tooltip("The audio source to play the typewriter sound effect")]
    public AudioSource typewriterSource;

    [Tooltip("The audio source to play when the player skips the typing effect")]
    public AudioSource dingSource;

    [Tooltip("Button click audio source")]
    public AudioSource buttonSource;

    [Tooltip("The audio clip to play when the player skips the typing effect")]
    public AudioClip dingClip;

    [Tooltip("The next text to scroll on this panel if there is one")]
    public GameObject text2;

    /// <summary>
    /// If this textscroll already advanced the tutorial once
    /// </summary>
    private bool sent = false;

    /// <summary>
    /// TextScroll script on text2 object
    /// </summary>
    private TextScroll text2Scroll;

    /// <summary>
    /// The TextMeshProUGUI on this game object
    /// </summary>
    private TextMeshProUGUI thisText;

    /// <summary>
    /// What the text should say after it completes scrolling
    /// </summary>
    private string targetString;

    /// <summary>
    /// Whether or not the scroll has completed
    /// </summary>
    private bool completed = false;

    /// <summary>
    /// Whether or not the scroll has started
    /// </summary>
    private bool started = false;

    /// <summary>
    /// Whether or not values have been initialized
    /// </summary>
    private bool initialized = false;

    void Start()
    {
        // Sets initial values
        InitializeValues();
    }

    void Update()
    {
        // Allows player to complete scrolling manually using space
        if (!completed)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                completed = true;
                thisText.text = targetString;
                typewriterSource.Stop();
                dingSource.PlayOneShot(dingClip);
            }
            if (!started && gameObject.activeInHierarchy)
            {
                started = true;
                StartScroll();
            }
        }
        else
        {
            if (text2 == null)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    AdvanceTutorial();
                }
            }
            else if (text2Scroll.completed)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    AdvanceTutorial();
                }
            }
        }
    }
 
    /// <summary>
    /// Stops the typewriter sounds from playing
    /// </summary>
    public void StopTypeWriter()
    {
        typewriterSource.Stop();
    }
    /// <summary>
    /// Starts text scrolling on this textbox
    /// </summary>
    public void StartScroll()
    {
        if (!initialized)
        {
            InitializeValues();
        }
        StartCoroutine(Scroll());
    }

    /// <summary>
    /// Resets text scrolling on this textbox if it hasn't completed
    /// </summary>
    public void ResetScroll()
    {
        if (text2 != null)
        {
            text2.GetComponent<TextScroll>().ResetScroll();
        }
        if (!completed && started)
        {
            typewriterSource.Stop();
            started = false;
            if (!initialized)
            {
                InitializeValues();
            }
            else
            {
                thisText.text = "";
            }

            if (text2 != null)
            {
                text2.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Finishes text scrolling on this textbox
    /// </summary>
    public void FinishScroll()
    {
        typewriterSource.Stop();
        thisText.text = targetString;
        completed = true;
        if (text2 != null)
        {
            text2.GetComponent<TextScroll>().FinishScroll();
        }
    }

    /// <summary>
    /// Starts text appearing slowly over time
    /// </summary>
    /// <returns></returns>
    private IEnumerator Scroll()
    {
        // Creates initial variables
        string currentText = "";
        float timer = 0;
        int counter = 0;
        typewriterSource.Play();
        while (!completed)
        {
            // Increases timer
            timer += Time.deltaTime * charPerSeconds;
            // Checks if another character should be added
            if (counter < timer)
            {
                // Adds a character
                currentText += targetString[counter];
                thisText.text = currentText;
                counter++;
                // Checks if string has been completed
                if (counter >= targetString.Length)
                {
                    completed = true;
                }
            }
            yield return 0;
        }
        // If there is another text on this panel, starts its scroll
        if (text2 != null)
        {
            text2.SetActive(true);
        }
        typewriterSource.Stop();
    }

    /// <summary>
    /// Initialized necessary values 
    /// </summary>
    private void InitializeValues()
    {
        thisText = GetComponent<TextMeshProUGUI>();
        targetString = thisText.text;
        thisText.text = "";
        initialized = true;
        if (text2 != null)
        {
            text2Scroll = text2.GetComponent<TextScroll>();
        }
    }
    public void GoBack()
    {
        sent = false;
    }
    private void AdvanceTutorial()
    {
        if (!isGameplayTutorial && !sent)
        {
            tutorial.NextStep();
            sent = true;
            buttonSource.Play();
        }
        else if (toGameplay && !sent)
        {
            tutorial.StartInteractiveTutorial();
            buttonSource.Play();
            sent = true;
        }
    }
}
