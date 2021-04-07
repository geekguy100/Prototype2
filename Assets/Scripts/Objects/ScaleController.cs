using UnityEngine;

public class ScaleController : Interactable
{
    [Tooltip("Sound to play when scale animation starts/stops")]
    public AudioClip scaleSound;

    /// <summary>
    /// Whether or not scale animation is playing
    /// </summary>
    private bool isPlaying = false;

    /// <summary>
    /// Animator controlling scale animation
    /// </summary>
    private Animator scaleAnim;

    private void Start()
    {
        scaleAnim = GetComponent<Animator>();
        scaleAnim.keepAnimatorControllerStateOnDisable = true;
    }

    public void Interact()
    {
        // Change cursor to revealed one if it wasn't already
        if (!revealed)
        {
            revealed = true;
            ChangeCursor(revealed);
        }
        // Play animation if stopped, stop animation if playing
        isPlaying = !isPlaying;
        scaleAnim.SetBool("playScale", isPlaying);
        PlaySFX(scaleSound);
    }
}
