using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Creates text cells from a prefab into children and inserts commentdata into them.
/// Changing CurrentComment will update this object with new cells automatically.
/// </summary>

public class CommentInfoVisualized : MonoBehaviour {

    //public GameObject testImage;
    public GameObject textCell;

    private Comment _currentComment;
    public Comment CurrentComment
    {
        get
        {
            return _currentComment;
        }
        set
        {
            _currentComment = value;
            GenerateInfo();
        }
    }

    private List<GameObject> textCells;

    //public delegate void EventWithComment(Comment comment);
    //public event EventWithComment ChangeCurrentComment;

    private void Start()
    {
        //CurrentComment = GenerateTestComment();
        textCells = new List<GameObject>();
    }

    public void GenerateInfo()
    {
        ClearVisuals();
        Debug.Log("Generating info");

        //if (CurrentComment == null)
        //{
        //    Debug.Log("No comment selected, switching to latest");
        //    CurrentComment = ChooseLatestComment();
        //}
        if (CurrentComment == null)
        {
            Debug.Log("No comment selected, using null values");
            CurrentComment = GenerateTestComment();
        }

        GenerateTextCell(CurrentComment.data.userName);
        GenerateTextCell(CurrentComment.data.submittedShortDate);
        GenerateTextCell(CurrentComment.data.submittedShortTime);

        GenerateTextCell(CurrentComment.data.dataString);
        GenerateTextCell(CurrentComment.data.commentedObjectName);
        GenerateTextCell(CurrentComment.data.quickcheck.ToString());

        GenerateTextCell(CurrentComment.data.type.ToString());
        GenerateTextCell(CurrentComment.data.SHPath);
        GenerateTextCell(CurrentComment.data.commentatorPosition.ToString());
    }

    //public void GenerateTestImages()
    //{
    //    if (testImage)
    //    {
    //        for (int i = 0; i < 5; i++)
    //        {
    //            Instantiate(testImage, transform, false);
    //        }
    //    }
    //    else
    //        Debug.Log("No test image set!");
    //}

    public void GenerateTextCell(string textContent)
    {
        if (!textCell)
        {
            //Load from resources
        }

        if (textCell)
        {
            GameObject cell = Instantiate(textCell, transform, false);
            Text cellText = cell.transform.GetChild(0).GetComponent<Text>();
            cellText.text = textContent;
            //Debug.Log("+Text cell count: " + textCells.Count + ", cellText: " + cellText.text);
            textCells.Add(cell);
        }
        else
        {
            Debug.LogError("No textcell prefab set or found!");
        }
    }

    private Comment GenerateTestComment()
    {
        Comment newComment = new Comment();
        CommentData data = new CommentData();
        data.commentatorPosition = Vector3.zero;
        data.commentedObjectName = "ObjectName";
        data.dataString = "Data";
        data.quickcheck = 123;
        data.SHPath = "Screenshot path";
        data.submittedShortDate = DateTime.Now.ToShortDateString();
        data.submittedShortTime = DateTime.Now.ToShortTimeString();
        data.type = Comment.CommentType.Text;
        data.userName = "Username";

        newComment.data = data;

        return newComment;
    }

    private void ClearVisuals()
    {
        if (textCells.Count > 0)
        {
            textCells.ForEach(Destroy);
            textCells.Clear();
        }
    }


}
