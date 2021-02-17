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

namespace TJ
{
    public  class Timer : MonoBehaviour
    {
        [Tooltip("The timer per question in seconds.")]
        [SerializeField] protected float timePerQuestion = 25f;

        [Tooltip("The time to wait before the event is invoked in seconds.")]
        [SerializeField] private float eventWaitTime = 2f;

        [Tooltip("The timer's fill area image.")]
        [SerializeField] private Image timerImg = null;

        [Tooltip("Time given to answer a question before stat gain begins to decay")]
        [SerializeField] private float graceTimePerQuestion = 25f;

        [Tooltip("Time before stat gain decays to 50%")]
        [SerializeField] private float decayTime = 25f;

        [Tooltip("Lowest the stat gain multiplier can go")]
        [SerializeField] private float minStatMultiplier = .5f;

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

        private bool completed = false;
        public bool Completed { get { return completed; } }

        public delegate void TimerEndHandler();
        public event TimerEndHandler OnTimerEnd;

        Color orange = new Color(1, .64f, 0);

        //public Image background;

        //The Coroutine previously called to Countdown - used to make sure the timer stops and resets properly.
        private Coroutine lastCall = null;

        private void Awake()
        {
            timerImg.fillAmount = 1;
            skipTime = timePerQuestion - graceTimePerQuestion - decayTime;
            decayPerSecond = (statMultiplier - minStatMultiplier) / decayTime;
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

            timerImg.fillAmount = 1;
            timerImg.color = Color.green;

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
                // Checks if stat decay has already finished
                if (time < skipTime)
                {
                    // Changes color to orange if not orange yet
                    if (timerImg.color != orange)
                    {
                        timerImg.color = orange;
                    }
                }
                // Checks if grace time is over and start decay should start
                else if (time < (timePerQuestion - graceTimePerQuestion))
                {
                    // Changes color to yellow if not yellow yet
                    if (timerImg.color != Color.yellow)
                    {
                        timerImg.color = Color.yellow;
                    }
                    // Decays stat multiplier
                    statMultiplier -= decayPerSecond * Time.deltaTime;
                }
                time -= Time.deltaTime;
                timerImg.fillAmount -= Time.deltaTime / timePerQuestion;
                yield return null;
            }

            completed = true;

            //TODO: Play a timer ran out SFX
            //yield return new WaitForSeconds(eventWaitTime);

            TimerEndAction();
        }

        protected virtual void TimerEndAction()
        {
            OnTimerEnd?.Invoke();
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
        /// Returns the time left on the current timer
        /// </summary>
        /// <returns>Time left on timer</returns>
        public float GetTimeLeft()
        {
            return time;
        }
    }
}