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
    public int quickCheck;
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
    public int _quickCheck;


    //default constructor
    //public Comment()
    //{
    //    _submittedTime = System.DateTime.Now;
    //}

    ////constructor
    //public Comment(GameObject user, GameObject target, string screenshotPath, CommentType type, string dataString, DateTime submittedTime)
    //{
    //    _dataString = dataString;
    //    CommentT = type;

    //    _SHPath = screenshotPath;
    //    _userName = user.name;
    //    _commentedObjectName = target.name;
    //    _submittedTime = submittedTime;
    //    _pos = user.transform.position;
    //    _quickCheck = ConvertToQuickCheck(2);
    //}


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
        _data.quickCheck = _quickCheck;
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
        _quickCheck = _data.quickCheck;
    }

    //called by OnBeforeSave event
    public void ApplyDataToContainer()
    {
        SaveData.AddCommentData(_data);
    }

    public void SortAndAddToLocalList()
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
        if (_quickCheck == testComment._quickCheck &&  //comparing ints is quicker than strings
            _userName == testComment._userName &&
            _commentedObjectName == testComment._commentedObjectName &&
            _dataString == testComment._dataString
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

    public int ConvertToQuickCheck(int maxLength)
    {
        string userName = TruncateString(_userName, maxLength);
        string objectName = TruncateString(_commentedObjectName, maxLength);
        string date = TruncateString(_submittedTime.ToShortDateString() , maxLength);
        string uberString = userName + objectName + date;
        Debug.Log("Joining strings: " + userName + " " + objectName + " " + date);
        int magic = ConvertFirstCharsToInt(uberString, maxLength * 4);
        Debug.Log("QuickCheck: " + magic);
        return magic;
    }

    public void ConvertToQuickCheck(int maxLength, CommentData data)
    {
        string userName = TruncateString(data.userName, maxLength);
        string objectName = TruncateString(data.commentedObjectName, maxLength);
        string date = TruncateString(data.submittedTime.ToShortDateString(), maxLength);
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
