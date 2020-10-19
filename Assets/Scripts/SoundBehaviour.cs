using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBehaviour : MonoBehaviour
{
    public AudioSource click;
    public void playSound(AudioSource click)
    {
        click.Play();
    }
}
