using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//Use www class for the server?

public struct VoiceComment
{
    public string commenterName;
    public Vector3 commentPosition;
}

//[RequireComponent(typeof(AudioSource))]
public class PlayComment : MonoBehaviour {

	AudioClip[] comments;
	AudioSource audioSource;
    //The name of the comment and a struct that contains the name of the commenter and the position of the comment
    Dictionary<string, VoiceComment> dictionary;

    string commentToFind;

    //mihin tungetaan tämä skripti?? -> Miten saadaan record comments tänne? -> miten saadaan ladattua positiot????

    void Start(){
		audioSource = GetComponent<AudioSource> ();
	}

	void LoadComments(){
		//Hae klippi serveriltä
        //Hae positiotiedot serveriltä
        //Pistä kommentin nimi ja positio NamePos dictionaryyn
        //Pistä kommentin nimi ja kommentoijan nimi toiseen dictionaryyn
	}

    void PlayCommentInPosition()
    {
        foreach (var comment in dictionary)
        {
            if(comment.Key == commentToFind)
            {
                audioSource.clip = comments[Array.IndexOf(comments, commentToFind)];
            }
        }

        audioSource.Play();
    }
}
