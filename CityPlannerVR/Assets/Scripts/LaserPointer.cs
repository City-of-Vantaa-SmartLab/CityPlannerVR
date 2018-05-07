//Based on SteamVR_LaserPointer.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public struct LaserEventArgs
{
    //public uint handNumber;
    public float distance;
    public Transform target;
}

public delegate void LaserEventHandler(object sender, LaserEventArgs e);


public class LaserPointer : PunBehaviour
{
    public bool active = true;
    public bool triggered;
    public Color editorColor;
    public Color commentColor;
    public float thickness = 0.002f;
    public GameObject holder;
    public GameObject pointer;
    bool isActive = false;
    public bool isForNetworking; //means it is "fake" and does not show for the local player
    public bool isInEditingMode;  //comment or edit mode
    public event LaserEventHandler PointerIn;
    public event LaserEventHandler PointerOut;

    [SerializeField]
    private InputMaster inputMaster;

    Transform previousContact = null;

    //For the whispering to work, we give this laser to photonAvatar (set in photonPlayerAvatar)
    [HideInInspector]
    public VoiceController voiceController;

    private void Awake()
    {
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
        if (isInEditingMode)
            newMaterial.SetColor("_Color", editorColor);
        else
            newMaterial.SetColor("_Color", commentColor);

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
            bool[] status = new bool[2] { false, false }; //0: active, 1: isInEditingMode

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


        Ray raycast = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        bool bHit = Physics.Raycast(raycast, out hit);

        if (previousContact && previousContact != hit.transform)
        {
            LaserEventArgs args = new LaserEventArgs();
            args.distance = 0f;
            args.target = previousContact;
            OnPointerOut(args);
            previousContact = null;
        }
        if (bHit && previousContact != hit.transform)
        {
            LaserEventArgs argsIn = new LaserEventArgs();
            argsIn.distance = hit.distance;
            argsIn.target = hit.transform;
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

    public void ActivateFakeLaserRPC(bool[] status)
    {
        photonView.RPC("ActivateFakeLaser", PhotonTargets.OthersBuffered, status);
    }

    //Will only be sent to other clients (except when laserpointer is initialized)
    //0: active, 1: isInEditingMode
    [PunRPC]
    private void ActivateFakeLaser(bool[] status, PhotonMessageInfo info)
    {
        if (info.sender == photonView.owner)
        {
            ActivateCube(status[0]);
            if (status[0])
                isInEditingMode = status[1];
        }

    }

}
