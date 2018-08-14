﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A test class for thumbs button, will be removed?
/// </summary>

public class ThumbManager : MonoBehaviour {

    private SaveAndLoadComments SLComments;
    private CommentToolManager commentToolManager;
    private GameObject player;
    //private GameObject target;
    private string commentLayer = "CommentTool";

    private void Start()
    {
        SLComments = GameObject.Find("GameController").GetComponent<SaveAndLoadComments>();
        commentToolManager = transform.GetComponentInParent<CommentToolManager>();
    }

    public void Test()
    {
        Debug.Log("THUMBS MANAGER TESTAA");
    }

    //public void CreateThumbUp()
    //{
    //    CommentData tempData = CreateThumbData();
    //    tempData.dataString = "1";
    //    SLComments.CreateNewComment(tempData);
    //}

    //public void CreateThumbDown()
    //{
    //    CommentData tempData = CreateThumbData();
    //    tempData.dataString = "0";
    //    SLComments.CreateNewComment(tempData);
    //}

    //needs to be cleaned up
    private CommentData CreateThumbData()
    {
        String userName = PhotonNetwork.player.NickName;
        //string targetName = commentToolManager.LEArgs.target.name;

        CommentData tempData = new CommentData();

        //tempData.userName = PhotonNetwork.player.NickName; //check with PhotonNetwork.player.IsLocal ?
        //tempData.userName = commenter.GetComponent<PhotonView>().owner.NickName;
        tempData.userName = userName;
        tempData.commentedObjectName = commentToolManager.targetName;
        tempData.SHPath = "";
        //tempData.commentatorPosition = player.transform.position;
        tempData.commentType = Comment.CommentType.Thumb;
        //tempData.type = 2;

        tempData.quickcheck = 0; //will be created in saveAndLoadComments script

        return tempData;
    }

    //private GameObject GetTarget(object sender, LaserEventArgs e)
    //{
    //    if (e.target.gameObject.layer != LayerMask.NameToLayer(commentLayer))
    //    {
    //        return e.target.gameObject;
    //    }
    //    else
    //        return null;
    //}

}
