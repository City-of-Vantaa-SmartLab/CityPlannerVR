using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable] //attributes for json
public class CommentData
{
    //CommentMetaData metaData;
    public string dataString;
    public Comment.CommentType type;

    public string userName; //player
    public GameObject commentedObject;
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
    public GameObject _commentedObject;
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
        _commentedObject = target;
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
        _data.commentedObject = _commentedObject;
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
        _commentedObject = _data.commentedObject;
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
                SaveData.commentLists.textComments.Add(this);
                break;

            case Comment.CommentType.Voice:
                SaveData.commentLists.voiceComments.Add(this);
                break;

            case Comment.CommentType.Thumb:
                SaveData.commentLists.thumbComments.Add(this);
                break;

            default:
                Debug.Log("Type not set for comment " + this.name + " by user " + this._userName + " while being sorted!");
                break;
        }
    }

}
