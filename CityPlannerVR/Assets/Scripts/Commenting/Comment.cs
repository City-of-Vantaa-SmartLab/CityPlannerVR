using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

[Serializable] //attributes for json
public class CommentData
{
    //CommentMetaData metaData;
    public string dataString;
    public Comment.CommentType type;

    public string userName; //player
    public string commentedObjectName;
    public string SHPath;
    
    public string submittedLongDate;
    public Vector3 commentatorPosition;
    public int quickCheck;
}

//From https://gamedev.stackexchange.com/questions/137523/unity-json-utility-does-not-serialize-datetime


[Serializable]
public class Comment {

    public enum CommentType { None, Text, Thumb, Voice };

    //public CommentType CurrentType { get; set; }
    public CommentData data; //used for storing and loading data


    //default constructor
    public Comment()
    {
        data = null;
        OnEnable();
        //Debug.Log("Comment created");
    }

    //Not a monobehaviour!
    private void OnEnable()
    {
        //SaveData.OnLoaded += LoadData;
        //SaveData.OnBeforeSave += StoreData;
        SaveData.OnBeforeSave += ApplyDataToContainer;
    }

    //Not a monobehaviour!
    private void OnDisable()
    {
        //SaveData.OnLoaded -= LoadData;
        //SaveData.OnBeforeSave -= StoreData;
        SaveData.OnBeforeSave -= ApplyDataToContainer;
    }

    public void ApplyDataToContainer()
    {
        SaveData.AddCommentData(data);
        OnDisable();
    }

    public void SortAndAddToLocalList()
    {
        switch (data.type)
        {
            case Comment.CommentType.Text:
                if (!IsCommentInList(SaveData.commentLists.textComments))
                    SaveData.commentLists.textComments.Add(this);
                break;

            case Comment.CommentType.Voice:
                if (!IsCommentInList(SaveData.commentLists.textComments))
                    SaveData.commentLists.voiceComments.Add(this);
                break;

            case Comment.CommentType.Thumb:
                if (!IsCommentInList(SaveData.commentLists.textComments))
                    SaveData.commentLists.thumbComments.Add(this);
                break;

            default:
                Debug.Log("Type not set for comment (quickcheck: " + data.quickCheck + ") by user " + this.data.userName + " while being sorted!");
                break;
        }
    }

    public bool IsCommentInList(List<Comment> testList)
    {
        Comment temp;
        for (int i = 0; i < SaveData.commentLists.textComments.Count; i++)
        {
            temp = SaveData.commentLists.textComments[i];
            if (IsTheSameComment(temp))
            {
                    return true;
            }
        }
        return false;
    }

    public bool IsTheSameComment(Comment testComment)
    {
        if (data.quickCheck == testComment.data.quickCheck &&  //comparing ints is quicker than strings
            data.userName == testComment.data.userName &&
            data.commentedObjectName == testComment.data.commentedObjectName &&
            data.dataString == testComment.data.dataString
            )
            return true;
        else
            return false;
    }

    private int ConvertFirstCharsToInt(string str, int maxLength)
    {
        string newStr = TruncateString(str, maxLength);
        byte[] bytes = Encoding.Default.GetBytes(newStr);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        int magic = BitConverter.ToInt32(bytes, 0);
        return magic;
    }

    public void GenerateQuickCheck(int subStringMaxLength)
    {
        string userName = TruncateString(data.userName, subStringMaxLength);
        string objectName = TruncateString(data.commentedObjectName, subStringMaxLength);
        string date = TruncateString(data.submittedLongDate, subStringMaxLength);
        string uberString = userName + objectName + date;
        Debug.Log("Joining strings: " + userName + " " + objectName + " " + date);
        int magic = ConvertFirstCharsToInt(uberString, subStringMaxLength * 4);
        Debug.Log("QuickCheck: " + magic);
        data.quickCheck = magic;
    }

    public void ConvertToQuickCheck(int maxLength, CommentData data)
    {
        string userName = TruncateString(data.userName, maxLength);
        string objectName = TruncateString(data.commentedObjectName, maxLength);
        string date = TruncateString(data.submittedLongDate, maxLength);
        string uberString = userName + objectName + date;
        Debug.Log("Joining strings: " + userName + " " + objectName + " " + date);
        int magic = ConvertFirstCharsToInt(uberString, maxLength * 4);
        Debug.Log("Adding QuickCheck to data: " + magic);
        data.quickCheck = magic;
    }

    private string TruncateString(string str, int maxLength)
    {
        int length = str.Length;
        if (length > maxLength)
            length = maxLength;
        string newStr = str.Substring(0, length);
        return newStr;
    }

}
