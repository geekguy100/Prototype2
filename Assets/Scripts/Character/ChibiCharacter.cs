/*****************************************************************************
// File Name :         ChibiCharacter.cs
// Author :            Kyle Grenier
// Creation Date :     03/06/2021
//
// Brief Description : Script to control the behavior of the chibi tutorial character.
*****************************************************************************/
using UnityEngine;
using System.Collections;

public class ChibiCharacter : MonoBehaviour
{
    private Animator bodyAnim;

    [Tooltip("The Animator responsible for controlling the character's facial animation.")]
    [SerializeField] private Animator faceAnim;

    [Tooltip("The Animator responsible for controlling the character's arm animation.")]
    [SerializeField] private Animator armAnim;

    [Tooltip("The AudioSource that plays object SFX")]
    [SerializeField] private AudioSource sfxSource;

    [Tooltip("Pop audio clip")]
    [SerializeField] private AudioClip popSound;

    private void Awake()
    {
        bodyAnim = GetComponent<Animator>();
    }

    private void Start()
    {
        float blinkTime = Random.Range(5f, 7f);
        StartCoroutine(RepeatFacialAnimation("Blink", blinkTime));

        float tapTime = Random.Range(10f, 30f);
        StartCoroutine(RepeatBodyAnimation("TapFoot", tapTime));

        float walkTime = Random.Range(15f, 30f);
        StartCoroutine(RepeatBodyAnimation("Walk", walkTime));
    }
    
    /// <summary>
    /// Repeats a facial animation given a trigger and repeat rate.
    /// </summary>
    /// <param name="triggerName">The trigger to transition to the desired animation.</param>
    /// <param name="repeatRate">The amount of time in seconds to wait before triggering this animation again.</param>
    private IEnumerator RepeatFacialAnimation(string triggerName, float repeatRate)
    {
        while (true)
        {
            yield return new WaitForSeconds(repeatRate);
            faceAnim.SetTrigger(triggerName);
        }
    }

    /// <summary>
    /// Repeats a body animation given a trigger and repeat rate.
    /// </summary>
    /// <param name="triggerName">The trigger to transition to the desired animation.</param>
    /// <param name="repeatRate">The amount of time in seconds to wait before triggering this animation again.</param>
    private IEnumerator RepeatBodyAnimation(string triggerName, float repeatRate)
    {
        while (true)
        {
            yield return new WaitForSeconds(repeatRate);
            bodyAnim.SetTrigger(triggerName);
        }
    }

    public void PointToProjector()
    {
        armAnim.SetTrigger("Point");
    }

    public void StopPointing()
    {
        armAnim.SetTrigger("Idle");
    }

    public void PlayPop()
    {
        sfxSource.clip = popSound;
        sfxSource.PlayScheduled(0);
    }
}