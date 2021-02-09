/*
 * Plays the sound when the buttons are clicked
 */
using UnityEngine;

namespace Ein
{
    public class SoundBehaviour : MonoBehaviour
    {
        // What sound to play
        public AudioSource click;

        // Function to call to actually play the sound.
        public void playSound(AudioSource click)
        {
            click.Play();
        }
    }

}