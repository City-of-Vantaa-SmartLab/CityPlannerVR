using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenCommentTool : MonoBehaviour {

    GameObject playerAvatar;

    GameObject commentTool;
    GameObject commentOutput;
    PlayComment playComment;
    RecordComment recordComment;

    CheckPlayerSize playerSize;

    void Start()
    {
        playerAvatar = PhotonPlayerAvatar.LocalPlayerInstance;

        commentTool = GameObject.Find("Comments");
        playComment = commentOutput.GetComponent<PlayComment>();
        recordComment = commentTool.GetComponentInChildren<RecordComment>();

        //So both hands get the references before commentTool is disabled
        //TODO: TARKISTA ONKO TARPEELLINEN
        Invoke("DisableCommentTool", 1);
    }

    void DisableCommentTool()
    {
        commentOutput.SetActive(false);
        commentTool.SetActive(false);
    }

    //private void OnHoverButtonEnter(object sender, LaserEventArgs e)
    //{
    //    if (e.target.tag == buttonTag)
    //    {
    //        e.target.gameObject.GetComponent<UnityEngine.UI.Image>().color = Color.blue;

    //    }
    //}

    //private void OnHoverButtonExit(object sender, LaserEventArgs e)
    //{
    //    if (e.target.tag == buttonTag)
    //    {
    //        e.target.gameObject.GetComponent<UnityEngine.UI.Image>().color = Color.green;
    //    }
    //}

    public void OpenCommentOutputPanel()
    {
        
        commentOutput.SetActive(true);
        //playComment.LoadComments();
        
    }

    public void ActivateCommentTool()
    {
        //playComment.pointedTarget = HoverTabletManager.commentTarget.gameObject;
        commentTool.SetActive(true);

        //CommentTool position
        commentTool.transform.position = transform.position;
        commentTool.transform.localScale = Vector3.one;
        

        commentTool.transform.LookAt(gameObject.transform);


        CommentToolManager commentToolManager;
        commentToolManager = commentTool.GetComponent<CommentToolManager>();
        commentToolManager.targetName = HoverTabletManager.commentTarget.name;
    }

    public void HideCommentTool()
    {
        if(commentOutput.GetActive() == true)
        {
            //if we are closing the whole comment wheel
            commentOutput.SetActive(false);
            commentTool.SetActive(false);
        }
    }
}
