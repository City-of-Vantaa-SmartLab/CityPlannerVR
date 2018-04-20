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
    private GameObject myHandGO;
    [SerializeField]
    private InputMaster inputMaster;
    [SerializeField]
    private ToolManager toolManager;
    [SerializeField]
    private ToolManager.ToolType myTool;

    public GameObject myTargetedObject;
    [SerializeField]
    private PhotonLaserManager otherLaserManager;
    [SerializeField]
    private LaserPointer myPointer;

    private void Start()
    {
        if (!InitOwn())
            Debug.Log("Failed to initialize PhotonLaserManager on hand" + myHandNumber);

        SubscriptionOn();
        Invoke("InitOther", 0.5f);
        if (myTool != ToolManager.ToolType.Laser)
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
                myTool = toolManager.currentTool;
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

        toolManager.OnToolChange += HandleToolChange;
        myPointer.PointerIn += HandlePointerIn;
        myPointer.PointerOut += HandlePointerOut;
    }

    private void SubscriptionOff()
    {
        inputMaster.TriggerClicked -= HandleTriggerClicked;

        toolManager.OnToolChange -= HandleToolChange;
        myPointer.PointerIn -= HandlePointerIn;
        myPointer.PointerOut -= HandlePointerOut;
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

    private void HandleToolChange(uint handNumber, ToolManager.ToolType tool)
    {
        myHandNumber = (int)handNumber;
        myTool = tool;
        if (tool == ToolManager.ToolType.Laser)
            ToggleLaser(handNumber, true);
        else
            ToggleLaser(handNumber, false);
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
    }

    public void DeactivateObject()
    {
        ActivateObject(false);
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
