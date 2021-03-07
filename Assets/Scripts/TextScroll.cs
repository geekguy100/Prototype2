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
    /// <summary>
    /// Number of characters per second that show up on screen
    /// </summary>
    private float charPerSeconds = 15;

    [Tooltip("The audio source to play the typewriter sound effect")]
    public AudioSource typewriterSource;

    [Tooltip("The next text to scroll on this panel if there is one")]
    public GameObject text2;

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

    void Start()
    {
        // Sets initial values
        thisText = GetComponent<TextMeshProUGUI>();
        targetString = thisText.text;
        thisText.text = "";
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
            }
            if (!started && gameObject.activeInHierarchy)
            {
                started = true;
                StartScroll();
            }
        }
    }
    /// <summary>
    /// Starts text scrolling on this textbox
    /// </summary>
    public void StartScroll()
    {
        StartCoroutine(Scroll());
    }

    /// <summary>
    /// Resets text scrolling on this textbox if it hasn't completed
    /// </summary>
    public void ResetScroll()
    {
        if (!completed)
        {
            typewriterSource.Stop();
            started = false;
            thisText.text = "";
            if (text2 != null)
            {
                text2.GetComponent<TextScroll>().ResetScroll();
                text2.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Finishes text scrolling on this textbox
    /// </summary>
    public void FinishScroll()
    {
        thisText.text = targetString;
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
}
