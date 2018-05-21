﻿//Based on SteamVR_LaserPointer.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public struct LaserEventArgs
{
    //public uint handNumber;
    public float distance;
    public Transform target;
    public Vector3 hitPoint;
}

public delegate void LaserEventHandler(object sender, LaserEventArgs e);


public class LaserPointer : PunBehaviour
{
    public bool active = true;
    public bool triggered;
    public Color editorColor;
    public Color fakeColor;
    public float thickness = 0.002f;
    public GameObject holder;
    public GameObject pointer;
    bool isActive = false;
    public bool isForNetworking; //means it is "fake" and does not show for the local player
    public event LaserEventHandler PointerIn;
    public event LaserEventHandler PointerOut;

    private InputMaster inputMaster;

    Transform previousContact = null;

    //For the whispering to work, we give this laser to photonAvatar (set in photonPlayerAvatar)
    [HideInInspector]
    public VoiceController voiceController;

    GameObject commentTool;
    GameObject commentOutput;
    PlayComment playComment;

    RecordComment recordComment;

    string commentToolTag = "CommentToolTag";
    string commentObjectTag = "Building";

    Ray raycast;

    private void Awake()
    {
        commentTool = GameObject.Find("CommentTool");
        commentOutput = GameObject.Find("CommentList");
        playComment = commentOutput.GetComponent<PlayComment>();

        holder = new GameObject();
        holder.transform.parent = this.transform;
        holder.transform.localPosition = Vector3.zero;
        holder.transform.localRotation = Quaternion.identity;

        pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pointer.transform.parent = holder.transform;
        pointer.transform.localScale = new Vector3(thickness, thickness, 100f);
        pointer.transform.localPosition = new Vector3(0f, 0f, 50f);
        pointer.transform.localRotation = Quaternion.identity;

        Material newMaterial = new Material(Shader.Find("Unlit/Color"));
        if (!isForNetworking)
            newMaterial.SetColor("_Color", editorColor);
        else
            newMaterial.SetColor("_Color", fakeColor);

        pointer.GetComponent<MeshRenderer>().material = newMaterial;

        BoxCollider collider = pointer.GetComponent<BoxCollider>();
        if (collider)
        {
            Object.Destroy(collider);
        }

        triggered = false;

        //Invoke("StartNetworking", 1f);
    }

    private void Start()
    {
        if (isForNetworking)
        {
            bool status = false; //0: active, 1: isInEditingMode

            PhotonLaserManager photonLaserManager;
            if (gameObject.name == "PhotonHandLeft")
            {
                photonLaserManager = GameObject.Find("Player/SteamVRObjects/Hand1/Laserpointer").GetComponent<PhotonLaserManager>();
            }
            else if (gameObject.name == "PhotonHandRight")
            {
                photonLaserManager = GameObject.Find("Player/SteamVRObjects/Hand2/Laserpointer").GetComponent<PhotonLaserManager>();
            }
            else
            {
                Debug.LogError("Could not determine photonlasermanager for laserpointer in " + gameObject.name);
                return;
            }
            photonLaserManager.myFakeLaser = this;
            photonView.RPC("ActivateFakeLaser", PhotonTargets.AllBuffered, status);
        }

        commentOutput.SetActive(false);
        commentTool.SetActive(false);

        PointerIn += OnHoverButtonEnter;
        PointerIn += OpenCommentOutputPanel;
        PointerIn += ActivateCommentTool;
        PointerIn += HideCommentTool;

        PointerOut += OnHoverButtonExit;
        PointerOut += CheckIfHiding;
    }

    public virtual void OnPointerIn(LaserEventArgs e)
    {
        if (!isForNetworking && PointerIn != null && active)
            PointerIn(this, e);
    }

    public virtual void OnPointerOut(LaserEventArgs e)
    {
        if (!isForNetworking && PointerOut != null && active)
            PointerOut(this, e);
    }


    // Update is called once per frame
    void Update()
    {
        //if (!isActive)
        //{
        //    isActive = true;
        //    this.transform.GetChild(0).gameObject.SetActive(true);
        //}

        float dist = 100f;

        raycast = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        bool bHit = Physics.Raycast(raycast, out hit);

        if (previousContact && previousContact != hit.transform)
        {
            LaserEventArgs args = new LaserEventArgs();
            args.distance = 0f;
            args.target = previousContact;
            args.hitPoint = Vector3.zero;
            OnPointerOut(args);
            previousContact = null;
        }
        if (bHit && previousContact != hit.transform)
        {
            LaserEventArgs argsIn = new LaserEventArgs();
            argsIn.distance = hit.distance;
            argsIn.target = hit.transform;
            argsIn.hitPoint = hit.point;
            OnPointerIn(argsIn);
            previousContact = hit.transform;
        }
        if (!bHit)
        {
            previousContact = null;
        }
        if (bHit && hit.distance < 100f)
        {
            dist = hit.distance;
        }

        if (triggered)
        {
            pointer.transform.localScale = new Vector3(thickness * 5f, thickness * 5f, dist);
        }
        else
        {
            pointer.transform.localScale = new Vector3(thickness, thickness, dist);
        }

        pointer.transform.localPosition = new Vector3(0f, 0f, dist / 2f);
    }

    public void ActivateCube(bool active)
    {
            pointer.SetActive(active);
    }

    public void ActivateFakeLaserRPC(bool status)
    {
        photonView.RPC("ActivateFakeLaser", PhotonTargets.OthersBuffered, status);
    }

    //Will only be sent to other clients (except when laserpointer is initialized)
    //0: active, 1: isInEditingMode
    [PunRPC]
    private void ActivateFakeLaser(bool status, PhotonMessageInfo info)
    {
        if (info.sender == photonView.owner)
        {
            ActivateCube(status);
        }

    }

    //------------------------------------------------------------------------------------------------------------------------------
    //Comment stuff
    //------------------------------------------------------------------------------------------------------------------------------

    private void OnHoverButtonEnter(object sender, LaserEventArgs e)
    {
        if (e.target.tag == "Button")
        {
            e.target.gameObject.GetComponent<UnityEngine.UI.Image>().color = Color.blue;
            
        }
    }

    private void OnHoverButtonExit(object sender, LaserEventArgs e)
    {
        if (e.target.tag == "Button")
        {
            e.target.gameObject.GetComponent<UnityEngine.UI.Image>().color = Color.green;
        }
    }

    private void OpenCommentOutputPanel(object sender, LaserEventArgs e)
    {
        if(e.target.tag == commentToolTag && e.target.name == "Empty")
        {
            commentOutput.SetActive(true);
            playComment.LoadComments();
        }
    }

    void ActivateCommentTool(object sender, LaserEventArgs e)
    {
        if(e.target.tag == commentObjectTag)
        {
            if (recordComment == null)
            {
                recordComment = GameObject.Find("PhotonAvatar(Clone)").GetComponent<RecordComment>();
            }

            recordComment.target = e.target.gameObject;
            commentTool.SetActive(true);
            commentTool.transform.position = e.hitPoint - raycast.direction;
            commentTool.transform.LookAt(gameObject.transform);
        }
    }

    bool closeCommentOutput = false;
    bool closeCommentTool = false;

    //Hides objects when the laser doesn't hit them anymore
    void CheckIfHiding(object sender, LaserEventArgs e)
    {
        if (e.target.name == commentOutput.name || e.target.name == "Empty")
        {
            closeCommentOutput = true;
        }
        else if (e.target.tag == commentToolTag)
        {
            closeCommentTool = true;
        }
    }

    void HideCommentTool(object sender, LaserEventArgs e)
    {
        if (closeCommentOutput && e.target.tag == commentToolTag && e.target.name != commentOutput.name)
        {
            commentOutput.SetActive(false);
            closeCommentOutput = false;
        }
        else if (closeCommentTool && e.target.tag != commentToolTag)
        {
            commentOutput.SetActive(false);
            commentTool.SetActive(false);
            closeCommentTool = false;
        }
    }
}
