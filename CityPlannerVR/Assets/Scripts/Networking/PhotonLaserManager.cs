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
    public LaserPointer myFakeLaser;
    public GameObject myTargetedObject;

    private GameObject myHandGO;
    private InputMaster inputMaster;
    private ToolManager toolManager;
    [SerializeField]
    private ToolManager.ToolType myTool;
    [SerializeField]
    private PhotonLaserManager otherLaserManager;
    [SerializeField]
    private LaserPointer myPointer;
    [SerializeField]
    private bool fakeStatus = false;

    private void Awake()
    {
        if (!InitOwn())
            Debug.LogError("Failed to initialize PhotonLaserManager on hand" + myHandNumber);
    }

    private void Start()
    {
        SubscriptionOn();
        InitOther();
        //Invoke("InitOther", 0.5f);
        if (myTool != ToolManager.ToolType.EditingLaser)
            //photonView.RPC("ActivateObject", PhotonTargets.All, false);
            Invoke("DeactivateObject", 0.5f);
    }

    private void OnDestroy()
    {
        SubscriptionOff();
    }

    private bool InitOwn()
    {
        inputMaster = GameObject.Find("Player").GetComponent<InputMaster>();
        myPointer = gameObject.GetComponent<LaserPointer>();
        myHandGO = transform.parent.gameObject;
        if (myHandGO)
        {
            toolManager = myHandGO.GetComponent<ToolManager>();
            if (toolManager)
            {
                myTool = toolManager.Tool;
                myHandNumber = toolManager.myHandNumber;
            }
        }
        else
            Debug.Log(this.name + " could not find hand GameObject!");
        
        if (!myHandGO || !myPointer || !inputMaster || !toolManager)
        {
            return false;
        }
        return true;
    }

    private bool InitOther()
    {
        GameObject otherHand;
        if (myHandGO)
            otherHand = myHandGO.GetComponent<Hand>().otherHand.gameObject;
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

        toolManager.AnnounceToolChanged += HandleToolChange;
        myPointer.PointerIn += HandlePointerIn;
        myPointer.PointerOut += HandlePointerOut;
    }

    private void SubscriptionOff()
    {
        inputMaster.TriggerClicked -= HandleTriggerClicked;
        inputMaster.TriggerUnclicked -= HandleTriggerUnclicked;

        toolManager.AnnounceToolChanged -= HandleToolChange;
        myPointer.PointerIn -= HandlePointerIn;
        myPointer.PointerOut -= HandlePointerOut;
    }

    private void ToggleLaser(uint deviceIndex, bool status)
    {
        if (myPointer)
        {
            //if (myPointer.active == status)
            //    return;
            ActivateObject(status);
            if (myTool == ToolManager.ToolType.EditingLaser)
            {
                myPointer.GetComponentInChildren<MeshRenderer>().material.color = myPointer.editorColor;
            }

            if (myPointer.active == false)
            {
                inputMaster.LaserIsOff();
            }

        }
        else
            Debug.Log("myPointer not set for lasermanager on" + gameObject.name);
        SendLaserStatusToOthers(status);
    }

    private void HandleToolChange(uint handNumber, ToolManager.ToolType tool)
    {
        myHandNumber = (int)handNumber;
        myTool = tool;
        if (tool == ToolManager.ToolType.EditingLaser)
            ToggleLaser(handNumber, true);
        else
            ToggleLaser(handNumber, false);
    }


    private void HandlePointerOut(object sender, LaserEventArgs e)
    {
        if (myPointer.active)
        {
            myTargetedObject = null;
            var highlightScript = e.target.GetComponent<HighlightSelection>();
            if (highlightScript != null)
            {
                    highlightScript.ToggleHighlight(sender, false);
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
        if (myPointer.active)
        {
            myTargetedObject = e.target.gameObject;
            var highlightScript = e.target.GetComponent<HighlightSelection>();
            if (highlightScript != null)
            {
                highlightScript.ToggleHighlight(sender, true);
            }

            var button = e.target.GetComponent<Button>();
            if (button != null)
            {
                button.Select();
                Debug.Log("HandlePointerIn", e.target.gameObject);
            }
        }
    }


    public void ActivateObject(bool active)
    {
        myPointer.active = active;
        myPointer.ActivateCube(active);
        fakeStatus = active;
    }

    private void SendLaserStatusToOthers(bool active)
    {
        if (myFakeLaser)
            myFakeLaser.ActivateFakeLaserRPC(active);
        else
            Debug.Log("No fake laser found for " + transform.parent.name);
    }

    public void DeactivateObject()
    {
        ActivateObject(false);
        SendLaserStatusToOthers(false);
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
