/*****************************************************************************
// File Name :         BlinkController.cs
// Author :            Kyle Grenier
// Creation Date :     04/05/2021
//
// Brief Description : Makes the character blink after a random amount of time.
*****************************************************************************/
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BlinkController : MonoBehaviour
{
    private Animator anim;

    private const float MIN_BLINK_TIME = 2f;
    private const float MAX_BLINK_TIME = 8f;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        Invoke("Blink", Random.Range(MIN_BLINK_TIME, MAX_BLINK_TIME));
    }

    private void Blink()
    {
        anim.SetTrigger("Blink");
        Invoke("Blink", Random.Range(MIN_BLINK_TIME, MAX_BLINK_TIME));
    }
}