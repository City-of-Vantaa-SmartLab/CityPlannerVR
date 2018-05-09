using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommentData : MonoBehaviour {

    public class CommentMetaData
    {
        public string userName; //player
        public GameObject commentedObject;
        public string SHPath;
        public System.DateTime submittedTime;
    }

    public enum CommentType { None, Text, Peukutus, Voice };
    public int toolRights;

    public CommentType Comment
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


    public CommentMetaData _metaData;
    public string _data;
    
    public CommentData(CommentMetaData metaData, CommentType type, string data)
    {
        _metaData = metaData;
        _data = data;
        Comment = type;

        //switch (type)  //no generalisation needed after all
        //{
        //    case CommentType.Text:
        //        break;
        //    case CommentType.Peukutus:
        //        break;
        //    case CommentType.Voice:
        //        break;

        //    default:
        //        Debug.LogError("No type set for comment!");
        //        break;
        //}
    }

    public CommentMetaData CreateMetaData(GameObject user, GameObject target, string screenshotPath)
    {
        CommentMetaData metaData = new CommentMetaData
        {
            SHPath = screenshotPath,
            userName = user.name, //mieluummin haetaan photonin kautta, vähemmän parametrejä
            commentedObject = target,
            submittedTime = System.DateTime.Now
        };
        return metaData;
    }
}

