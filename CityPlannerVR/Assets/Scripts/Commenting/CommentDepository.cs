using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Used in conjuction with CommentInfoVisualized to display metadata. If CommentInfoVisualized component
/// is not in the same GameObject, drag the appropriate gameobjet to targetForMetaData.
/// </summary>

public class CommentDepository : MonoBehaviour {

    public GameObject testImage;
    public GameObject targetForMetaData;
    public CommentInfoVisualized metaDataHolder;
    public List<Comment> currentList;
    public Comment.CommentType currentType;

    private int currentCommentIndex;

    private void Start()
    {
        if (!targetForMetaData)
            metaDataHolder = gameObject.GetComponent<CommentInfoVisualized>();
        else
            metaDataHolder = targetForMetaData.GetComponent<CommentInfoVisualized>();
        if (!metaDataHolder)
            Debug.Log("Could not find comment metadata visualization component!");
    }


    [SerializeField]
    private List<Comment> texts, voices, thumbs = new List<Comment>();

    public void ChooseTextComments()
    {
        GenerateListVisuals(Comment.CommentType.Text, 0);
    }

    /// <summary>
    /// Changes current visualized list and executes update
    /// </summary>

    public void GenerateListVisuals(Comment.CommentType type, int index)
    {
        if (!targetForMetaData)
        {
            Debug.LogError("No target set for comment visualization!");
            return;
        }

        switch (type)
        {
            case Comment.CommentType.Text:
                //if (currentType != Comment.CommentType.Text)  
                //{
                    currentList = SaveData.commentLists.textComments;   //If the list is not a reference, continuous updating is required!
                    currentType = Comment.CommentType.Text;
                //}
                break;

            case Comment.CommentType.Thumb:
                if (currentType != Comment.CommentType.Thumb)
                {
                    currentList = SaveData.commentLists.thumbComments;
                    currentType = Comment.CommentType.Thumb;
                }
                break;

            case Comment.CommentType.Voice:
                if (currentType != Comment.CommentType.Voice)
                {
                    currentList = SaveData.commentLists.voiceComments;
                    currentType = Comment.CommentType.Voice;
                }
                break;

            case Comment.CommentType.None:
                Debug.Log("No list for type none!");
                break;

            default:
                Debug.LogError("Invalid comment type!");
                break;

        }
        if (currentList != null)
            GenerateVisualsOnList(currentList, index);
    }



    private void GenerateVisualsOnList(List<Comment> list, int listIndex)
    {
        if (!metaDataHolder)
            metaDataHolder = targetForMetaData.GetComponent<CommentInfoVisualized>();
        if (metaDataHolder)
        {
            metaDataHolder.CurrentComment = list[listIndex];  //Changing current comment holder should clear the old one automatically and generate new visuals
        }
        else
        {
            Debug.Log("Could not find component CommentListVisualized!");
        }
    }

    public void RotateComment(bool forwards)
    {
        if (currentList.Count == 0)
        {
            Debug.Log("Current comment list is empty!");
            return;
        }
        int nextCommentIndex = currentCommentIndex;
        if (forwards)
            nextCommentIndex++;
        else
            nextCommentIndex--;

        if (nextCommentIndex >= currentList.Count)
            nextCommentIndex = 0;
        if (nextCommentIndex < 0)
            nextCommentIndex = currentList.Count - 1;
        GenerateVisualsOnList(currentList, nextCommentIndex);
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
                Instantiate(testImage, targetForMetaData.transform, false);
            }


        }
        else
            Debug.Log("No test image set!");
    }

}
