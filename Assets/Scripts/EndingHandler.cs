/*****************************************************************************
// File Name :         EndingHandler.cs
// Author :            TJ Caron
// Creation Date :     03/17/2021
//
// Brief Description : Handles showing endings for each stat
*****************************************************************************/
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
    public GameObject mainMenuButton;

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

    // Start is called before the first frame update
    void Start()
    {
        nextButtonImage = nextButton.GetComponent<Image>();
        quitButtonImage = quitButton.GetComponent<Image>();
        menuButtonImage = mainMenuButton.GetComponent<Image>();
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
        textPanel.color = new Color(textPanel.color.r, textPanel.color.g, textPanel.color.g, 0);
        endText.color = new Color(endText.color.r, endText.color.g, endText.color.g, 0);

        // Checks if player has seen all the endings
        if (numEndings >= maxEndings)
        {
            nextButton.SetActive(false);
            mainMenuButton.SetActive(true);
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
    private void FadeIn()
    {
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            float normalized = t / fadeTime;
            textPanel.color = Color.Lerp(Color.clear, Color.white, normalized);
            endText.color = Color.Lerp(Color.clear, Color.black, normalized);
        }
        textPanel.color = Color.white;
        endText.color = Color.white;
        float alpha = 0;
        bool finished = false;
        while (!finished)
        {
            alpha += Time.deltaTime * (1 / fadeTime);
            if (alpha > 1)
            {
                alpha = 1;
                finished = true;
            }
            textPanel.color = new Color(textPanel.color.r, textPanel.color.g, textPanel.color.g, alpha);
            endText.color = new Color(endText.color.r, endText.color.g, endText.color.g, alpha);
        }
    }
}
