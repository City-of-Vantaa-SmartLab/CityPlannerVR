using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Creates text cells from a prefab into this gameobject's children and inserts commentdata into them.
/// Changing CurrentComment will update this object with new cells automatically.
/// </summary>

public class CommentInfoVisualized : MonoBehaviour {

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
        textCells = new List<GameObject>();
    }

    public void GenerateInfo()
    {
        ClearVisuals();
        Debug.Log("Generating info");

        if (CurrentComment == null)
        {
            Debug.Log("No comment selected, using null values");
            CurrentComment = Comment.GenerateTestComment();
        }

        GenerateTextCell(CurrentComment.data.userName);
        GenerateTextCell(CurrentComment.data.submittedShortDate);
        GenerateTextCell(CurrentComment.data.submittedShortTime);

        GenerateTextCell(CurrentComment.data.dataString);
        GenerateTextCell(CurrentComment.data.commentedObjectName);
        GenerateTextCell(CurrentComment.data.quickcheck.ToString());

        GenerateTextCell(CurrentComment.data.commentType.ToString());
        GenerateTextCell(CurrentComment.data.SHPath);
        //GenerateTextCell(CurrentComment.data.commentatorPosition.ToString());
    }

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

    private void ClearVisuals()
    {
        if (textCells.Count > 0)
        {
            textCells.ForEach(Destroy);
            textCells.Clear();
        }
    }


}
