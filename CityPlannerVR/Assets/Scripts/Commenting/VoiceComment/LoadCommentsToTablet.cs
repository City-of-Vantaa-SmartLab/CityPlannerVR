using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Xml;
using System.Xml.Serialization;

using UnityEngine.UI;
/// <summary>
/// Info that is saved into a text file so the voice comments can be played later in a right place
/// </summary>
public struct VoiceComment2
{
    public string commenterName;
    public string targetName;
    public Vector3 commentPosition;
    public int commentIndex;

    public VoiceComment2(string name, string target, Vector3 position, int index)
    {
        commenterName = name;
        targetName = target;
        commentPosition = position;
        commentIndex = index;
    }
}

/// <summary>
/// Play the voice comment
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class LoadCommentsToTablet : MonoBehaviour {

    /// <summary> All the voice comments are stored here </summary>
    List<AudioClip> commentClips;
    /// <summary> All the voice comments that are played in this location (i.e Prisma as only comments that are about it) </summary>
    List<string> commentsToPlayHere;
    /// <summary> Is used to play the voice comment </summary>
	AudioSource audioSource;
    ///<summary> The name of the comment and a struct that contains the name of the commenter and the position of the comment </summary>
    public Dictionary<string, VoiceComment2> commentDictionary;

    /// <summary> The position where the target object was during recording </summary>
    [HideInInspector]
    public PositionData positionData;
    /// <summary> Storage for an instance of a PositionDatabase class which is used to record information on text file </summary>
    [HideInInspector]
    public PositionDatabase positionDB;

    /// <summary> The UI image component that is used to display a button for each comment </summary>
    GameObject buttonImage;
    /// <summary> The text on a single button </summary>
    Text buttonText;
    /// <summary> The ScrollableList panel object that contains all the buttons when they are created </summary>
    GameObject panel;

    DirectoryInfo info;
    FileInfo[] fileInfo;


    /// <summary>
    /// which one of the comments is the player listening to
    /// </summary>
    int commentIndexNumber;

    /// <summary>
    /// Reference to the button object
    /// </summary>
    GameObject commentButton;

    bool isFirstEnable = true;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        commentsToPlayHere = new List<string>();
        commentDictionary = new Dictionary<string, VoiceComment2>();

        panel = GameObject.Find("ScrollableList");

        commentButton = (GameObject)Resources.Load("VoiceComment");

        commentClips = new List<AudioClip>();
    }

    private void OnEnable()
    {
        //If this is done before Awake, don't load comments
        if (!isFirstEnable)
        {
            LoadComments();
        }
        isFirstEnable = false;
    }
    /// <summary>
    /// Loads all voiceComment files and sorts them
    /// </summary>
    public void LoadComments()
    {
        InitializeCollections();
        //Debug.Log(record.SavePath + record.AudioExt);

        if (Directory.Exists(RecordComment.SavePath + RecordComment.AudioExt))
        {
            info = new DirectoryInfo(RecordComment.SavePath + RecordComment.AudioExt);
            fileInfo = info.GetFiles();

            if (fileInfo.Length > 0)
            {
                //fileInfo also contains the meta files but we don't want them in this list
                //comments = new AudioClip[fileInfo.Length/2];
                //comments = Resources.LoadAll<AudioClip>("Comments/VoiceComments/AudioFiles");
                StartCoroutine(LoadCommentsFromStreamingAssets());

                XmlSerializer serializer = new XmlSerializer(typeof(PositionDatabase));
                FileStream file = File.Open(RecordComment.SavePath + RecordComment.slash + RecordComment.positionFileName, FileMode.Open);
                if (file.Length > 0)
                {
                    positionDB = (PositionDatabase)serializer.Deserialize(file);
                    file.Close();
                }

                for (int i = 0; i < commentClips.Count; ++i)
                {
                    commentDictionary.Add(positionDB.list[i].recordName, new VoiceComment2(positionDB.list[i].commenterName, positionDB.list[i].targetName, new Vector3(positionDB.list[i].position[0], positionDB.list[i].position[1], positionDB.list[i].position[2]), i));
                }

                GetAllCommentsForObjects();
                if (commentsToPlayHere.Count > 0)
                {
                    CreateButtons();
                }
                else
                {
                    //TODO: Joku teksti indikoimaan, ettei kommentteja ole?
                }
            }
        }
    }

    /// <summary>
    /// Loads the voice comment clips from the StreamingAssets folder
    /// </summary>
    IEnumerator LoadCommentsFromStreamingAssets()
    {
        int index = 0;
        WWW request = null;

        for (int i = 0; i < fileInfo.Length; i++)
        {
            request = new WWW("file:///" + RecordComment.SavePath + RecordComment.AudioExt + fileInfo[i].Name);
            if (fileInfo[i].Name.EndsWith(".wav"))
            {
                commentClips.Add(request.GetAudioClip());
                commentClips[index].name = "VoiceComment";

                index++;
            }
        }
        yield return request;
    }

    /// <summary>
    /// Gets all the comments for one object
    /// </summary>
    void GetAllCommentsForObjects()
    {
        //commentsToPlayHere.Clear();
        foreach (KeyValuePair<string, VoiceComment2> comment in commentDictionary)
        {
            if (comment.Value.targetName == HoverTabletManager.commentTarget.name)
            {
                commentsToPlayHere.Add(comment.Key);
            }
        }
    }

    /// <summary>
    /// Initilizes all commentDictionary, commentsToPlayHere and commentClips
    /// </summary>
    void InitializeCollections()
    {
        commentDictionary.Clear();
        commentsToPlayHere.Clear();
        //comments = new AudioClip[0];
        commentClips.Clear();

    }

    /// <summary>
    /// Creates the button object and puts it under the ScrollableList
    /// </summary>
    void CreateButtons()
    {
        for (int i = 0; i < commentsToPlayHere.Count; i++)
        {
            buttonImage = Instantiate(commentButton);
            buttonImage.transform.SetParent(panel.transform);
            buttonImage.transform.localPosition = Vector3.zero;
            buttonImage.transform.localRotation = Quaternion.identity;
            buttonImage.transform.localScale = Vector3.one * 10;
            buttonText = buttonImage.GetComponentInChildren<Text>();
            buttonText.text = "Joku:";
        }
    }

    /// <summary>
    /// Plays the comment
    /// </summary>
    /// <param name="commentName"> The name of the comment</param>
    public void PlayCommentInPosition(string commentName)
    {
        int index = commentsToPlayHere.IndexOf(commentName);
        //int index = commentDictionary[commentName].commentIndex;
        audioSource.clip = commentClips[index];

        audioSource.Play();
    }
}
