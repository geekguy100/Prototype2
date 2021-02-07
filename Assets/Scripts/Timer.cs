/*****************************************************************************
// File Name :         Timer.cs
// Author :            Kyle Grenier
// Creation Date :     02/04/2021
//
// Brief Description : Timer class that counts down. Next question on timer finish.
*****************************************************************************/
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [Tooltip("The timer per question in seconds.")]
    [SerializeField] private float timePerQuestion = 25f;

    [Tooltip("The time to wait before the event is invoked in seconds.")]
    [SerializeField] private float eventWaitTime = 2f;

    [Tooltip("The timer's fill area image.")]
    [SerializeField] private Image timerImg = null;

    private bool completed = false;
    public bool Completed { get { return completed; } }

    public delegate void TimerEndHandler();
    public event TimerEndHandler OnTimerEnd;

    private void Awake()
    {
        timerImg.fillAmount = 1;
    }

    /// <summary>
    /// Resets the timer and starts it up again.
    /// </summary>
    public void Reset()
    {
        StopCoroutine("Countdown");
        completed = false;
        timerImg.fillAmount = 1;

        StartCoroutine(Countdown());
    }

    /// <summary>
    /// Counts the timer down from its max time to zero.
    /// </summary>
    private IEnumerator Countdown()
    {
        float time = timePerQuestion;

        while (time > 0f)
        {
            time -= Time.deltaTime;
            timerImg.fillAmount -= Time.deltaTime / timePerQuestion;

            yield return null;
        }

        completed = true;

        //TODO: Play a timer ran out SFX
        yield return new WaitForSeconds(eventWaitTime);

        //Call any methods that have subscribed to the OnTimerEnd event.
        OnTimerEnd?.Invoke();
    }
}