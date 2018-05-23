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
//TODO: Käy läpi kommentit ja järjestä/seulo tietyllä tavalla

public struct VoiceComment
{
    public string commenterName;
    public Vector3 commentPosition;
    public int commentIndex;

    public VoiceComment(string name, Vector3 position, int index)
    {
        commenterName = name;
        commentPosition = position;
        commentIndex = index;
    }
}

[RequireComponent(typeof(AudioSource))]
public class PlayComment : MonoBehaviour {

	AudioClip[] comments;
    List<string> commentsToPlayHere;
	AudioSource audioSource;
    //The name of the comment and a struct that contains the name of the commenter and the position of the comment
    Dictionary<string, VoiceComment> commentDictionary;

    RecordComment record;

    GameObject displayedButton;
    int commentIndex = 0;

    [HideInInspector]
    public PositionData positionData;
    [HideInInspector]
    public PositionDatabase positionDB;

    string commentToFind;

    GameObject buttonImage;
    Text buttonText;
    GameObject panel;

    DirectoryInfo info;
    FileInfo[] fileInfo;

    void Awake(){
		audioSource = GetComponent<AudioSource> ();

        commentsToPlayHere = new List<string>();
        commentDictionary = new Dictionary<string, VoiceComment>();

        panel = GameObject.Find("CommentTool/CommentList/Canvas/Panel/ScrollableList");
        
    }

    public void LoadComments(){

        if(record == null)
        {
            record = GameObject.Find("PhotonAvatar(Clone)").GetComponent<RecordComment>();
        }

        InitializeCollections();
        //Debug.Log(record.SavePath + record.AudioExt);

        if (Directory.Exists(record.SavePath + record.AudioExt))
        {
            info = new DirectoryInfo(record.SavePath + record.AudioExt);
            fileInfo = info.GetFiles();
            if(fileInfo.Length > 0)
            {
                //fileInfo also contains the meta files but we don't want them in this list
                comments = new AudioClip[fileInfo.Length/2];
                comments = Resources.LoadAll<AudioClip>("Comments/VoiceComments/AudioFiles");

                XmlSerializer serializer = new XmlSerializer(typeof(PositionDatabase));
                FileStream file = File.Open(record.SavePath + record.slash + record.positionFileName, FileMode.Open);
                if (file.Length > 0)
                {
                    positionDB = (PositionDatabase)serializer.Deserialize(file);
                    file.Close();
                }

                for (int i = 0; i < comments.Length; ++i)
                {
                    commentDictionary.Add(positionDB.list[i].recordName, new VoiceComment(positionDB.list[i].commenterName, new Vector3(positionDB.list[i].position[0], positionDB.list[i].position[1], positionDB.list[i].position[2]), i));
                }

                CreateButton(commentIndex);
            }
        }
    }

    void InitializeCollections()
    {
        commentDictionary.Clear();
        commentsToPlayHere.Clear();
        comments = new AudioClip[0];

    }

    void CreateButton(int index)
    {
        buttonImage = (GameObject)Instantiate(Resources.Load("ButtonBackgroundImage"));
        buttonImage.transform.SetParent(panel.transform);
        buttonImage.transform.localPosition = Vector3.zero;
        buttonImage.transform.localRotation = Quaternion.identity;
        buttonImage.transform.localScale = Vector3.one;
        buttonText = buttonImage.GetComponentInChildren<Text>();
        buttonText.text = positionDB.list[index].recordName;

        displayedButton = buttonImage;
    }

    public void PlayCommentInPosition(string commentName)
    {
        int index = commentDictionary[commentName].commentIndex;
        audioSource.clip = comments[index];

        audioSource.Play();
    }

    public void GoForward()
    {
        if(displayedButton != null)
        {
            Destroy(displayedButton);
        }

        if(commentIndex == comments.Length)
        {
            commentIndex = 0;
        }
        else
        {
            ++commentIndex;
            CreateButton(commentIndex);
        }
    }

    public void GoBackward()
    {
        if (displayedButton != null)
        {
            Destroy(displayedButton);
        }

        if (commentIndex == 0)
        {
            commentIndex = comments.Length;
        }
        else
        {
            --commentIndex;
            CreateButton(commentIndex);
        }
    }
}
