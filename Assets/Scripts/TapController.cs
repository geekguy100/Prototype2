/*****************************************************************************
// File Name :         TapController.cs
// Author :            Kyle Grenier
// Creation Date :     04/05/2021
//
// Brief Description : Makes the character tap after a random amount of time.
*****************************************************************************/
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class TapController : MonoBehaviour
{
    private Animator anim;

    private const float MIN_BLINK_TIME = 5f;
    private const float MAX_BLINK_TIME = 10f;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        Invoke("Tap", Random.Range(MIN_BLINK_TIME, MAX_BLINK_TIME));
    }

    private void Tap()
    {
        anim.SetTrigger("Tap");
        Invoke("Tap", Random.Range(MIN_BLINK_TIME, MAX_BLINK_TIME));
    }
}