using System;
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

    public VoiceComment(string name, Vector3 position)
    {
        commenterName = name;
        commentPosition = position;
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


    [HideInInspector]
    public PositionData positionData;
    [HideInInspector]
    public PositionDatabase positionDB;

    string commentToFind;

    GameObject buttonImage;
    Text buttonText;
    GameObject panel;
    List<GameObject> buttons;

    DirectoryInfo info;
    FileInfo[] fileInfo;

    void Awake(){
		audioSource = GetComponent<AudioSource> ();

        commentsToPlayHere = new List<string>();
        commentDictionary = new Dictionary<string, VoiceComment>();
        buttons = new List<GameObject>();

        panel = GameObject.Find("CommentTool/CommentList/Canvas/Panel/ScrollableList");
        
    }

    public void LoadComments(){

        if(record == null)
        {
            record = GameObject.Find("PhotonAvatar(Clone)").GetComponent<RecordComment>();
        }

        InitializeCollections();
        Debug.Log(record.SavePath + record.AudioExt);

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

                //Destroy old buttons and create new according to new situation
                DestroyButtons();

                for (int i = 0; i < comments.Length; ++i)
                {
                    commentDictionary.Add(positionDB.list[i].recordName, new VoiceComment(positionDB.list[i].commenterName, new Vector3(positionDB.list[i].position[0], positionDB.list[i].position[1], positionDB.list[i].position[2])));
                    CreateButtons(i);
                }
            }
        }
    }

    void InitializeCollections()
    {
        commentDictionary.Clear();
        commentsToPlayHere.Clear();
        comments = new AudioClip[0];

    }

    void DestroyButtons()
    {

        if (buttons.Count > 0)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                Destroy(buttons[i]);
            }

            buttons.Clear();
        }
        
    }

    void CreateButtons(int index)
    {
        buttonImage = (GameObject)Instantiate(Resources.Load("ButtonBackgroundImage"));
        //buttonText = buttonImage.GetComponent<Text>();
        buttonImage.transform.parent = panel.transform;
        buttonImage.transform.localPosition = Vector3.zero;
        buttonImage.transform.localRotation = Quaternion.identity;
        buttonImage.transform.localScale = Vector3.one;
        buttonText = buttonImage.GetComponentInChildren<Text>();
        buttonText.text = positionDB.list[index].recordName;

        buttons.Add(buttonImage);
    }

    void PlayCommentInPosition(string commentName)
    {
        audioSource.clip = comments[Array.IndexOf(comments, commentName)];

        audioSource.Play();
    }
}
