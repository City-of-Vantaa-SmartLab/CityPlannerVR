using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Play the voice comment
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class PlayComment : MonoBehaviour {

    /// <summary>
    /// Is used to play the voice comment
    /// </summary>
    [HideInInspector]
	public AudioSource audioSource;

    [Tooltip("The PlayButton game object")]
    public GameObject playButton;
    [Tooltip("The PauseButton game object")]
    public GameObject pauseButton;

    /// <summary>
    /// The comment this button plays (is given to it when created in LoadCommentsToTablet class
    /// </summary>
    [HideInInspector]
    public AudioClip comment;


    void Awake(){
		audioSource = GetComponent<AudioSource> ();
    }

    /// <summary>
    /// Plays the comment
    /// </summary>
    public void PlayCommentInPosition()
    {
        Debug.Log("comment = " + comment);
        audioSource.clip = comment;
        playButton.SetActive(false);
        pauseButton.SetActive(true);

        audioSource.Play();
        Debug.Log("AudioSource.isPlaying = " + audioSource.isPlaying);

        //StartCoroutine(IsAudioFinished());
    }

    public void PauseComment()
    {
        //StopCoroutine(IsAudioFinished());
        playButton.SetActive(true);
        pauseButton.SetActive(false);

        audioSource.Pause();
    }

    IEnumerator IsAudioFinished()
    {
        if (audioSource.isPlaying)
        {
            yield return null;
        }

        else
        {
            //playButton.SetActive(true);
            //pauseButton.SetActive(false);
        }
    }
}
