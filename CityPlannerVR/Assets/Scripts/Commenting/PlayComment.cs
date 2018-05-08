using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(AudioSource)]
public class PlayComment : MonoBehaviour {

	AudioClip[] comments;
	AudioSource audioSource;

	void Start(){
		audioSource = GetComponent<AudioSource> ();
	}

	void LoadComment(){
		
	}
}
