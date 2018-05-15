﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Xml;
using System.Xml.Serialization;

using UnityEngine.UI;

//Use www class for the server?

//TODO: Kun osotetaan taloa, lista sen talon kommenteista
//TODO: kun painetaan triggeriä tms. niin valitaan soitettava kommentti
//TODO: Käy läpi kommentit ja jäjestä/seulo tietyllä tavalla

public struct VoiceComment
{
    public string commenterName;
    public Vector3 commentPosition;

    public VoiceComment(string name, Vector3 position)
    {
        commenterName = name;
        commentPosition = position;
    }
}

[RequireComponent(typeof(AudioSource))]
public class PlayComment : MonoBehaviour {

	AudioClip[] comments;
	AudioSource audioSource;
    //The name of the comment and a struct that contains the name of the commenter and the position of the comment
    Dictionary<string, VoiceComment> dictionary;

    RecordComment record;

    string commentToFind;

    GameObject buttonImage;
    Text buttonText;
    GameObject panel;

    void Start(){
		audioSource = GetComponent<AudioSource> ();

        panel = GameObject.Find("CommentList/Canvas/Panel");
    }

	void LoadComments(){

        if (File.Exists(record.SavePath))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(PositionDatabase));
            FileStream file = File.Open(record.SavePath, FileMode.Open);
            if (file.Length > 0)
            {
                record.positionDB = (PositionDatabase)serializer.Deserialize(file);
                file.Close();

                for (int i = 0; i < record.positionDB.list.Count; ++i)
                {
                    dictionary.Add(record.position.recordName, new VoiceComment(record.position.recordName, new Vector3(record.position.position[0], record.position.position[1], record.position.position[2])));
                    comments[i] = Resources.Load("VoiceChat/" + record.position.recordName) as AudioClip;
                    buttonImage = (GameObject)Instantiate(Resources.Load("ButtonBackgroundImage"));
                    buttonText = buttonImage.GetComponent<Text>();
                    buttonImage.transform.parent = panel.transform;
                    buttonImage.transform.localScale = Vector3.one;
                    buttonText = buttonImage.GetComponentInChildren<Text>();
                    buttonText.text = record.position.recordName;
                }
            }
        }
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
