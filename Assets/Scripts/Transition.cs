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
    }

    private Animator fadeAnimator;

    [Tooltip("The time to wait after fading in.")]
    [SerializeField] private float fadeWaitTime;


    public delegate void TransitionCallback();
    /// <summary>
    /// Perform the transition effect.
    /// </summary>
    private IEnumerator PerformTransition(TransitionCallback callback)
    {
        fadeAnimator.SetTrigger("FadeIn");
        yield return new WaitForSeconds(fadeWaitTime);
        fadeAnimator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(fadeWaitTime);
        callback?.Invoke();
    }

    public void StartTransition(TransitionCallback callback)
    {
        StartCoroutine(PerformTransition(callback));
    }
}