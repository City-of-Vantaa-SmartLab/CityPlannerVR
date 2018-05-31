using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Stores comments locally to lists (by type). For debugging only!
/// Use SaveData singleton for easy access to similar lists (like in this script)
/// </summary>

public class CommentDepository : MonoBehaviour {

    public GameObject testImage;
    public GameObject targetForVisuals;

    [SerializeField]
    private List<Comment> texts, voices, thumbs = new List<Comment>();


    void Start () {
        //InitializeCommentLists();
        InvokeRepeating("UpdateLists", 1, 3);
    }


    void InitializeCommentLists()
    {
        texts = new List<Comment>();
        voices = new List<Comment>();
        thumbs = new List<Comment>();
    }

    private void UpdateLists()
    {
        texts = SaveData.commentLists.textComments; //copy content or reference?
        voices = SaveData.commentLists.voiceComments;
        thumbs = SaveData.commentLists.thumbComments;
    }

    public void GenerateVisuals(Comment.CommentType type)
    {
        if (!targetForVisuals)
        {
            Debug.LogError("No target set for comment visualization!");
            return;
        }
        Debug.Log("Generating list for type: " + type);
        switch (type)
        {
            case Comment.CommentType.Text:
                //GenerateVisualsOnList(texts);
                GenerateTestImages();
                break;

            case Comment.CommentType.Thumb:
                GenerateVisualsOnList(voices);
                break;

            case Comment.CommentType.Voice:
                GenerateVisualsOnList(thumbs);
                break;

            case Comment.CommentType.None:
                Debug.Log("No list for type none!");
                break;

            default:
                Debug.LogError("No such comment type!");
                break;

        }
    }

    private void GenerateVisualsOnList(List<Comment> list)
    {
        
    }

    public void GenerateTestImages()
    {
        if (testImage)
        {
            for (int i = 0; i < 5; i++)
            {
                //GameObject go = new GameObject();
                //Image newImage = go.AddComponent<Image>();
                //newImage = testImage.GetComponent<Image>();

                ////go.transform.parent = targetForVisuals.transform;
                //go.transform.SetParent(targetForVisuals.transform, false);
                Instantiate(testImage, targetForVisuals.transform, false);
            }


        }
        else
            Debug.Log("No test image set!");
    }

}
