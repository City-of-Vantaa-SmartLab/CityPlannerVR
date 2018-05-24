using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A test class for thumbs button, will be removed?
/// </summary>

public class ThumbManager : MonoBehaviour {

    private SaveAndLoadComments SLComments;
    private GameObject player;
    private GameObject target;

    private void Start()
    {
        SLComments = GameObject.Find("GameController").GetComponent<SaveAndLoadComments>();
    }

    public void Test()
    {
        Debug.Log("THUMBS MANAGER TESTAA");
    }

    public void CreateThumbUp()
    {
        CommentData tempData = CreateThumbData();
        SLComments.CreateNewComment(tempData);
    }

    public void CreateThumbDown()
    {

    }

    private CommentData CreateThumbData()
    {
        GameObject commenter = GetPlayer();
        GameObject target = GetTarget();
        CommentData tempData = new CommentData();

        //tempData.userName = PhotonNetwork.player.NickName; //check with PhotonNetwork.player.IsLocal ?
        tempData.userName = commenter.GetComponent<PhotonView>().owner.NickName;
        tempData.commentedObjectName = target.name;
        tempData.SHPath = "";
        tempData.submittedTime = System.DateTime.Now;
        tempData.commentatorPosition = commenter.transform.position;
        tempData.type = Comment.CommentType.Thumb;
        
        tempData.quickCheck = 0; //will be created in saveAndLoadComments script

        
            //string dataString;
            //Comment.CommentType type;

            //string userName; //player
            //string commentedObjectName;
            //string SHPath;
            //System.DateTime submittedTime;
            //Vector3 commentatorPosition;
            //int quickCheck;


            return tempData;
    }

    private GameObject GetPlayer()
    {
        throw new NotImplementedException();
    }

    private GameObject GetTarget()
    {
        throw new NotImplementedException();
    }
}
