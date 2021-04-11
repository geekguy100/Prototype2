/*****************************************************************************
// File Name :         ResultsHandler.cs
// Author :            Kyle Grenier
// Creation Date :     02/18/2021
//
// Brief Description : Handles displaying the results after each question.
*****************************************************************************/
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections;
using TMPro;


public class ResultsHandler : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider efficiencySlider = null;
    [SerializeField] private Slider approvalSlider = null;
    [SerializeField] private Slider financeSlider = null;
    [SerializeField] private Slider efficiencyBackground = null;
    [SerializeField] private Slider approvalBackground = null;
    [SerializeField] private Slider financeBackground = null;

    //added by Ein. These are the sliders in the loading menu
    [SerializeField] private Slider efficiencySliderLoading = null;
    [SerializeField] private Slider approvalSliderLoading = null;
    [SerializeField] private Slider financeSliderLoading = null;

    [Header("Percent Change Texts")]
    [SerializeField] private TextMeshProUGUI efficiencyText = null;
    [SerializeField] private TextMeshProUGUI approvalText = null;
    [SerializeField] private TextMeshProUGUI financeText = null;

    private float[] stats;
    private int slider = 1; // The slider to animate.

    [Header("Animation Settings")]
    [Tooltip("The time it takes to complete the animation of the slider.")]
    [SerializeField] private float animationTime = 1f;

    [Tooltip("The time to wait in seconds before the sliders animate.")]
    [SerializeField] private float animationDelayTime = 0f;

    [Header("Results Text")]
    [Tooltip("Text to hold the result from the most recent scenario.")]
    [SerializeField] private TextMeshProUGUI resultsText = null;

    [Header("Stat Variables and Objects")]
    [Tooltip("Text to hold the result from the most recent scenario.")]
    public float[] statThresholds;

    [Tooltip("Efficiency Background objects")]
    public GameObject[] efficiencyObjs;

    [Tooltip("Low Efficiency Window Object")]
    public GameObject lowFinanceWindow;

    [Tooltip("High Efficiency Window Object")]
    public GameObject highFinanceWindow;

    [Tooltip("Approval Background objects")]
    public GameObject[] approvalObjs;

    [Tooltip("Low Approval Window Object")]
    public GameObject lowApprovalWindow;

    [Tooltip("High Approval Window Object")]
    public GameObject highApprovalWindow;

    [Tooltip("Low Efficiency Window Object")]
    public GameObject lowEfficiencyWindow;

    [Tooltip("High Efficiency Window Object")]
    public GameObject highEfficiencyWindow;

    [Tooltip("Finance Background objects")]
    public GameObject[] financeObjs;

    [Tooltip("Color used for positive stat change text")]
    public Color darkGreen;



    //[Tooltip("Color used for negative stat change text")]
    //public Color darkRed;

    /// <summary>
    /// What state each stat is in
    /// 0 = bad
    /// 1 = neutral
    /// 2 = good
    /// </summary>
    private int[] statStates = {0,0,0};

    
    /// <summary>
    /// Sets the sliders to the correct initial values.
    /// </summary>
    /// <param name="stats">The initial stats.</param>
    public void Init(float[] stats)
    {
        this.stats = stats.Clone() as float[];

        efficiencySlider.value = stats[1] / 100f;
        approvalSlider.value = stats[2] / 100f;
        financeSlider.value = stats[3] / 100f;


    }

    /// <summary>
    /// Display the results screen.
    /// </summary>
    /// <param name="stats">The updated stats.</param>
    public void Display(float[] stats, string result)
    {
        if (this.stats == null)
        {
            print("NEW ARRAY");
            this.stats = new float[4];
        }

        SetBackgroundSliders(stats);
        this.stats = stats.Clone() as float[];

        resultsText.text = result;

        ShowBackgroundObjs();
        StartCoroutine(WaitThenAnimate());
    }

    private void SetBackgroundSliders(float[] updatedStats)
    {
        float change = updatedStats[1] - stats[1];
        //print("EFF  " + change);

        if (change > 0)
            efficiencyBackground.value = updatedStats[1] / 100f;
        else
            efficiencyBackground.value = stats[1] / 100f;


        change = updatedStats[2] - stats[2];
        //print("APP  " + change);
        if (change > 0)
            approvalBackground.value = updatedStats[2] / 100f;
        else
            approvalBackground.value = stats[2] / 100f;


        change = updatedStats[3] - stats[3];
        //print("FIN  " + change);
        if (change > 0)
            financeBackground.value = updatedStats[3] / 100f;
        else
            financeBackground.value = stats[3] / 100f;
    }

    /// <summary>
    /// Handles calling the coroutine to animate updating the slider values.
    /// </summary>
    /// <param name="stats">The updated stats.</param>
    private void AnimateSlider()
    {
        switch (slider)
        {
            case 1:
                StartCoroutine(AnimateSliderCoroutine(efficiencySlider, efficiencyText, stats[1] / 100f));
                break;
            case 2:
                StartCoroutine(AnimateSliderCoroutine(approvalSlider, approvalText, stats[2] / 100f));
                break;
            case 3:
                StartCoroutine(AnimateSliderCoroutine(financeSlider, financeText, stats[3] / 100f));
                break;
        }
    }

    /// <summary>
    /// Waits before animating the sliders.
    /// </summary>
    private IEnumerator WaitThenAnimate()
    {
        yield return new WaitForSeconds(animationDelayTime);
        AnimateSlider();
    }

    /// <summary>
    /// Animates a slider to a new value.
    /// </summary>
    /// <param name="slider">The slider to animate.</param>
    /// <param name="newValue">The value to animate the slider to.</param>
    /// <returns></returns>
    private IEnumerator AnimateSliderCoroutine(Slider slider, TextMeshProUGUI changeText, float newValue)
    {
        float previousValue = slider.value;
        
        if (newValue > 1)
            print("new value error: " + newValue);

        if (newValue < (statThresholds[0] / 100f))
        {
            slider.fillRect.gameObject.GetComponent<Image>().color = Color.red;
        }
        else if (newValue < (statThresholds[1] / 100f))
        {
            slider.fillRect.gameObject.GetComponent<Image>().color = Color.yellow;
        }
        else
        {
            slider.fillRect.gameObject.GetComponent<Image>().color = Color.green;
        }

        float time = 0f;

        while (time < animationTime)
        {
            time += Time.deltaTime;
            slider.value = Mathf.Lerp(previousValue, newValue, time/animationTime);
            yield return null;
        }


        double change = newValue - previousValue;
        //print("ResultsHandler: slider[" + this.slider + "] value: " + slider.value);

        // Add a '+' to the front of the change text if we had an increase.
        string frontString = change > 0 ? "+" : string.Empty;

        // Set the font ocolor to green if we had an increase, and to red if we had a decrease.
        Color color = change > 0 ? darkGreen : Color.red;

        changeText.color = color;
        changeText.text = frontString + Math.Round(change * 100).ToString();

        ++this.slider;
        AnimateSlider();
    }

    /// <summary>
    /// Shows correct background objects for each stat
    /// </summary>
    private void ShowBackgroundObjs()
    {
        for(int i = 1; i < stats.Length; i++)
        {
            // Changes statState to correct number based on thresholds
            if (stats[i] < statThresholds[0])
            {
                statStates[i - 1] = 0;
            }
            else if (stats[i] < statThresholds[1])
            {
                statStates[i - 1] = 1;
            }
            else
            {
                statStates[i - 1] = 2;
            }
        }
        
        // Hides all background objects so only correct ones will be shown
        HideAllBackgroundObjs();


        // Shows the correct background objects for each stat
        // statStates[0] = efficiency
        // statStates[1] = approval
        // statStates[2] = finance
        for (int i = 0; i < statStates.Length; i++)
        {
            switch (i)
            {
                case 0:
                    efficiencyObjs[statStates[i]].SetActive(true);
                    if (statStates[i] == 0)
                    {
                        lowEfficiencyWindow.SetActive(true);
                    }
                    else if (statStates[i] == 2)
                    {
                        highEfficiencyWindow.SetActive(true);
                    }
                    break;
                case 1:
                    approvalObjs[statStates[i]].SetActive(true);
                    // Shows window objects
                    if (statStates[i] == 0)
                    {
                        lowApprovalWindow.SetActive(true);
                    }
                    else if (statStates[i] == 2)
                    {
                        highApprovalWindow.SetActive(true);
                    }
                    break;
                case 2:
                    financeObjs[statStates[i]].SetActive(true);
                    // Shows window objects
                    if (statStates[i] == 0)
                    {
                        lowFinanceWindow.SetActive(true);
                    }
                    else if (statStates[i] == 2)
                    {
                        highFinanceWindow.SetActive(true);
                    }
                    break;
            }
        }

        // Shows window objects
    }

    /// <summary>
    /// Hides all background objects
    /// </summary>
    private void HideAllBackgroundObjs()
    {
        foreach(GameObject obj in efficiencyObjs)
        {
            obj.SetActive(false);
        }
        foreach(GameObject obj in approvalObjs)
        {
            obj.SetActive(false);
        }
        foreach(GameObject obj in financeObjs)
        {
            obj.SetActive(false);
        }

        // hides window objects
        lowFinanceWindow.SetActive(false);
        highFinanceWindow.SetActive(false);
        lowApprovalWindow.SetActive(false);
        highApprovalWindow.SetActive(false);
        lowEfficiencyWindow.SetActive(false);
        highEfficiencyWindow.SetActive(false);
    }
    /// <summary>
    /// Sets all of the change texts to empty strings and resets the slider counter.
    /// </summary>
    public void Reset()
    {
        StopAllCoroutines();
        slider = 1;

        // Added this because it looked awkward with the text suddenly disappearing after continue button is pressed.
        Invoke("ResetChangeText", 2f);

        // Make sure all of the sliders reach their final values.
        efficiencySlider.value = stats[1] / 100f;
        approvalSlider.value = stats[2] / 100f;
        financeSlider.value = stats[3] / 100f;

        //update the values of sliders in loading menu. Added by Ein :)
        efficiencySliderLoading.value = stats[1] / 100f;
        approvalSliderLoading.value = stats[2] / 100f;
        financeSliderLoading.value = stats[3] / 100f;

        // Change loading efficiency slider color
        if (stats[1] < statThresholds[0])
        {
            efficiencySliderLoading.fillRect.gameObject.GetComponent<Image>().color = Color.red;
        }
        else if (stats[1] < statThresholds[1])
        {
            efficiencySliderLoading.fillRect.gameObject.GetComponent<Image>().color = Color.yellow;
        }
        else
        {
            efficiencySliderLoading.fillRect.gameObject.GetComponent<Image>().color = Color.green;
        }

        // Change loading approval slider color
        if (stats[2] < statThresholds[0])
        {
            approvalSliderLoading.fillRect.gameObject.GetComponent<Image>().color = Color.red;
        }
        else if (stats[2] < statThresholds[1])
        {
            approvalSliderLoading.fillRect.gameObject.GetComponent<Image>().color = Color.yellow;
        }
        else
        {
            approvalSliderLoading.fillRect.gameObject.GetComponent<Image>().color = Color.green;
        }

        // Change loading finance slider color
        if (stats[3] < statThresholds[0])
        {
            financeSliderLoading.fillRect.gameObject.GetComponent<Image>().color = Color.red;
        }
        else if (stats[3] < statThresholds[1])
        {
            financeSliderLoading.fillRect.gameObject.GetComponent<Image>().color = Color.yellow;
        }
        else
        {
            financeSliderLoading.fillRect.gameObject.GetComponent<Image>().color = Color.green;
        }
    }

    private void ResetChangeText()
    {
        efficiencyText.text = string.Empty;
        approvalText.text = string.Empty;
        financeText.text = string.Empty;
    }

    /// <summary>
    /// Resets all background objects so only the given ones are showing
    /// </summary>
    /// <param name="index">
    /// Which background objects to show
    /// 0 = bad
    /// 1 = neutral
    /// 2 = good
    /// </param>
    public void ResetBackgroundObjs(int index)
    {
        HideAllBackgroundObjs();
        approvalObjs[index].SetActive(true);
        efficiencyObjs[index].SetActive(true);
        financeObjs[index].SetActive(true);
    }
}

