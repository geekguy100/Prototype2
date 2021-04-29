/*****************************************************************************
// File Name :         AnimationController.cs
// Author :            Ashley Wielgos
// Creation Date :     03/06/2021
//
// Brief Description : Holder for animation scripts.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AnimationController : MonoBehaviour
{
    Animator animator;
    public int roombaPath;

    [SerializeField] private AudioMixer uiMixer;

    [Tooltip("AudioSource playing roomba sound")]
    public AudioSource roombaSource;
    
    [Tooltip("Audio Clip of roomba bumping into something")]
    public AudioClip roombaBump;

    [Tooltip("AudioSource used for SFX")]
    public AudioSource sfxSource;

    [Header("Animators")]
    [Tooltip("Animator for title transparency")]
    public Animator titleTransparency;

    [Tooltip("Animator for title zoom")]
    public Animator titleZoom;

    [Tooltip("Animator for title sides")]
    public Animator titleSides;

    [Tooltip("Animator for title start button")]
    public Animator titleStart;

    [Tooltip("Animator for title settings button")]
    public Animator titleSettings;

    [Tooltip("Animator for title credits button")]
    public Animator titleCredits;

    [Tooltip("Animator for title exit button")]
    public Animator titleExit;

    [Tooltip("Animator for title character movement")]
    public Animator titleCharacter;

    [Tooltip("Animator for title character movement")]
    public Animator titleText;

    [Tooltip("Animator for title start button")]
    public Animator titleStartText;

    [Tooltip("Animator for title settings button")]
    public Animator titleSettingsText;

    [Tooltip("Animator for title credits button")]
    public Animator titleCreditsText;

    [Tooltip("Animator for title exit button")]
    public Animator titleExitText;

    private float originalVolume;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        if (roombaPath == 1)
        {
            animator.SetBool("Tutorial", true);
        }
        else if (roombaPath == 2)
        {
            animator.SetBool("Conference", true);
        }

        uiMixer.GetFloat("UIVolume", out originalVolume);
    }

    private void OnEnable()
    {
        if (roombaSource != null)
        {
            Transition.instance.OnTransitionStart += FadeOutAudio;
            Transition.instance.OnTransitionEnd += FadeInAudio;
        }
    }

    private void OnDisable()
    {
        StopRoombaSound();
        Transition.instance.OnTransitionStart -= FadeOutAudio;
        Transition.instance.OnTransitionEnd -= FadeInAudio;
    }

    public void TitleSequence()
    {
        titleZoom.SetBool("TitleZoom", true);
        titleSides.SetBool("TitleSides", true);
        titleTransparency.SetBool("TitleTransparent", true);
        //titleCharacter.SetBool("TitleCharacter", true);
        titleStart.SetBool("TitleButton", true);
        titleSettings.SetBool("TitleButton", true);
        titleCredits.SetBool("TitleButton", true);
        titleExit.SetBool("TitleButton", true);
        titleText.SetBool("TitleText", true);
        titleStartText.SetBool("ButtonText", true);
        titleSettingsText.SetBool("ButtonText", true);
        titleCreditsText.SetBool("ButtonText", true);
        titleExitText.SetBool("ButtonText", true);
    }

    public void PlayRoombaSound()
    {
        if (roombaSource != null)
            roombaSource.Play();
    }

    public void PlayRoombaBump()
    {
        sfxSource.clip = roombaBump;
        sfxSource.PlayScheduled(0);
    }
    public void StopRoombaSound()
    {
        if (roombaSource != null)
            roombaSource.Stop();
    }

    private void FadeOutAudio()
    {
        print("FADING OUT AUDIO>>>");
        StartCoroutine(FadeAudio(-80));
    }

    private void FadeInAudio()
    {
        print("FADING IN AUDIO>>>");
        StartCoroutine(FadeAudio(originalVolume));
    }

    private IEnumerator FadeAudio(float finalVolume)
    {
        const float FADE_TIME = 1f;
        float currentTime = 0f;

        while (currentTime < FADE_TIME)
        {
            currentTime += Time.deltaTime;
            float newVol = Mathf.Lerp(originalVolume, finalVolume, currentTime / FADE_TIME);
            uiMixer.SetFloat("UIVolume", newVol);
            yield return null;
        }
    }
}
