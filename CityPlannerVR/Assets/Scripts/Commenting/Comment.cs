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
    public System.DateTime submittedTime;
    public Vector3 commentatorPosition;
}

public class Comment : MonoBehaviour {

    public enum CommentType { None, Text, Thumb, Voice };

    public CommentType CommentT
    {
        get
        {
            return currentType;
        }
        set
        {
            currentType = value;
        }
    }

    private CommentType currentType;
    public string _dataString; //comment's text or a filepath for voice files
    public CommentData _data; //used for storing and loading data

    public string _userName; //player
    public string _commentedObjectName;
    public string _SHPath; //path to the screenshot that was taken with the commit
    public System.DateTime _submittedTime;
    public Vector3 _pos;

    //default constructor
    public Comment()
    {
        _submittedTime = System.DateTime.Now;
    }

    //constructor
    public Comment(GameObject user, GameObject target, string screenshotPath, CommentType type, string dataString)
    {
        _dataString = dataString;
        CommentT = type;

        _SHPath = screenshotPath;
        _userName = user.name; //mieluummin haetaan photonin kautta, vähemmän parametrejä 
        _commentedObjectName = target.name;
        _submittedTime = System.DateTime.Now;
        _pos = user.transform.position;
    }


    private void OnEnable()
    {
        SaveData.OnLoaded += LoadData;
        SaveData.OnBeforeSave += StoreData;
        SaveData.OnBeforeSave += ApplyDataToContainer;
    }

    private void OnDisable()
    {
        SaveData.OnLoaded -= LoadData;
        SaveData.OnBeforeSave -= StoreData;
        SaveData.OnBeforeSave -= ApplyDataToContainer;
    }

    //Needed only if the content is changed during runtime
    public void StoreData()
    {
        _data.dataString = _dataString;
        _data.type = CommentT;

        _data.userName = _userName;
        _data.commentedObjectName = _commentedObjectName;
        _data.SHPath = _SHPath;
        _data.submittedTime = _submittedTime;
        _data.commentatorPosition = _pos;
    }

    //Loads variables from CommentData, should be used after initalization (if not created by user)
    public void LoadData()
    {
        _dataString = _data.dataString;
        CommentT = _data.type;

        _userName = _data.userName;
        _commentedObjectName = _data.commentedObjectName;
        _SHPath = _data.SHPath;
        _submittedTime = _data.submittedTime;
        _pos = _data.commentatorPosition;
    }

    //called by OnBeforeSave event
    public void ApplyDataToContainer()
    {
        SaveData.AddCommentData(_data);
    }

    public void SortAndAddToList()
    {
        switch (_data.type)
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
                Debug.Log("Type not set for comment " + this.name + " by user " + this._userName + " while being sorted!");
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
        if (_userName == testComment._userName &&
            _submittedTime == testComment._submittedTime &&
            _commentedObjectName == testComment._commentedObjectName &&
            _dataString == testComment._dataString
            )
            return true;
        else
            return false;
    }

    public int ConvertFirstCharsToInt(string str)
    {
        int maxLength = 5; //reduce this if too taxing
        int length = str.Length;
        if (length > maxLength)
            length = maxLength;
        string newStr = str.Substring(0, length);

        byte[] bytes = Encoding.Default.GetBytes(newStr);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        int magic = BitConverter.ToInt32(bytes, 0);

        return magic;
    }

}
