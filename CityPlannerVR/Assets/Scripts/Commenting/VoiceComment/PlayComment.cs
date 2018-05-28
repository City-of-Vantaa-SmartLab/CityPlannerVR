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
//TODO: Käy läpi kommentit ja järjestä/seulo tietyllä tavalla

public struct VoiceComment
{
    public string commenterName;
    public string targetName;
    public Vector3 commentPosition;
    public int commentIndex;

    public VoiceComment(string name, string target, Vector3 position, int index)
    {
        commenterName = name;
        targetName = target;
        commentPosition = position;
        commentIndex = index;
    }
}

[RequireComponent(typeof(AudioSource))]
public class PlayComment : MonoBehaviour {

    List<AudioClip> commentClips;
	//AudioClip[] comments;
    List<string> commentsToPlayHere;
	AudioSource audioSource;
    //The name of the comment and a struct that contains the name of the commenter and the position of the comment
    public Dictionary<string, VoiceComment> commentDictionary;

    RecordComment record;

    GameObject displayedButton;
    int commentIndex = 0;

    [HideInInspector]
    public PositionData positionData;
    [HideInInspector]
    public PositionDatabase positionDB;

    GameObject buttonImage;
    Text buttonText;
    GameObject panel;

    DirectoryInfo info;
    FileInfo[] fileInfo;

    Text commentNumber;

    int index;

    GameObject commentButton;

    //Is set in laserPointer script
    public GameObject pointedTarget;

    void Awake(){
		audioSource = GetComponent<AudioSource> ();

        commentsToPlayHere = new List<string>();
        commentDictionary = new Dictionary<string, VoiceComment>();

        panel = GameObject.Find("CommentTool/CommentList/Canvas/Panel/ScrollableList");

        record = gameObject.transform.parent.GetComponentInChildren<RecordComment>();

        commentNumber = GetComponentInChildren<Text>();

        commentButton = (GameObject)Resources.Load("ButtonBackgroundImage");

        commentClips = new List<AudioClip>();
    }

    public void LoadComments(){

        InitializeCollections();
        //Debug.Log(record.SavePath + record.AudioExt);

        if (Directory.Exists(record.SavePath + record.AudioExt))
        {
            info = new DirectoryInfo(record.SavePath + record.AudioExt);
            fileInfo = info.GetFiles();
            if(fileInfo.Length > 0)
            {
                //fileInfo also contains the meta files but we don't want them in this list
                //comments = new AudioClip[fileInfo.Length/2];
                //comments = Resources.LoadAll<AudioClip>("Comments/VoiceComments/AudioFiles");
                StartCoroutine(LoadCommentsFromStreamingAssets());

                XmlSerializer serializer = new XmlSerializer(typeof(PositionDatabase));
                FileStream file = File.Open(record.SavePath + record.slash + record.positionFileName, FileMode.Open);
                if (file.Length > 0)
                {
                    positionDB = (PositionDatabase)serializer.Deserialize(file);
                    file.Close();
                }

                for (int i = 0; i < commentClips.Count; ++i)
                {
                    commentDictionary.Add(positionDB.list[i].recordName, new VoiceComment(positionDB.list[i].commenterName, positionDB.list[i].targetName , new Vector3(positionDB.list[i].position[0], positionDB.list[i].position[1], positionDB.list[i].position[2]), i));
                }

                GetAllCommentForObjects();
                if (commentsToPlayHere.Count > 0)
                {
                    CreateButton(commentIndex);
                    UpdateCommentNumber();
                }
                else
                {
                    if (displayedButton != null)
                    {
                        Destroy(displayedButton.gameObject);
                        displayedButton = null;
                    }

                    buttonImage = Instantiate(commentButton);
                    buttonImage.transform.SetParent(panel.transform);
                    buttonImage.transform.localPosition = Vector3.zero;
                    buttonImage.transform.localRotation = Quaternion.identity;
                    buttonImage.transform.localScale = Vector3.one;
                    buttonText = buttonImage.GetComponentInChildren<Text>();
                    buttonText.text = "No comments for this object";

                    displayedButton = buttonImage;
                    commentNumber.text = "0/0";
                }
            }
        }
    }

    IEnumerator LoadCommentsFromStreamingAssets()
    {
        for (int i = 0; i < fileInfo.Length; i++)
        {
            WWW request = new WWW("file:///" + record.SavePath + record.AudioExt + fileInfo[i].Name);
            if (fileInfo[i].Name.EndsWith(".wav"))
            {
                commentClips.Add(request.GetAudioClip());

                yield return request;
            }

        }
    }

    void GetAllCommentForObjects()
    {
        //commentsToPlayHere.Clear();

        foreach (KeyValuePair<string, VoiceComment> comment in commentDictionary)
        {
            if(comment.Value.targetName == pointedTarget.name)
            {
                commentsToPlayHere.Add(comment.Key);
            }
        }
    }

    void InitializeCollections()
    {
        commentDictionary.Clear();
        commentsToPlayHere.Clear();
        //comments = new AudioClip[0];
        commentClips.Clear();

    }

    void CreateButton(int index)
    {
        if (displayedButton != null)
        {
            Destroy(displayedButton.gameObject);
            displayedButton = null;
        }

        buttonImage = Instantiate(commentButton);
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
        int index = commentsToPlayHere.IndexOf(commentName);
        //int index = commentDictionary[commentName].commentIndex;
        audioSource.clip = commentClips[index];

        audioSource.Play();
    }

    public void GoForward()
    {
        if (commentsToPlayHere.Count > 0)
        {
            if (commentIndex == commentsToPlayHere.Count - 1)
            //if(commentIndex == comments.Length - 1)
            {
                commentIndex = 0;
            }
            else
            {
                ++commentIndex;
            }

            CreateButton(commentIndex);
            UpdateCommentNumber();
        }
    }

    public void GoBackward()
    {
        if (commentsToPlayHere.Count > 0)
        {
            if (commentIndex == 0)
            {
                commentIndex = commentsToPlayHere.Count - 1;
                //commentIndex = comments.Length - 1;
            }
            else
            {
                --commentIndex;
            }
            CreateButton(commentIndex);
            UpdateCommentNumber();
        }
    }

    void UpdateCommentNumber()
    {
        if (commentDictionary.Count > 0)
        {
            index = commentIndex + 1;
        }
        else
        {
            index = 0;
        }

        //commentNumber.text = index + "/" + commentDictionary.Count;
        commentNumber.text = index + "/" + commentsToPlayHere.Count;
    }
}
