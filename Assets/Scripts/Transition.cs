/*****************************************************************************
// File Name :         Transition.cs
// Author :            Kyle Grenier
// Creation Date :     #CREATIONDATE#
//
// Brief Description : ADD BRIEF DESCRIPTION OF THE FILE HERE
*****************************************************************************/
using UnityEngine;
using System.Collections;
using System;

public class Transition : MonoBehaviour
{
    public static Transition instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        fadeAnimator = GetComponent<Animator>();
        pauseManager = GameObject.Find("GameManager").GetComponent<PauseManager>();
    }

    private Animator fadeAnimator;

    private float fadeWaitTime;

    public Action OnTransitionStart;
    public Action OnTransitionEnd;

    [Tooltip("The time to wait after fading in.")]
    [SerializeField] private float pauseWaitTime;

    [Tooltip("The possible animator controllers to use for transition animations.")]
    [SerializeField] private RuntimeAnimatorController[] animatorControllers;
    [SerializeField] private float[] fadeWaitTimes;

    private PauseManager pauseManager;

    private bool variableTransitions = false;


    public delegate void TransitionCallback();
    /// <summary>
    /// Perform the transition effect.
    /// </summary>
    private IEnumerator PerformTransition(TransitionCallback callback)
    {
        if (variableTransitions)
            SetRandomController();
        else
        {
            fadeAnimator.runtimeAnimatorController = animatorControllers[1];
            fadeWaitTime = fadeWaitTimes[1];
        }

        print("Transitioning Start");

        OnTransitionStart?.Invoke();

        fadeAnimator.SetTrigger("FadeIn");
        yield return new WaitForSeconds(fadeWaitTime);
        //print("should be false it is: " + pauseManager.canPause);
        fadeAnimator.SetTrigger("FadeOut");
        //yield return new WaitForSeconds(fadeWaitTime);
        //print("should be false it is: " + pauseManager.canPause);

        print("Transitioning End");
        OnTransitionEnd?.Invoke();

        callback?.Invoke();
        yield return new WaitForSeconds(pauseWaitTime);
        pauseManager.canPause = true;
    }

    /// <summary>
    /// Change wheter or not transitions other than the typical fade-in-and-out can play.
    /// </summary>
    /// <param name="variableTransitions">True if transitions other than the typical fade-in-and-out can play.</param>
    public void SetVariableTransitions(bool variableTransitions)
    {
        this.variableTransitions = variableTransitions;
    }

    private void SetRandomController()
    {
        int range = animatorControllers.Length;
        int index = UnityEngine.Random.Range(0, range);
        fadeAnimator.runtimeAnimatorController = animatorControllers[index];
        fadeWaitTime = fadeWaitTimes[index];
    }

    public void StartTransition(TransitionCallback callback, bool variableTransition = true)
    {       
        pauseManager.canPause = false;
        SetVariableTransitions(variableTransition);
        StartCoroutine(PerformTransition(callback));
    }
}