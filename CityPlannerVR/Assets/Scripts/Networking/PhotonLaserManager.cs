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

    public int myHandNumber; //This should be set at inspector to either 1 or 2
    [SerializeField]
    private GameObject myHand;
    [SerializeField]
    private InputListener inputListener;
    [SerializeField]
    private ToolManager toolManager;
    [SerializeField]
    private ToolManager.ToolType myTool;
    [SerializeField]
    private GameObject laserCube;

    //public uint hand1Index;
    //public uint hand2Index;
    public uint myHandIndex;
    public GameObject myTargetedObject;
    [SerializeField]
    private PhotonLaserManager otherLaserManager;
    [SerializeField]
    private LaserPointer myPointer;
    private bool initOwnSuccess;
    private bool initOtherSuccess;

    private void Start()
    {
        initOwnSuccess = InitOwn();
        if (!initOwnSuccess)
            Debug.Log("Failed to initialize PhotonLaserManager on hand" + myHandNumber);
        //else
        //    Debug.Log("Initialized PhotonLaserManager");

        SubscriptionOn();
        Invoke("InitOther", 1);
        if (myTool != ToolManager.ToolType.Laser)
            photonView.RPC("ActivateObject", PhotonTargets.All, false);
    }

    private void OnDestroy()
    {
        SubscriptionOff();
    }

    private bool InitOwn()
    {
        if (myHandNumber == 0)
            Debug.Log("Hand number not set for PhotonLaserManager! Set at inspector to either 1 or 2");
        myPointer = gameObject.GetComponent<LaserPointer>();
        inputListener = GameObject.Find("Player").GetComponent<InputListener>();
        laserCube = gameObject.transform.GetChild(0).GetChild(0).gameObject;
        toolManager = gameObject.GetComponentInParent<ToolManager>();
        if (toolManager)
            myTool = toolManager.currentTool;
        myHand = transform.parent.gameObject;

        if (!myPointer || !inputListener || !toolManager || !laserCube || !myHand)
        {
            return false;
        }
        return true;
    }

    private bool InitOther()
    {
        GameObject otherHand;
        if (myHand)
            otherHand = myHand.GetComponent<Hand>().otherHand.gameObject;
        else
            return false;

        if (otherHand)
            otherLaserManager = otherHand.GetComponentInChildren<PhotonLaserManager>();
        else
        {
            Debug.Log("Could not find other hand for hand" + myHandNumber);
            return false;
        }
        return true;
    }

    private void SubscriptionOn()
    {
        inputListener.TriggerClicked += HandleTriggerClicked;
        if (myHandNumber == 1)
            inputListener.Hand1DeviceFound += HandleMyIndexFound;
        if (myHandNumber == 2)
            inputListener.Hand2DeviceFound += HandleMyIndexFound;

        toolManager.OnToolChange += HandleToolChange;
        myPointer.PointerIn += HandlePointerIn;
        myPointer.PointerOut += HandlePointerOut;
    }

    private void SubscriptionOff()
    {
        inputListener.TriggerClicked -= HandleTriggerClicked;
        if (myHandNumber == 1)
            inputListener.Hand1DeviceFound -= HandleMyIndexFound;
        if (myHandNumber == 2)
            inputListener.Hand2DeviceFound -= HandleMyIndexFound;

        toolManager.OnToolChange -= HandleToolChange;
        myPointer.PointerIn -= HandlePointerIn;
        myPointer.PointerOut -= HandlePointerOut;
    }

    private void HandleMyIndexFound(uint deviceIndex)
    {
        myHandIndex = deviceIndex;
        //if (myHandNumber == 1)
        //    inputListener.Hand1DeviceFound -= HandleMyIndexFound;
        //if (myHandNumber == 2)
        //    inputListener.Hand2DeviceFound -= HandleMyIndexFound;
    }

    private void ToggleLaser(uint deviceIndex, bool turnOn)
    {
        //Debug.Log("Toggling laser on: " + turnOn);
        if (turnOn != myPointer.active)  //checks if it is already on/off
        {
            photonView.RPC("ActivateObject", PhotonTargets.All, turnOn);
            if (myPointer.active == false)
            {
                inputListener.LaserIsOff();
            }
        }
    }

    private void HandleToolChange(uint deviceIndex, ToolManager.ToolType tool)
    {
        if (deviceIndex == myHandIndex)
        {
            myTool = tool;
            if (tool == ToolManager.ToolType.Laser)
                ToggleLaser(deviceIndex, true);
            else
                ToggleLaser(deviceIndex, false);
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
        if (e.controllerIndex == myHandIndex && myPointer.active)
        {
            inputListener.SelectByLaser(myPointer, myTargetedObject);
        }

    }

}
