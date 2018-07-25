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

/// <summary>
/// Play the voice comment
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class PlayComment : MonoBehaviour {

    /// <summary>
    /// All the voice comments are stored here
    /// </summary>
    List<AudioClip> commentClips;
    /// <summary>
    /// All the voice comments that are played in this location (i.e Prisma as only comments that are about it)
    /// </summary>
    List<string> commentsToPlayHere;
    /// <summary>
    /// Is used to play the voice comment
    /// </summary>
	AudioSource audioSource;
    ///<summary>
    ///The name of the comment and a struct that contains the name of the commenter and the position of the comment
    ///</summary>
    public Dictionary<string, VoiceComment> commentDictionary;

    /// <summary>
    /// The button displayed at a time
    /// </summary>
    GameObject displayedButton;
    /// <summary>
    /// The index of a voice comment
    /// </summary>
    int commentIndex = 0;

    /// <summary>
    /// The position where the target object was during recording
    /// </summary>
    [HideInInspector]
    public PositionData positionData;
    /// <summary>
    /// Storage for an instance of a PositionDatabase class which is used to record information on text file
    /// </summary>
    [HideInInspector]
    public PositionDatabase positionDB;

    /// <summary>
    /// The UI image component that is used to display a button for each comment
    /// </summary>
    GameObject buttonImage;
    /// <summary>
    /// The text on a single button
    /// </summary>
    Text buttonText;
    /// <summary>
    /// The ScrollableList panel object that contains all the buttons when they are created
    /// </summary>
    GameObject panel;

    DirectoryInfo info;
    FileInfo[] fileInfo;

    /// <summary>
    /// The UI text component used to show to the player how many comments are there in particular object
    /// and which one of the is the player listening
    /// </summary>
    //---Text commentNumber;

    /// <summary>
    /// which one of the comments is the player listening to
    /// </summary>
    int commentIndexNumber;

    /// <summary>
    /// Reference to the button object
    /// </summary>
    GameObject commentButton;

    //Is set in laserPointer script
    public GameObject pointedTarget;

    bool isFirstEnable = true;

    void Awake(){
		audioSource = GetComponent<AudioSource> ();

        commentsToPlayHere = new List<string>();
        commentDictionary = new Dictionary<string, VoiceComment>();

        panel = GameObject.Find("Hover_tablet_prefab/PagesCanvas/ReadCommentsPage/CommentList/ScrollableList");

        //---commentNumber = GetComponentInChildren<Text>();

        commentButton = (GameObject)Resources.Load("ButtonBackgroundImage");

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
    public void LoadComments(){

        InitializeCollections();
        //Debug.Log(record.SavePath + record.AudioExt);

        if (Directory.Exists(RecordComment.SavePath + RecordComment.AudioExt))
        {
            info = new DirectoryInfo(RecordComment.SavePath + RecordComment.AudioExt);
            fileInfo = info.GetFiles();
            if(fileInfo.Length > 0)
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
                    commentDictionary.Add(positionDB.list[i].recordName, new VoiceComment(positionDB.list[i].commenterName, positionDB.list[i].targetName , new Vector3(positionDB.list[i].position[0], positionDB.list[i].position[1], positionDB.list[i].position[2]), i));
                }

                GetAllCommentsForObjects();
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
                    //---commentNumber.text = "0/0";
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

        foreach (KeyValuePair<string, VoiceComment> comment in commentDictionary)
        {
            if(comment.Value.targetName == pointedTarget.name)
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
    /// <param name="index">Index of the selected comment in commentsToPlay here</param>
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
        buttonText.text = commentsToPlayHere[index];

        displayedButton = buttonImage;
    }

    /// <summary>
    /// Plays the comment
    /// </summary>
    /// <param name="commentName"></param>
    public void PlayCommentInPosition(string commentName)
    {
        int index = commentsToPlayHere.IndexOf(commentName);
        //int index = commentDictionary[commentName].commentIndex;
        audioSource.clip = commentClips[index];

        audioSource.Play();
    }

    /// <summary>
    /// Used to scroll comments forward
    /// </summary>
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

    /// <summary>
    /// Used to scroll comments backwards
    /// </summary>
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

    /// <summary>
    /// Updates the comment number text
    /// </summary>
    void UpdateCommentNumber()
    {
        if (commentDictionary.Count > 0)
        {
            commentIndexNumber = commentIndex + 1;
        }
        else
        {
            commentIndexNumber = 0;
        }

        //commentNumber.text = index + "/" + commentDictionary.Count;
        //---commentNumber.text = commentIndexNumber + "/" + commentsToPlayHere.Count;
    }
}
