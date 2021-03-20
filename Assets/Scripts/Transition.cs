/*****************************************************************************
// File Name :         Transition.cs
// Author :            Kyle Grenier
// Creation Date :     #CREATIONDATE#
//
// Brief Description : ADD BRIEF DESCRIPTION OF THE FILE HERE
*****************************************************************************/
using UnityEngine;
using System.Collections;

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

    [Tooltip("The time to wait after fading in.")]
    [SerializeField] private float fadeWaitTime;

    [Tooltip("The time to wait after fading in.")]
    [SerializeField] private float pauseWaitTime;

    [Tooltip("The Pause Manager script on the Game Manager object")]
    private PauseManager pauseManager;

    public delegate void TransitionCallback();
    /// <summary>
    /// Perform the transition effect.
    /// </summary>
    private IEnumerator PerformTransition(TransitionCallback callback)
    {
        fadeAnimator.SetTrigger("FadeIn");
        yield return new WaitForSeconds(fadeWaitTime);
        print("should be false it is: " + pauseManager.canPause);
        fadeAnimator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(fadeWaitTime);
        print("should be false it is: " + pauseManager.canPause);
        callback?.Invoke();
        yield return new WaitForSeconds(pauseWaitTime);
        pauseManager.canPause = true;
    }

    public void StartTransition(TransitionCallback callback)
    {       
        pauseManager.canPause = false;
        StartCoroutine(PerformTransition(callback));
    }
}