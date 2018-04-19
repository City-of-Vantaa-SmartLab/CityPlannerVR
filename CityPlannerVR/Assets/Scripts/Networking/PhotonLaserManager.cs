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

    public int myHandNumber;
    [SerializeField]
    private GameObject myHand;
    [SerializeField]
    private InputMaster inputMaster;
    [SerializeField]
    private ToolManager toolManager;
    [SerializeField]
    private ToolManager.ToolType myTool;
    //[SerializeField]
    //private GameObject laserCube;

    //public uint hand1Index;
    //public uint hand2Index;
    public uint myHandIndex;
    public GameObject myTargetedObject;
    [SerializeField]
    private PhotonLaserManager otherLaserManager;
    [SerializeField]
    private LaserPointer myPointer;
    private bool initOwnSuccess;

    private void Start()
    {
        if (!InitOwn())
            Debug.Log("Failed to initialize PhotonLaserManager on hand" + myHandNumber);
        //else
        //    Debug.Log("Initialized PhotonLaserManager");

        SubscriptionOn();
        Invoke("InitOther", 1);
        if (myTool != ToolManager.ToolType.Laser)
            //photonView.RPC("ActivateObject", PhotonTargets.All, false);
            ActivateObject(false);
    }

    private void OnDestroy()
    {
        SubscriptionOff();
    }

    private bool InitOwn()
    {
        inputMaster = GameObject.Find("Player").GetComponent<InputMaster>();
        myPointer = gameObject.GetComponent<LaserPointer>();
        myHand = transform.parent.gameObject;
        if (myHand)
        {
            if (myHand.name == "Hand1")
                myHandNumber = 1;
            else if (myHand.name == "Hand2")
                myHandNumber = 2;
            if (myHandNumber == 0)
                Debug.Log("Hand number could not be determined for PhotonLaserManager!");
            //laserCube = gameObject.transform.GetChild(0).GetChild(0).gameObject;
            toolManager = myHand.GetComponent<ToolManager>();
            if (toolManager)
                myTool = toolManager.currentTool;
        }
        else
            Debug.Log(this.name + " could not find hand GameObject!");
        
        if (!myHand || !myPointer || !inputMaster || !toolManager /*|| !laserCube*/ )
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
        inputMaster.TriggerClicked += HandleTriggerClicked;
        inputMaster.TriggerUnclicked += HandleTriggerUnclicked;
        if (myHandNumber == 1)
            inputMaster.Hand1DeviceFound += HandleMyIndexFound;
        if (myHandNumber == 2)
            inputMaster.Hand2DeviceFound += HandleMyIndexFound;

        toolManager.OnToolChange += HandleToolChange;
        myPointer.PointerIn += HandlePointerIn;
        myPointer.PointerOut += HandlePointerOut;
    }

    private void SubscriptionOff()
    {
        inputMaster.TriggerClicked -= HandleTriggerClicked;
        if (myHandNumber == 1)
            inputMaster.Hand1DeviceFound -= HandleMyIndexFound;
        if (myHandNumber == 2)
            inputMaster.Hand2DeviceFound -= HandleMyIndexFound;

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

    private void ToggleLaser(uint deviceIndex, bool status)
    {
        if (myPointer)
        {
            //photonView.RPC("ActivateObject", PhotonTargets.All, status);
            ActivateObject(status);
            if (myPointer.active == false)
            {
                inputMaster.LaserIsOff();
            }
        }
        else
            Debug.Log("myPointer not set for lasermanager on" + gameObject.name);
    }

    private void HandleToolChange(uint deviceIndex, ToolManager.ToolType tool)
    {
        if (deviceIndex == myHandNumber)
        {
            myTool = tool;
            if (tool == ToolManager.ToolType.Laser)
                ToggleLaser(deviceIndex, true);
            else
                ToggleLaser(deviceIndex, false);
        }

    }


    private void HandlePointerOut(object sender, LaserEventArgs e)
    {
        if (myPointer.active && e.handNumber == myHandNumber)
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


    private void HandlePointerIn(object sender, LaserEventArgs e)
    {
        if (myPointer.active && e.handNumber == myHandNumber)
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
        myPointer.ActivateCube(active);
        //laserCube.SetActive(active);
        //gameObject.SetActive(active);
    }

    private void HandleTriggerClicked(object sender, ClickedEventArgs e)
    {
        if (e.controllerIndex == myHandNumber && myPointer.active)
        {
            inputMaster.SelectByLaser(myPointer, myTargetedObject);
            myPointer.triggered = true;
        }

    }

    private void HandleTriggerUnclicked(object sender, ClickedEventArgs e)
    {
        if (e.controllerIndex == myHandNumber && myPointer.active)
        {
            myPointer.triggered = false;
        }
    }

}
