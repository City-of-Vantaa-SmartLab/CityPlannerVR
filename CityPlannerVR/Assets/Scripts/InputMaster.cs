﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using UnityEngine.EventSystems;
using Valve.VR.InteractionSystem;
using System;

/// <summary>
/// Replaces inputlistener. CONTROLLER INDEX IN EVENTARGS e IS USED FOR HAND INDEX (1 or 2!)
/// </summary>

public class InputMaster : MonoBehaviour {

    [SerializeField]
    private Hand hand1;
    [SerializeField]
    private Hand hand2;
    [SerializeField]
    private CheckPlayerSize playerSize;
    [SerializeField]
    private UnityEngine.XR.XRNode leftHandNode;
    [SerializeField]
    private UnityEngine.XR.XRNode rightHandNode;

    public bool hand1Found;
    public bool hand2Found;

    //Ints below are for debugging in inspector: 0 means none, 1 and 2 means the active hand
    [SerializeField]
    private int trigger;
    [SerializeField]
    private int grip;
    [SerializeField]
    private int padclicked;
    [SerializeField]
    private int padtouched;
    [SerializeField]
    private int menubutton;
    [SerializeField]
    private float padX;
    [SerializeField]
    private float padY;
    private bool trackCoordinates;

    //Put here the events broadcasted by this script
    public event ClickedEventHandler TriggerClicked;
    public event ClickedEventHandler TriggerUnclicked;
    public event ClickedEventHandler MenuButtonClicked;
    public event ClickedEventHandler MenuButtonUnclicked;
    public event ClickedEventHandler Gripped;
    public event ClickedEventHandler Ungripped;
    public event ClickedEventHandler PadClicked;
    public event ClickedEventHandler PadUnclicked;
    public event ClickedEventHandler PadTouched;
    public event ClickedEventHandler PadUntouched;

    public delegate void EventWithIndex(uint deviceIndex);
    public event EventWithIndex OnClearSelections; //event for highlightselection
    public event EventWithIndex Hand1DeviceFound;
    public event EventWithIndex Hand2DeviceFound;


    void Start () {
        GameObject hand1GO;
        GameObject hand2GO;

        // Get gameobject handling player VR stuff
        hand1GO = GameObject.Find("Player/SteamVRObjects/Hand1");
        hand2GO = GameObject.Find("Player/SteamVRObjects/Hand2");
        hand1 = hand1GO.GetComponent<Hand>();
        hand2 = hand2GO.GetComponent<Hand>();
        if (hand1)
            hand1Found = true;
        if (hand2)
            hand2Found = true;

        leftHandNode = UnityEngine.XR.XRNode.LeftHand;
        rightHandNode = UnityEngine.XR.XRNode.RightHand;

        //playerSize = playerVR.GetComponent<CheckPlayerSize>();

        //StartCoroutine(TrackHeadCoroutine());
        //StartCoroutine(TrackHandNodeCoroutine(UnityEngine.XR.XRNode.LeftHand, hand1GO));  //Left hand
        //StartCoroutine(TrackHandNodeCoroutine(UnityEngine.XR.XRNode.RightHand, hand2GO));  //Right hand

    }

	// Update is called once per frame
	void Update () {
        if (hand1Found)
            GetInput(hand1, 1);
        if (hand2Found)
            GetInput(hand2, 2);
    }

    void GetInput(Hand hand, int handIndex)
    {
        //if (hand.controller.GetHairTriggerDown(SteamVR_Controller.ButtonMask.Trigger)) //might be better, investigate!

        if (hand.controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            Valve.VR.VRControllerState_t controllerState = hand.controller.GetState();
            ClickedEventArgs e;
            e.controllerIndex = (uint)handIndex;
            e.flags = (uint)controllerState.ulButtonPressed;
            e.padX = controllerState.rAxis0.x;
            e.padY = controllerState.rAxis0.y;
            OnTriggerClicked(e);
            //Debug.Log("Hand" + handIndex + " trigger pressed");
            trigger = handIndex;
        }
        
        if (hand.controller.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            Valve.VR.VRControllerState_t controllerState = hand.controller.GetState();
            ClickedEventArgs e;
            e.controllerIndex = (uint)handIndex;
            e.flags = (uint)controllerState.ulButtonPressed;
            e.padX = controllerState.rAxis0.x;
            e.padY = controllerState.rAxis0.y;
            OnTriggerUnclicked(e);
            //Debug.Log("Hand" + handIndex + " trigger released");
            trigger = 0;
        }

        if (hand.controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
        {
            Valve.VR.VRControllerState_t controllerState = hand.controller.GetState();
            ClickedEventArgs e;
            e.controllerIndex = (uint)handIndex;
            e.flags = (uint)controllerState.ulButtonPressed;
            e.padX = controllerState.rAxis0.x;
            e.padY = controllerState.rAxis0.y;
            OnGripped(e);
            //Debug.Log("Hand" + handIndex + " grip pressed");
            grip = handIndex;
        }

        if (hand.controller.GetPressUp(SteamVR_Controller.ButtonMask.Grip))
        {
            Valve.VR.VRControllerState_t controllerState = hand.controller.GetState();
            ClickedEventArgs e;
            e.controllerIndex = (uint)handIndex;
            e.flags = (uint)controllerState.ulButtonPressed;
            e.padX = controllerState.rAxis0.x;
            e.padY = controllerState.rAxis0.y;
            OnUngripped(e);
            //Debug.Log("Hand" + handIndex + " grip released");
            grip = 0;
        }

        if (hand.controller.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
        {
            Valve.VR.VRControllerState_t controllerState = hand.controller.GetState();
            ClickedEventArgs e;
            e.controllerIndex = (uint)handIndex;
            e.flags = (uint)controllerState.ulButtonPressed;
            e.padX = controllerState.rAxis0.x;
            e.padY = controllerState.rAxis0.y;
            OnPadClicked(e);
            //Debug.Log("Hand" + handIndex + " pad clicked");
            padclicked = handIndex;
        }

        if (hand.controller.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
        {
            Valve.VR.VRControllerState_t controllerState = hand.controller.GetState();
            ClickedEventArgs e;
            e.controllerIndex = (uint)handIndex;
            e.flags = (uint)controllerState.ulButtonPressed;
            e.padX = controllerState.rAxis0.x;
            e.padY = controllerState.rAxis0.y;
            OnPadUnclicked(e);
            //Debug.Log("Hand" + handIndex + " pad released");
            padclicked = 0;
        }

        if (hand.controller.GetTouchDown(SteamVR_Controller.ButtonMask.Touchpad))
        {
            Valve.VR.VRControllerState_t controllerState = hand.controller.GetState();
            ClickedEventArgs e;
            e.controllerIndex = (uint)handIndex;
            e.flags = (uint)controllerState.ulButtonPressed;
            e.padX = controllerState.rAxis0.x;
            e.padY = controllerState.rAxis0.y;
            OnPadTouched(e, hand);
            //Debug.Log("Hand" + handIndex + " pad touched with coordinates x: " + e.padX + " and y: " + e.padY);
            padtouched = handIndex;
            //padx = e.padX;
            //pady = e.padY;
        }

        if (hand.controller.GetTouchUp(SteamVR_Controller.ButtonMask.Touchpad))
        {
            Valve.VR.VRControllerState_t controllerState = hand.controller.GetState();
            ClickedEventArgs e;
            e.controllerIndex = (uint)handIndex;
            e.flags = (uint)controllerState.ulButtonPressed;
            e.padX = controllerState.rAxis0.x;
            e.padY = controllerState.rAxis0.y;
            OnPadUntouched(e);
            //Debug.Log("Hand" + handIndex + " pad untouched with coordinates x: " + e.padX + " and y: " + e.padY);
            //coordinates always 0!!!
            padtouched = 0;
        }

        if (hand.controller.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            Valve.VR.VRControllerState_t controllerState = hand.controller.GetState();
            ClickedEventArgs e;
            e.controllerIndex = (uint)handIndex;
            e.flags = (uint)controllerState.ulButtonPressed;
            e.padX = controllerState.rAxis0.x;
            e.padY = controllerState.rAxis0.y;
            OnMenuClicked(e);
            //Debug.Log("Hand" + handIndex + " menubutton pressed");
            menubutton = handIndex;
        }

        if (hand.controller.GetPressUp(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            Valve.VR.VRControllerState_t controllerState = hand.controller.GetState();
            ClickedEventArgs e;
            e.controllerIndex = (uint)handIndex;
            e.flags = (uint)controllerState.ulButtonPressed;
            e.padX = controllerState.rAxis0.x;
            e.padY = controllerState.rAxis0.y;
            OnMenuUnclicked(e);
            //Debug.Log("Hand" + handIndex + " menubutton released");
            menubutton = 0;
        }
    }

    public virtual void OnTriggerClicked(ClickedEventArgs e)
    {
        if (TriggerClicked != null)
            TriggerClicked(this, e);
    }

    public virtual void OnTriggerUnclicked(ClickedEventArgs e)
    {
        if (TriggerUnclicked != null)
            TriggerUnclicked(this, e);
    }

    public virtual void OnMenuClicked(ClickedEventArgs e)
    {
        if (MenuButtonClicked != null)
            MenuButtonClicked(this, e);
    }

    public virtual void OnMenuUnclicked(ClickedEventArgs e)
    {
        if (MenuButtonUnclicked != null)
            MenuButtonUnclicked(this, e);
    }

    public virtual void OnPadClicked(ClickedEventArgs e)
    {
        if (PadClicked != null)
            PadClicked(this, e);
    }

    public virtual void OnPadUnclicked(ClickedEventArgs e)
    {
        if (PadUnclicked != null)
            PadUnclicked(this, e);
    }

    public virtual void OnPadTouched(ClickedEventArgs e, Hand hand)
    {
        trackCoordinates = true;
        StartCoroutine(TrackCoordinates(e, hand));
    }

    public virtual void OnPadUntouched(ClickedEventArgs e)
    {
        trackCoordinates = false;
        if (PadUntouched != null)
            PadUntouched(this, e);
    }

    public virtual void OnGripped(ClickedEventArgs e)
    {
        if (Gripped != null)
            Gripped(this, e);
    }

    public virtual void OnUngripped(ClickedEventArgs e)
    {
        if (Ungripped != null)
            Ungripped(this, e);
    }

    public void LaserIsOff()
    {
        if (OnClearSelections != null)
        {
            LaserPointer temp1, temp2;
            temp1 = hand1.GetComponent<LaserPointer>();
            temp2 = hand2.GetComponent<LaserPointer>();
            if ((temp1 == null || !temp1.active) && (temp2 == null || !temp2.active))
                OnClearSelections(0);
        }
    }

    public void SelectByLaser(LaserPointer laserPointer, GameObject targetedObject)
    {
        if (laserPointer.gameObject.activeSelf && targetedObject != null)
        {
            var highlightScript = targetedObject.GetComponent<HighlightSelection>();
            if (highlightScript != null)
            {
                highlightScript.ToggleSelection(this.gameObject);
            }
        }
    }

    IEnumerator TrackCoordinates(ClickedEventArgs e, Hand hand)
    {
        Vector2 coordinates = hand.controller.GetAxis();
        while (trackCoordinates)
        {
            //Vector2 coordinates = hand.controller.GetAxis();
            coordinates = hand.controller.GetAxis();
            e.padX = coordinates.x;
            e.padY = coordinates.y;
            //Debug.Log("Hand" + handIndex + " pad touched with coordinates x: " + e.padX + " and y: " + e.padY);
            padX = e.padX;
            padY = e.padY;

            if (PadTouched != null)
                PadTouched(this, e);
            //Debug.Log("tracking...");
            yield return new WaitForSeconds(.1f);
        }
    }


    ////this could be added to scripts that need them, or centralized here if there are too many
    //IEnumerator TrackHandNodeCoroutine(UnityEngine.XR.XRNode node, GameObject hand)
    //{
    //    while (true)
    //    {
    //        hand.transform.rotation = UnityEngine.XR.InputTracking.GetLocalRotation(node);

    //        if (playerSize.isSmall)
    //        {
    //            //all the axes are same for scale, so no matter which one is used. (If they're not, something is wrong and it should be fixed)
    //            hand.transform.position = playerGO.transform.position + UnityEngine.XR.InputTracking.GetLocalPosition(node) * playerGO.transform.localScale.x;
    //            //Now player won't be able to pick up building or other stuff we don't want when they are shrinked down on the table
    //            //hand1.hoverLayerMask = finalMask;
    //            //hand2.hoverLayerMask = finalMask;
    //        }

    //        //Check if we are in god mode (big)
    //        else
    //        {
    //            hand.transform.position = playerGO.transform.position + UnityEngine.XR.InputTracking.GetLocalPosition(node);
    //            //Player is normal sized again and must be able to move everything again
    //            //hand1.hoverLayerMask = -1;
    //            //hand2.hoverLayerMask = -1;
    //        }

    //        //CmdScaleHands(playerVR.transform.localScale * 0.07f);

    //        yield return null;
    //    }
    //}

}
