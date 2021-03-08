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

public class AnimationController : MonoBehaviour
{
    public int roombaPath;
    Animator animator;
    
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
