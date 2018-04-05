using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Valve.VR;
using Valve.VR.InteractionSystem;
using System;
using Photon;


public class PhotonLaserManager : PunBehaviour {

    [SerializeField]
    private GameObject myHand;
    [SerializeField]
    private GameObject otherHand;
    [SerializeField]
    private InputListener inputListener;
    [SerializeField]
    private ToolManager toolManager;
    private ToolManager.ToolType myTool;
    [SerializeField]
    private GameObject laserCube;

    public uint hand1Index;
    public uint hand2Index;
    public uint myHandIndex;
    public GameObject myTargetedObject;
    [SerializeField]
    private PhotonLaserManager otherLaserManager;
    [SerializeField]
    private SteamVR_LaserPointer myPointer;
    [SerializeField]
    private SteamVR_LaserPointer otherPointer;

    private void Awake()
    {

    }

    private void Start()
    {
        myPointer = gameObject.GetComponent<SteamVR_LaserPointer>();
        inputListener = GameObject.Find("Player").GetComponent<InputListener>();
        hand1Index = inputListener.hand1Index;
        hand2Index = inputListener.hand2Index;
        WhichHandIsMine();

        toolManager = gameObject.GetComponentInParent<ToolManager>();

        myTool = toolManager.currentTool;
        SubscriptionOn();
        Invoke("FindOther", 1);
    }

    private void OnDestroy()
    {
        SubscriptionOff();
    }

    private void FindOther()
    {
        if (otherHand)
        {
            otherPointer = otherHand.GetComponentInChildren<SteamVR_LaserPointer>();
            otherLaserManager = otherPointer.gameObject.GetComponent<PhotonLaserManager>();
        }
        else
        {
            Debug.Log("Could not find other hand! Trying again in 10s");
            Invoke("WhichHandIsMine", 10f);
            Invoke("FindOther", 10.5f);
        }
        laserCube = gameObject.transform.GetChild(0).GetChild(0).gameObject;
        photonView.RPC("ActivateObject", PhotonTargets.All, false);

    }

    private void SubscriptionOn()
    {
        inputListener.TriggerClicked += HandleTriggerClicked;
        toolManager.ToolChangeWithCE += HandleToolChangeWithCE;
        toolManager.OnToolChange += HandleToolChange;
        myPointer.PointerIn += HandlePointerIn;
        myPointer.PointerOut += HandlePointerOut;

    }

    private void SubscriptionOff()
    {
        inputListener.TriggerClicked -= HandleTriggerClicked;
        toolManager.ToolChangeWithCE -= HandleToolChangeWithCE;
        myPointer.PointerIn -= HandlePointerIn;
        myPointer.PointerOut -= HandlePointerOut;
    }

    private void WhichHandIsMine()
    {
        myHand = transform.parent.gameObject;
        otherHand = myHand.GetComponent<Hand>().otherHand.gameObject;
        //myHandIndex = gameObject.GetComponentInParent<SteamVR_TrackedController>().controllerIndex;
        //if (myHandIndex == hand1Index)
        //{
        //    myHand = GameObject.Find("Player/SteamVRObjects/Hand1");
        //    otherHand = GameObject.Find("Player/SteamVRObjects/Hand2");
        //}
        //else if (myHandIndex == hand2Index)
        //{
        //    otherHand = GameObject.Find("Player/SteamVRObjects/Hand1");
        //    myHand = GameObject.Find("Player/SteamVRObjects/Hand2");
        //}
        //else
        //    Debug.Log("Could not determine hand for " + gameObject.name + " with index: " + myHandIndex);
        //if (!otherHand)
        //    Debug.Log("Could not determine other hand!");

    }

    private void ToggleLaser(uint handIndex)
    {
        if (myPointer.active == true)
        {
            photonView.RPC("ActivateObject", PhotonTargets.All, false);
            if (otherPointer == null || otherPointer.active == false)
            {
                inputListener.lasersAreActive = false;
                inputListener.InvokeLasersAreOff(handIndex);
            }
        }
        else
        {
            photonView.RPC("ActivateObject", PhotonTargets.All, true);
            inputListener.lasersAreActive = true;

        }
    }

    private void HandleToolChange(uint handIndex)
    {
        if (handIndex == myHandIndex)
        {
            if (myTool == ToolManager.ToolType.Laser)
                ToggleLaser(handIndex);
            else if (toolManager.currentTool == ToolManager.ToolType.Laser)
                ToggleLaser(handIndex);
            myTool = toolManager.currentTool;
        }

    }

    private void HandleToolChangeWithCE(object sender, ClickedEventArgs e)
    {
        if (e.controllerIndex == myHandIndex)
        {
            if (myTool == ToolManager.ToolType.Laser)
                ToggleLaser(myHandIndex);
            else if (toolManager.currentTool == ToolManager.ToolType.Laser)
                ToggleLaser(myHandIndex);
            myTool = toolManager.currentTool;
        }
        
    }

    private void HandlePointerOut(object sender, PointerEventArgs e)
    {
        if (myPointer.active && e.controllerIndex == myHandIndex)
        {
            myTargetedObject = null;
            var highlightScript = e.target.GetComponent<HighlightSelection>();
            if (highlightScript != null)
            {
                if (highlightScript.isHighlighted)
                {
                    if (!otherLaserManager || otherLaserManager.myTargetedObject != myTargetedObject)
                        highlightScript.ToggleHighlight();
                }
            }

            var button = e.target.GetComponent<Button>();
            if (button != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                Debug.Log("HandlePointerOut: ", e.target.gameObject);
            }
        }

    }


    private void HandlePointerIn(object sender, PointerEventArgs e)
    {
        if (myPointer.active && e.controllerIndex == myHandIndex)
        {
            myTargetedObject = e.target.gameObject;

            var highlightScript = e.target.GetComponent<HighlightSelection>();

            if (highlightScript != null)
            {
                if (!highlightScript.isHighlighted)
                    highlightScript.ToggleHighlight();
            }

            var button = e.target.GetComponent<Button>();
            if (button != null)
            {
                button.Select();
                Debug.Log("HandlePointerIn", e.target.gameObject);
            }
        }
        
    }

    [PunRPC]
    public void ActivateObject(Boolean active)
    {
        myPointer.active = active;
        laserCube.SetActive(active);
        //gameObject.SetActive(active);
    }

    private void HandleTriggerClicked(object sender, ClickedEventArgs e)
    {
        if (e.controllerIndex == myHandIndex && inputListener)
        {
            inputListener.SelectByLaser(myPointer, myTargetedObject);
        }

    }

}
