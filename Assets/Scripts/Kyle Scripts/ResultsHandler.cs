/*****************************************************************************
// File Name :         ResultsHandler.cs
// Author :            Kyle Grenier
// Creation Date :     02/18/2021
//
// Brief Description : Handles displaying the results after each question.
*****************************************************************************/
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class ResultsHandler : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider prSlider = null;
    [SerializeField] private Slider efficiencySlider = null;
    [SerializeField] private Slider environmentSlider = null;
    [SerializeField] private Slider financeSlider = null;
    private float[] stats;
    private int slider = 0; // The slider to animate.

    [Header("Animation Settings")]
    [Tooltip("The speed of the slider change animation.")]
    [SerializeField] private float animationSpeed = 1f;

    [Header("Results Text")]
    [Tooltip("Text to hold the result from the most recent scenario.")]
    [SerializeField] private TextMeshProUGUI resultsText;


    /// <summary>
    /// Sets the sliders to the correct initial values.
    /// </summary>
    /// <param name="stats"></param>
    public void Init(float[] stats)
    {
        prSlider.value = stats[0] / 100f;
        efficiencySlider.value = stats[1] / 100f;
        environmentSlider.value = stats[2] / 100f;
        financeSlider.value = stats[3] / 100f;
    }

    /// <summary>
    /// Display the results screen.
    /// </summary>
    /// <param name="stats">The updated stats.</param>
    public void Display(float[] stats, string result)
    {
        this.stats = stats;

        resultsText.text = result;

        AnimateSlider();
    }

    /// <summary>
    /// Handles calling the coroutine to animate updating the slider values.
    /// </summary>
    /// <param name="stats">The updated stats.</param>
    private void AnimateSlider()
    {
        switch (slider)
        {
            case 0:
                StartCoroutine(AnimateSliderCoroutine(prSlider, stats[0] / 100f));
                break;
            case 1:
                StartCoroutine(AnimateSliderCoroutine(efficiencySlider, stats[1]/100f));
                break;
            case 2:
                StartCoroutine(AnimateSliderCoroutine(environmentSlider, stats[2] / 100f));
                break;
            case 3:
                StartCoroutine(AnimateSliderCoroutine(financeSlider, stats[3] / 100f));
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Animates a slider to a new value.
    /// </summary>
    /// <param name="slider">The slider to animate.</param>
    /// <param name="newValue">The value to animate the slider to.</param>
    /// <returns></returns>
    private IEnumerator AnimateSliderCoroutine(Slider slider, float newValue)
    {
        while (Mathf.Abs(slider.value - newValue) > 0.05f)
        {
            slider.value = Mathf.Lerp(slider.value, newValue, Time.deltaTime * animationSpeed);
            yield return null;
        }

        ++this.slider;
        AnimateSlider();
    }

    public void Continue()
    {
        slider = 0;
    }

}