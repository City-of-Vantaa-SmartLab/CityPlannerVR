using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenCommentTool : MonoBehaviour {

    GameObject player;
    GameObject playerAvatar;

    LaserPointer laser;
    PhotonLaserManager photonLaser;

    GameObject commentTool;
    GameObject commentOutput;
    PlayComment playComment;
    RecordComment recordComment;

    string commentToolTag = "CommentToolTag";
    string buttonTag = "Button";
    string commentObjectTag = "Building";
    CheckPlayerSize playerSize;

    void Start()
    {
        player = GameObject.Find("Player");
        playerSize = player.GetComponent<CheckPlayerSize>();

        playerAvatar = PhotonPlayerAvatar.LocalPlayerInstance;

        laser = GetComponent<LaserPointer>();
        photonLaser = GetComponent<PhotonLaserManager>();

        commentTool = GameObject.Find("CommentTool");
        commentOutput = GameObject.Find("CommentList");
        playComment = commentOutput.GetComponent<PlayComment>();
        recordComment = commentTool.GetComponentInChildren<RecordComment>();

        //PointerIn += OnHoverButtonEnter;
        laser.PointerIn += OpenCommentOutputPanel;
        //PointerIn += ActivateCommentTool;
        laser.PointerIn += HideCommentTool;

        //PointerOut += OnHoverButtonExit;
        laser.PointerOut += CheckIfHiding;

        //So the RecordPlayers Start can happen before it is disabled
        Invoke("DisableCommentTool", 0);
    }

    private void DisableCommentTool()
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

    public void OpenCommentOutputPanel(object sender, LaserEventArgs e)
    {
        if (e.target.tag == commentToolTag && e.target.name == "Empty")
        {
            commentOutput.SetActive(true);
            playComment.LoadComments();
        }
    }

    public void ActivateCommentTool(LaserPointer laser, GameObject target)
    {
        if (target.tag == commentObjectTag || target.tag == "Spawnable")
        {
            playComment.pointedTarget = target.gameObject;
            recordComment.target = target.gameObject;
            commentTool.SetActive(true);

            if (playerSize.isSmall)
            {
				commentTool.transform.position = (laser.hitPoint - playerAvatar.transform.position)/6 + playerAvatar.transform.position;
                commentTool.transform.localScale = player.transform.localScale;
            }
            else
            {
                //CommentTool position
                commentTool.transform.position = laser.hitPoint - laser.direction;
                commentTool.transform.localScale = new Vector3(1, 1, 1);
            }

            commentTool.transform.LookAt(gameObject.transform);
            

            CommentToolManager commentToolManager;
            commentToolManager = commentTool.GetComponent<CommentToolManager>();
            commentToolManager.targetName = target.name;
        }
    }

    bool closeCommentOutput = false;
    bool closeCommentTool = false;

    //Hides objects when the laser doesn't hit them anymore
    public void CheckIfHiding(object sender, LaserEventArgs e)
    {
        if (e.target.name == commentOutput.name || e.target.name == "Empty")
        {
            //if we are closing just the list of comments
            closeCommentOutput = true;
        }
        else if (e.target.tag == commentToolTag)
        {
            //if we are closing the whole comment wheel
            closeCommentTool = true;
        }
    }

    public void HideCommentTool(object sender, LaserEventArgs e)
    {
        if (closeCommentOutput && e.target.tag == commentToolTag && e.target.tag == buttonTag && e.target.name != commentOutput.name)
        {
            //if we are closing just the list of comments
            commentOutput.SetActive(false);
            closeCommentOutput = false;
        }
        else if (closeCommentTool && e.target.tag != commentToolTag && e.target.tag != buttonTag)
        {
            //if we are closing the whole comment wheel
            commentOutput.SetActive(false);
            commentTool.SetActive(false);
            closeCommentTool = false;
        }
    }
}
