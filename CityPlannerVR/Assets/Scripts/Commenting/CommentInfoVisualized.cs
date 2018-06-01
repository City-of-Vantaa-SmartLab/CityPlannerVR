using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommentInfoVisualized : MonoBehaviour {

    public GameObject testImage;
    public GameObject textCell;

    public Comment CurrentComment
    {
        get
        {
            return CurrentComment;
        }
        set
        {
            CurrentComment = value;
            GenerateInfo();
        }
    }

    private List<GameObject> textCells;

    private void GenerateInfo()
    {
        //if (textCells.Count > 0)
        //{
        //    textCells.ForEach(Destroy);
        //textCells.Clear();
        //}
        Debug.Log("Generating info");

        GenerateTextCell(CurrentComment._data.userName);
        GenerateTextCell(CurrentComment._data.submittedShortTime);
        GenerateTextCell(CurrentComment._data.dataString);

        GenerateTextCell(CurrentComment._data.commentedObjectName);
        GenerateTextCell(CurrentComment._data.quickCheck.ToString());
        GenerateTextCell(CurrentComment._data.userName);

        GenerateTextCell(CurrentComment._data.userName);
        GenerateTextCell(CurrentComment._data.userName);
        GenerateTextCell(CurrentComment._data.userName);
    }

    public void GenerateTestImages()
    {
        if (testImage)
        {
            for (int i = 0; i < 5; i++)
            {
                Instantiate(testImage, transform, false);
            }
        }
        else
            Debug.Log("No test image set!");
    }

    //public void GenerateTestText()
    //{
    //    if (textCell)
    //    {
    //        GameObject cell = Instantiate(textCell, transform, false);
    //        Text cellText = cell.transform.GetChild(0).GetComponent<Text>();
    //        cellText.text = "Testing is fun!";
    //    }
    //    else
    //        Debug.Log("No test text set!");
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
            textCells.Add(cell);
        }
        else
        {
            Debug.LogError("No textcell prefab set or found!");
        }
    }

}
