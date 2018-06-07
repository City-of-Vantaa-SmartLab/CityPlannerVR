using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Creates text cells from a prefab into children and inserts commentdata into them.
/// Changing CurrentComment will update this object with new cells automatically.
/// </summary>

public class CommentListVisualized : MonoBehaviour
{

    //public GameObject testImage;
    public GameObject textCell;
    public List<Comment> currentList;

    private List<GameObject> textCells;

    //public delegate void EventWithComment(Comment comment);
    //public event EventWithComment ChangeCurrentComment;

    private void Start()
    {
        currentList = null;
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
        if (currentList == null)
        {
            Debug.Log("No list selected, using test list");
            currentList = GenerateTestList();
        }

        foreach (Comment comment in currentList)
        {
            GenerateTextCell(comment.data.userName);
            GenerateTextCell(comment.data.submittedShortDate);
            GenerateTextCell(comment.data.quickcheck.ToString());

        }



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

    public List<Comment> GenerateTestList()
    {
        List<Comment> newList = new List<Comment>();
        for (int i = 0; i < 5; i++)
        {
            Comment testComment;
            testComment = CommentInfoVisualized.GenerateTestComment();
            newList.Add(testComment);
        }

        return newList;
    }

    private void ClearVisuals()
    {
        if (textCells.Count > 0)
        {
            textCells.ForEach(Destroy);
            textCells.Clear();
        }
    }

    public void Supertest1()
    {
        Debug.Log("SUPERTEST 1");
    }

    public void Supertest2()
    {
        Debug.Log("SUPERTEST 2");
    }


}
