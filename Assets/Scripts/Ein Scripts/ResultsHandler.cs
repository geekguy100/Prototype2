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

namespace Ein
{
    public class ResultsHandler : MonoBehaviour
    {
        [Header("Sliders")]
        [SerializeField] private Slider efficiencySlider = null;
        [SerializeField] private Slider approvalSlider = null;
        [SerializeField] private Slider financeSlider = null;

        [Header("Percent Change Texts")]
        [SerializeField] private TextMeshProUGUI efficiencyText = null;
        [SerializeField] private TextMeshProUGUI approvalText = null;
        [SerializeField] private TextMeshProUGUI financeText = null;

        private float[] stats;
        private int slider = 1; // The slider to animate.

        [Header("Animation Settings")]
        [Tooltip("The speed of the slider change animation.")]
        [SerializeField] private float animationSpeed = 1f;

        [Header("Results Text")]
        [Tooltip("Text to hold the result from the most recent scenario.")]
        [SerializeField] private TextMeshProUGUI resultsText = null;


        /// <summary>
        /// Sets the sliders to the correct initial values.
        /// </summary>
        /// <param name="stats">The initial stats.</param>
        public void Init(float[] stats)
        {
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

            while (Mathf.Abs(slider.value - newValue) > 0.005f)
            {
                slider.value = Mathf.Lerp(slider.value, newValue, Time.deltaTime * animationSpeed);
                yield return null;
            }


            double change = newValue - previousValue;
            //print("ResultsHandler: slider[" + this.slider + "] value: " + slider.value);

            // Add a '+' to the front of the change text if we had an increase.
            string frontString = change > 0 ? "+" : string.Empty;

            // Set the font ocolor to green if we had an increase, and to red if we had a decrease.
            Color color = change > 0 ? Color.green : Color.red;

            changeText.color = color;
            changeText.text = frontString + Math.Round(change * 100).ToString();


            ++this.slider;
            AnimateSlider();
        }

        /// <summary>
        /// Sets all of the change texts to empty strings and resets the slider counter back to 0.
        /// </summary>
        public void Reset()
        {
            slider = 1;
            efficiencyText.text = string.Empty;
            approvalText.text = string.Empty;
            financeText.text = string.Empty;
        }
    }
}
