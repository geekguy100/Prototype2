/*****************************************************************************
// File Name :         EndingHandler.cs
// Author :            TJ Caron
// Creation Date :     03/17/2021
//
// Brief Description : Handles showing endings for each stat
*****************************************************************************/
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndingHandler : MonoBehaviour
{
    [Tooltip("Ending Background Image")]
    public Image endBackground;

    [Tooltip("Ending Text Panel Image")]
    public Image textPanel;

    [Tooltip("Ending Text")]
    public TextMeshProUGUI endText;

    [Tooltip("Ending Next button")]
    public GameObject nextButton;

    [Tooltip("Ending Main Menu button")]
    public GameObject menuButton;

    [Tooltip("Ending Quit button")]
    public GameObject quitButton;

    /// <summary>
    /// Number of endings player has seen so far
    /// </summary>
    private int numEndings = 0;

    [Tooltip("Max number of endings")]
    public int maxEndings = 3;

    [Tooltip("Wait time before fading in text panel/buttons")]
    public float waitTime;

    [Tooltip("Time it should take to fade in - in seconds")]
    public float fadeTime;

    [Tooltip("Final color of the text panel")]
    public Color panelTarget;

    /// <summary>
    /// Image on the next button
    /// </summary>
    private Image nextButtonImage;

    /// <summary>
    /// Image on the quit button
    /// </summary>
    private Image quitButtonImage;

    /// <summary>
    /// Image on the menu button
    /// </summary>
    private Image menuButtonImage;

    /// <summary>
    /// Text on the next button
    /// </summary>
    private TextMeshProUGUI nextButtonText;

    /// <summary>
    /// Text on the menu button
    /// </summary>
    private TextMeshProUGUI menuButtonText;

    /// <summary>
    /// Text on the quit button
    /// </summary>
    private TextMeshProUGUI quitButtonText;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        nextButtonImage = nextButton.GetComponent<Image>();
        quitButtonImage = quitButton.GetComponent<Image>();
        menuButtonImage = menuButton.GetComponent<Image>();
        nextButtonText = nextButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        menuButtonText = menuButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        quitButtonText = quitButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    /// <summary>
    /// Starts showing a new ending
    /// </summary>
    /// <param name="text">Text for this ending</param>
    /// <param name="background">Background for this ending</param>
    public void StartNewEnding(string text, Sprite background)
    {
        // Increments number of endings seen
        numEndings++;

        // Sets text panel/buttons to be transparent
        textPanel.color = Color.clear;
        endText.color = Color.clear;
        nextButtonImage.color = Color.clear;
        quitButtonImage.color = Color.clear;
        menuButtonImage.color = Color.clear;
        nextButtonText.color = Color.clear;
        quitButtonText.color = Color.clear;
        menuButtonText.color = Color.clear;

        // Checks if player has seen all the endings
        if (numEndings >= maxEndings)
        {
            nextButton.SetActive(false);
            menuButton.SetActive(true);
            quitButton.SetActive(true);
        }

        // Displays ending
        endText.text = text;
        endBackground.sprite = background;

        // Starts to fade text panel in after wait time
        StartCoroutine("FadeIn");
    }

    /// <summary>
    /// Fades in the text panel and text over time
    /// </summary>
    private IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(waitTime);
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            float normalized = t / fadeTime;
            textPanel.color = Color.Lerp(Color.clear, panelTarget, normalized);
            endText.color = Color.Lerp(Color.clear, Color.black, normalized);
            if (numEndings >= maxEndings)
            {
                quitButtonImage.color = Color.Lerp(Color.clear, Color.white, normalized);
                quitButtonText.color = Color.Lerp(Color.clear, Color.black, normalized);
                menuButtonImage.color = Color.Lerp(Color.clear, Color.white, normalized);
                menuButtonText.color = Color.Lerp(Color.clear, Color.black, normalized);
            }
            else
            {
                nextButtonImage.color = Color.Lerp(Color.clear, Color.white, normalized);
                nextButtonText.color = Color.Lerp(Color.clear, Color.black, normalized);
            }
            yield return null;
        }        
        textPanel.color = panelTarget;
        endText.color = Color.black;      
    }
}
