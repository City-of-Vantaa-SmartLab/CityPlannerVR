using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetLocalPlayerName : MonoBehaviour {

    //The person who commented
    string commenter;
    GameObject commentWheel;
    RecordComment recordComment;

    private Dissonance.VoiceBroadcastTrigger voiceTrigger;

    void Awake()
    {
        commentWheel = GameObject.Find("CommentTool");
        recordComment = commentWheel.GetComponentInChildren<RecordComment>();

        voiceTrigger = gameObject.GetComponent<Dissonance.VoiceBroadcastTrigger>();
        recordComment.voiceTrigger = voiceTrigger;

        GetPlayerName();
    }

   public void GetPlayerName()
    {
        commenter = gameObject.GetComponent<PhotonView>().owner.NickName;
        recordComment.commenter = commenter;
        GameObject.Find("GameController").GetComponent<SaveAndLoadComments>().localPlayerName = commenter;
    }
}
