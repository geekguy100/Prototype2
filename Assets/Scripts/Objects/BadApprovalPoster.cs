using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadApprovalPoster : Interactable
{

    public AudioClip playClip;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact()
    {
        // Change cursor to revealed one if it wasn't already
        if (!revealed)
        {
            revealed = true;
            ChangeCursor(revealed);
        }
    }
}
