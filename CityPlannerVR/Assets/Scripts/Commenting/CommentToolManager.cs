using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Use this to find relevant information for comments, eg. commented object name etc.
/// </summary>

public class CommentToolManager : MonoBehaviour {

    public string localPlayerName;
    public string targetName;
    //public LaserEventArgs LEArgs;
    public object sender;

    void Start () {
        localPlayerName = PhotonNetwork.player.NickName;
        //LEArgs = null;
        sender = null;
	}
	
}
