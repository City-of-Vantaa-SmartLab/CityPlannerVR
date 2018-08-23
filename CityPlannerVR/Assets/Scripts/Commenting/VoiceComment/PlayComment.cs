using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Play the voice comment
/// </summary>
//[RequireComponent(typeof(AudioSource))]
public class PlayComment : MonoBehaviour {

    /// <summary>
    /// Is used to play the voice comment
    /// </summary>
    //[HideInInspector]
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

    public Slider slider;

    void Start()
    {
        audioSource = GetComponentInParent<AudioSource>();
    }

    /// <summary>
    /// Plays the comment
    /// </summary>
    public void PlayCommentInPosition()
    {
        audioSource.clip = comment;
        playButton.SetActive(false);
        pauseButton.SetActive(true);

        audioSource.PlayOneShot(comment);

		List<string> info = new List<string>();
		info.Add ("commentName");
		info.Add ("name");
		GameObject.Find ("GameManager").GetComponent<Logger> ().LogActionLine ("PlayCommentClicked", null);

        StartCoroutine(IsAudioFinished());
    }

    public void PauseComment()
    {
        StopCoroutine(IsAudioFinished());
        playButton.SetActive(true);
        pauseButton.SetActive(false);

        audioSource.Pause();

		GameObject.Find ("GameManager").GetComponent<Logger> ().LogActionLine ("PauseCommentClicked", null);
    }

    IEnumerator IsAudioFinished()
    {
        float time = 0;

        while (audioSource.isPlaying)
        {
            float progress = Mathf.Clamp01(time / comment.length);
            time += Time.deltaTime;
            slider.value = progress;
            
            yield return null;
        }

        playButton.SetActive(true);
        pauseButton.SetActive(false);
        slider.value = 0;

		GameObject.Find ("GameManager").GetComponent<Logger> ().LogActionLine ("PlayingCommentEnded", null);
    }
}
