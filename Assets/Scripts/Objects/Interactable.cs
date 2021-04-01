/*****************************************************************************
// File Name :         Interactable.cs
// Author :            TJ Caron
// Creation Date :     03/26/2021
//
// Brief Description : Class for all interactable background objects to extend
*****************************************************************************/
using UnityEngine;

public class Interactable : MonoBehaviour
{

    [Tooltip("Whether or not this object has been interacted with already")]
    public bool revealed = false;

    [Tooltip("Cursor texture used when this object has been interacted with already")]
    public Texture2D revealedCursor;

    [Tooltip("Cursor texture used when this object has not been interacted with already")]
    public Texture2D questionMarkCursor;

    [Tooltip("SFX audiosource")]
    public AudioSource sfxSource;

    /// <summary>
    /// Offset used on the revealed cursor for this object
    /// </summary>
    private Vector2 revealedOffset;

    /// <summary>
    /// Offset used on the question mark cursor for this object
    /// </summary>
    private Vector2 questionMarkOffset;

    private void Start()
    {
        questionMarkOffset = new Vector2(questionMarkCursor.width / 2, questionMarkCursor.height / 2);
        revealedOffset = new Vector2(revealedCursor.width / 2, revealedCursor.height / 2);
    }
    /// <summary>
    /// Changes the cursor sprite
    /// </summary>
    /// <param name="entering">Whether or not the cursor is entering the fan</param>
    public void ChangeCursor(bool entering)
    {
        // Checks if player is entering the object
        if (entering)
        {
            // Checks whether player has interacted with object already
            if (revealed)
            {
                Cursor.SetCursor(revealedCursor, revealedOffset, CursorMode.Auto);
            }
            else
            {
                Cursor.SetCursor(questionMarkCursor, questionMarkOffset, CursorMode.Auto);
            }
        }
        else
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }     
    }

    /// <summary>
    /// Plays a sound effect from the sfx source - prevents overlapping sound
    /// </summary>
    /// <param name="sound">Sound to play</param>
    protected void PlaySFX(AudioClip sound)
    {
        sfxSource.clip = sound;
        sfxSource.PlayScheduled(0);
    }
}
