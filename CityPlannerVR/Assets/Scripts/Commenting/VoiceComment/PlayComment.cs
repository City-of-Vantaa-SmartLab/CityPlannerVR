﻿using System;
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
        Debug.Log("AudioSource = " + audioSource);
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

        StartCoroutine(IsAudioFinished());
    }

    public void PauseComment()
    {
        StopCoroutine(IsAudioFinished());
        playButton.SetActive(true);
        pauseButton.SetActive(false);

        audioSource.Pause();
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
    }
}
