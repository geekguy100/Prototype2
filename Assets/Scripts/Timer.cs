/*****************************************************************************
// File Name :         Timer.cs
// Author :            Kyle Grenier / TJ Caron
// Creation Date :     02/04/2021
//
// Brief Description : Timer class that counts down. Next scenario loads on timer finish.
                       Contains a stat multiplier that decays as the timer goes on.
*****************************************************************************/
using UnityEngine;
using System.Collections;
using UnityEngine.UI;



public class Timer : MonoBehaviour
{
    [Tooltip("The timer per question in seconds.")]
    [SerializeField] protected float timePerQuestion = 25f;

    [Tooltip("The time to wait before the event is invoked in seconds.")]
    [SerializeField] private float eventWaitTime = 2f;

    /// <summary>
    /// Time it takes for full stat decay - equal to 1/4 of full timer
    /// </summary>
    private float decayTime = 0;

    [Tooltip("Whether or not this timer is part of the tutorial")]
    public bool isTutorialTimer = false;

    [Tooltip("Lowest the stat gain multiplier can go")]
    [SerializeField] private float minStatMultiplier = .5f;

    [Tooltip("How long the warnings stay on screen")]
    [SerializeField] private float warningTime = 3f;

    [Tooltip("Spinner showing timer's progress")]
    [SerializeField] private GameObject spinnerObj;

    [Tooltip("Warning saying timer is 3/4 over")]
    [SerializeField] private GameObject quarterWarning;

    [Tooltip("Warning saying timer is half over")]
    [SerializeField] private GameObject halfWarning;

    /// <summary>
    /// Time from stat decay ending to question being skipped
    /// </summary>
    private float skipTime;

    /// <summary>
    /// Amount the stat gain multiplier decays per second
    /// </summary>
    private float decayPerSecond;

    /// <summary>
    /// Multiplier for how much stats go up for the player - range from 100-50
    /// </summary>
    private float statMultiplier = 1;

    /// <summary>
    /// Amount of time left on timer
    /// </summary>
    private float time = 0;

    /// <summary>
    /// Number of degrees in full timer rotation
    /// </summary>
    private float fullRot = 360f;

    /// <summary>
    /// Change in rotation of timer
    /// </summary>
    private float rotChange = 0;

    private bool completed = false;
    public bool Completed { get { return completed; } }

    public delegate void TimerEndHandler();
    public event TimerEndHandler OnTimerEnd;

    private bool halfWarningSent, quarterWarningSent;

    [Tooltip("Sound when timer reaches half point")]
    public AudioClip halfWarningSound;

    [Tooltip("Sound when timer reaches quarter point")]
    public AudioClip quarterWarningSound;

    [Tooltip("SFX AudioSource")]
    public AudioSource sfxSource;

    [Tooltip("Warning SFX volume (0-1)")]
    public float warningVolume;

    /// <summary>
    /// Whether or not timer is paused
    /// </summary>
    private bool isPaused = false;
    //public Image background;

    //The Coroutine previously called to Countdown - used to make sure the timer stops and resets properly.
    private Coroutine lastCall = null;

    private void Awake()
    {
        skipTime = timePerQuestion / 4f;
        decayTime = timePerQuestion / 4f;
        decayPerSecond = (statMultiplier - minStatMultiplier) / decayTime;

        StartCoroutine(Countdown());
    }

    /// <summary>
    /// Resets the timer and starts it up again.
    /// </summary>
    public void Reset()
    {
        completed = false;

        if (lastCall != null)
            StopCoroutine(lastCall);

        statMultiplier = 1;

        spinnerObj.transform.rotation = Quaternion.identity;

        halfWarningSent = false;
        quarterWarningSent = false;
        isPaused = false;
        lastCall = StartCoroutine(Countdown());
    }

    /// <summary>
    /// Counts the timer down from its max time to zero.
    /// </summary>
    private IEnumerator Countdown()
    {
        time = timePerQuestion;

        while (time > 0)
        {
            if (!isPaused)
            {
                // Checks if timer is 3/4 over (send warning here)
                if (time < timePerQuestion / 4)
                {
                    // Sends quarter warning
                    if (!quarterWarningSent)
                    {
                        quarterWarningSent = true;
                        sfxSource.PlayOneShot(quarterWarningSound, warningVolume);
                        quarterWarning.SetActive(true);
                        Invoke("HideWarnings", warningTime);
                    }
                }
                // Checks if grace time is over and start decay should start
                else if (time < timePerQuestion / 2)
                {
                    // Sends half warning 
                    if (!halfWarningSent)
                    {
                        halfWarningSent = true;
                        sfxSource.PlayOneShot(halfWarningSound, warningVolume);
                        halfWarning.SetActive(true);
                        Invoke("HideWarnings", warningTime);
                    }
                    // Decays stat multiplier
                    //statMultiplier -= decayPerSecond * Time.deltaTime;
                }

                // Pause timer if player presses Q
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    TimerPause();
                }

                time -= Time.deltaTime;

                // Gets rotation change
                rotChange = Time.deltaTime * fullRot / timePerQuestion;

                // Adds rotation change to old rotation and applied new rotation to spinner
                Vector3 oldRot = spinnerObj.transform.rotation.eulerAngles;
                Vector3 newRot = oldRot + new Vector3(0, 0, -rotChange);
                spinnerObj.transform.rotation = Quaternion.Euler(newRot);
            }

            yield return null;
        }

        completed = true;

        //TODO: Play a timer ran out SFX
        //yield return new WaitForSeconds(eventWaitTime);

        TimerEndAction();
    }

    protected virtual void TimerEndAction()
    {
        if (!isTutorialTimer)
        {
            OnTimerEnd?.Invoke();
        }
    }
    public void PauseTimer()
    {
        isPaused = true;
    }

    public void UnpauseTimer()
    {
        isPaused = false;
    }
    /// <summary>
    /// Returns stat multiplier from timer
    /// </summary>
    /// <returns>Stat multiplier as float</returns>
    public float GetStatMultiplier()
    {
        // Makes sure stat multiplier isn't below its minimum
        if (statMultiplier < minStatMultiplier)
        {
            statMultiplier = minStatMultiplier;
        }
        return statMultiplier;
    }

    /// <summary>
    /// Hides timer warnings
    /// </summary>
    private void HideWarnings()
    {
        quarterWarning.SetActive(false);
        halfWarning.SetActive(false);
    }

    private void TimerPause()
    {
        isPaused = !isPaused;
    }

    /// <summary>
    /// Returns the time left on the current timer
    /// </summary>
    /// <returns>Time left on timer</returns>
    public float GetTimeLeft()
    {
        return time;
    }
}

