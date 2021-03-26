/*****************************************************************************
// File Name :         DinoController.cs
// Author :            TJ Caron
// Creation Date :     03/26/2021
//
// Brief Description : Changes dinosaur sprite and plays sound specific to 
                       sprite when clicked
*****************************************************************************/
using UnityEngine;
using UnityEngine.UI;

public class DinoController : Interactable
{
    [Tooltip("Different possible dino sprites")]
    public Sprite[] dinoSprites;

    [Tooltip("Sounds played when you click on the dinosaur")]
    public AudioClip[] dinoSounds;

    private Image thisImage;

    private int index = 0;

    private void Start()
    {
        thisImage = GetComponent<Image>();
    }

    public void Interact()
    {
        //revealed = true;
        index++;

        if (index >= dinoSprites.Length)
        {
            index = 0;
        }
        PlaySFX(dinoSounds[index]);
        thisImage.sprite = dinoSprites[index];
    }
}
