//Based on SteamVR_LaserPointer.cs
using UnityEngine;
using Photon;

/// <summary>
/// 
/// This script will use the first child gameobject as the pointer cube or create a new one.
/// </summary>

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
    public bool isForNetworking; //means it is "fake" and does not show for the local player
    public event LaserEventHandler PointerIn;
    public event LaserEventHandler PointerOut;

    Transform previousContact = null;

    //For the whispering to work, we give this laser to photonAvatar (set in photonPlayerAvatar)
    [HideInInspector]
    VoiceController voiceController;

    GameObject commentTool;
    GameObject commentOutput;
    PlayComment playComment;
    RecordComment recordComment;

	string commentToolTag = "Notusednow";//"CommentToolTag";
    string buttonTag = "Button";
	string commentObjectTag = "Notusednow";//"Building";

    GameObject player;
    CheckPlayerSize checkPlayerSize;

    Ray raycast;

    private void Awake()
    {
        commentTool = GameObject.Find("CommentTool");
        commentOutput = GameObject.Find("CommentList");
        playComment = commentOutput.GetComponent<PlayComment>();

        player = GameObject.Find("Player");
        checkPlayerSize = player.GetComponent<CheckPlayerSize>();

        InitHolder(transform);
        InitPointer(holder.transform);

        BoxCollider collider = pointer.GetComponent<BoxCollider>();
        if (collider)
        {
            Destroy(collider);
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

        recordComment = commentTool.GetComponentInChildren<RecordComment>();

        //So the RecordPlayers Start can happen before it is disabled
        Invoke("DisableCommentTool", 0);
        
        //PointerIn += OnHoverButtonEnter;
        PointerIn += OpenCommentOutputPanel;
        //PointerIn += ActivateCommentTool;
        PointerIn += HideCommentTool;

        //PointerOut += OnHoverButtonExit;
        PointerOut += CheckIfHiding;
    }

    private void DisableCommentTool()
    {
        commentOutput.SetActive(false);
        commentTool.SetActive(false);
    }

    private void InitHolder(Transform targetTransform)
    {
        if (targetTransform.childCount > 0)
            holder = targetTransform.GetChild(0).gameObject;

        if (holder == null)
        {
            holder = new GameObject();
            holder.transform.parent = this.transform;
            holder.transform.localPosition = Vector3.zero;
            holder.transform.localRotation = Quaternion.identity;
        }
    }


    public void InitPointer(Transform targetTransform)
    {

        if (pointer == null)
        {
            pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pointer.transform.parent = targetTransform;
            pointer.transform.localScale = new Vector3(thickness, thickness, 100f);
            pointer.transform.localPosition = new Vector3(0f, 0f, 50f);
            pointer.transform.localRotation = Quaternion.identity;
        }
        Material newMaterial = new Material(Shader.Find("Unlit/Color"));
        if (!isForNetworking)
            newMaterial.SetColor("_Color", editorColor);
        else
            newMaterial.SetColor("_Color", fakeColor);

        pointer.GetComponent<MeshRenderer>().material = newMaterial;
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
        if (!active)
            return;

        if (pointer == null)
        {
        InitPointer(transform);
        }

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
        holder.SetActive(active);
        //pointer.SetActive(active);
    }

    public void ActivateFakeLaserRPC(bool status)
    {
        photonView.RPC("ActivateFakeLaser", PhotonTargets.AllBuffered, status);
    }

    //Will only be sent to other clients (except when laserpointer is initialized)
    //0: active, 1: isInEditingMode
    [PunRPC]
    private void ActivateFakeLaser(bool status, PhotonMessageInfo info)
    {
        //Debug.Log("info sender and photonview owner: " + info.sender + " " + photonView.owner);
        if (info.sender == photonView.owner)
        {
            if (status == true && !photonView.isMine)
            {
                ActivateCube(true);
            }
            else
                ActivateCube(false);
        }

    }

    //------------------------------------------------------------------------------------------------------------------------------
    //Comment stuff
    //------------------------------------------------------------------------------------------------------------------------------

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

    private void OpenCommentOutputPanel(object sender, LaserEventArgs e)
    {
        if(e.target.tag == commentToolTag && e.target.name == "Empty")
        {
            commentOutput.SetActive(true);
            playComment.LoadComments();
        }
    }

    public void ActivateCommentTool(object sender, LaserEventArgs e)
    {
        if (e.target.tag == commentObjectTag || e.target.tag == "Props")
        {
            playComment.pointedTarget = e.target.gameObject;
            recordComment.target = e.target.gameObject;
            commentTool.SetActive(true);

            if(gameObject.transform.eulerAngles.y >= -45 && gameObject.transform.eulerAngles.y < 45)
            {
                commentTool.transform.position = gameObject.transform.position + new Vector3(0, player.transform.localScale.y, player.transform.localScale.z);
            }
            else if(gameObject.transform.eulerAngles.y >= 45 && gameObject.transform.eulerAngles.y < 135)
            {
                commentTool.transform.position = gameObject.transform.position + new Vector3(player.transform.localScale.x, player.transform.localScale.y, 0);
            }
            else if (gameObject.transform.eulerAngles.y >= 135 && gameObject.transform.eulerAngles.y < -135)
            {
                commentTool.transform.position = gameObject.transform.position + new Vector3(0, player.transform.localScale.y, -player.transform.localScale.z);
            }
            else
            {
                commentTool.transform.position = gameObject.transform.position + new Vector3(-player.transform.localScale.x, player.transform.localScale.y, 0);
            }
            commentTool.transform.LookAt(gameObject.transform);
            commentTool.transform.localScale = player.transform.localScale;

            CommentToolManager commentToolManager;
            commentToolManager = commentTool.GetComponent<CommentToolManager>();
            commentToolManager.targetName = e.target.name;

            //commentToolManager.sender = sender;
            //commentToolManager.LEArgs = e;
        }
    }

    bool closeCommentOutput = false;
    bool closeCommentTool = false;

    //Hides objects when the laser doesn't hit them anymore
    void CheckIfHiding(object sender, LaserEventArgs e)
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

    void HideCommentTool(object sender, LaserEventArgs e)
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
