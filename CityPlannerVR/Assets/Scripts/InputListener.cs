﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Valve.VR;
using Valve.VR.InteractionSystem;
using System;
using Photon;

/// <summary>
/// Subscribe for Hand#DeviceFound to get correct device index for e.g. when wands are powered on after beacons.
/// Clicked event subscriptions to and from this script are used for transferring input from multiple
/// sources to needed scripts. Events use primarily the same names as SteamVR_TrackedController.
/// Correct device index checking are done in the destination scripts.
/// </summary>

public class InputListener : PunBehaviour {

    //public GameObject hand1HoldObject; //not yet implemented
    //public GameObject hand2HoldObject;
    private GameObject hand1;
    private GameObject hand2;
    [SerializeField]
    public uint hand1IndexViaRole;
    [SerializeField]
    public uint hand2IndexViaRole;
    [SerializeField]
    public uint hand1IndexViaPos;
    [SerializeField]
    public uint hand2IndexViaPos;
    [SerializeField]
    private ETrackedControllerRole hand1Role;
    [SerializeField]
    private ETrackedControllerRole hand2Role;

    [SerializeField]
	private SteamVR_TrackedController hand1TrackedController;
	[SerializeField]
	private SteamVR_TrackedController hand2TrackedController;

    public SteamVR_TrackedObject hand1TrackedObject;
    public SteamVR_TrackedObject hand2TrackedObject;

    //Put here the events broadcasted by this script
    public event ClickedEventHandler TriggerClicked;
    public event ClickedEventHandler TriggerLifted;
    public event ClickedEventHandler MenuButtonClicked;
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

    private void Start()
    {
        InitializeHands();
        SubscriptionOn();
    }

    private void OnDestroy()
    {
        SubscriptionOff();
    }

    private void InitializeHands()
    {
        hand1 = GameObject.Find("Player/SteamVRObjects/Hand1");
        hand2 = GameObject.Find("Player/SteamVRObjects/Hand2");
        hand1TrackedController = hand1.GetComponent<SteamVR_TrackedController>();
        hand2TrackedController = hand2.GetComponent<SteamVR_TrackedController>();
        hand1TrackedObject = hand1.GetComponent<SteamVR_TrackedObject>();
        hand2TrackedObject = hand2.GetComponent<SteamVR_TrackedObject>();

        StartCoroutine("GetCorrectHandIndices");
    }

    //Checks every other second if both controllers have indices
    IEnumerator GetCorrectHandIndices()
    {
        bool foundHand1 = false;
        bool foundHand2 = false;
        var system = OpenVR.System;

        while (!foundHand1 || !foundHand2)
        {
            if (system != null)
            {
                if (!foundHand1)
                    hand1IndexViaRole = system.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.LeftHand);
                if (!foundHand2)
                    hand2IndexViaRole = system.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.RightHand);
            }
            else
            {
                Debug.Log("OpenVR.System not found!");
            }
            
            if ((int)hand1IndexViaRole != -1) //-1 is the same as none
            {
                if (!foundHand1)
                {
                    SetIndexForComponents((int)hand1IndexViaRole, hand1TrackedController, hand1TrackedObject);
                    foundHand1 = true;
                }
                //"Hand1Found != null" checks that there are subscribers for the event
                if (foundHand1 && Hand1DeviceFound != null)
                {
                    //Debug.Log("Broadcasting Hand1Found");
                    Hand1DeviceFound(hand1IndexViaRole);
                }

            }

            if ((int)hand2IndexViaRole != -1)
            {
                if (!foundHand2)
                {
                    SetIndexForComponents((int)hand2IndexViaRole, hand2TrackedController, hand2TrackedObject);
                    foundHand2 = true;
                }

                if (foundHand2 && Hand2DeviceFound != null)
                {
                    //Debug.Log("Broadcasting Hand2Found");
                    Hand2DeviceFound(hand2IndexViaRole);
                }
            }
            yield return new WaitForSeconds(2);
        }
        Debug.Log("Index for left/hand1: " + (int)hand1IndexViaRole + " and for right/hand2: " + (int)hand2IndexViaRole);
        Invoke("FinalBroadcast", 1); //some scripts might have not had enough time to subscribe

        //The script below is used for debugging purposes and checking what method is the most robust
        hand1Role = system.GetControllerRoleForTrackedDeviceIndex(hand1IndexViaRole);
        hand2Role = system.GetControllerRoleForTrackedDeviceIndex(hand2IndexViaRole);
        Debug.Log("Hand 1: " + hand1Role + " and hand 2: " + hand2Role);

        hand1IndexViaPos = (uint)SteamVR_Controller.GetDeviceIndex(
            SteamVR_Controller.DeviceRelation.Leftmost);
        hand2IndexViaPos = (uint)SteamVR_Controller.GetDeviceIndex(
            SteamVR_Controller.DeviceRelation.Rightmost);
        Debug.Log("Leftmost index (hand1): " + hand1IndexViaPos + " and rightmost index ( hand2): " + hand2IndexViaPos);

    }

    private void FinalBroadcast()
    {
        if (Hand1DeviceFound != null)
            Hand1DeviceFound(hand1IndexViaRole);
        if (Hand2DeviceFound != null)
            Hand2DeviceFound(hand2IndexViaRole);
    }

    private void SetIndexForComponents(int index, SteamVR_TrackedController trackedController, SteamVR_TrackedObject trackedObject)
    {
        if (trackedController && trackedObject)
        {
            trackedController.SetDeviceIndex(index);
            trackedObject.SetDeviceIndex(index);
        }
        else
            Debug.Log("Controller or object not found!");
    }


    private void SubscriptionOn()
    {
        hand1TrackedController.MenuButtonClicked += HandleMenuClicked;
        hand1TrackedController.TriggerClicked += HandleTriggerClicked;
        hand1TrackedController.TriggerUnclicked += HandleTriggerUnClicked;
        hand1TrackedController.Gripped += HandleGripped;
        hand1TrackedController.Ungripped += HandleUngripped;

        hand1TrackedController.PadClicked += HandlePadClicked;
        hand1TrackedController.PadUnclicked += HandlePadUnclicked;
        hand1TrackedController.PadTouched += HandlePadTouched;
        hand1TrackedController.PadUntouched += HandlePadUntouched;

        hand2TrackedController.MenuButtonClicked += HandleMenuClicked;
        hand2TrackedController.TriggerClicked += HandleTriggerClicked;
        hand2TrackedController.TriggerUnclicked += HandleTriggerUnClicked;
        hand2TrackedController.Gripped += HandleGripped;
        hand2TrackedController.Ungripped += HandleUngripped;

        hand2TrackedController.PadClicked += HandlePadClicked;
        hand2TrackedController.PadUnclicked += HandlePadUnclicked;
        hand2TrackedController.PadTouched += HandlePadTouched;
        hand2TrackedController.PadUntouched += HandlePadUntouched;

    }

    private void SubscriptionOff()
    {
        hand1TrackedController.MenuButtonClicked -= HandleMenuClicked;
        hand1TrackedController.TriggerClicked -= HandleTriggerClicked;
        hand1TrackedController.TriggerUnclicked -= HandleTriggerUnClicked;
        hand1TrackedController.Gripped -= HandleGripped;
        hand1TrackedController.Ungripped -= HandleUngripped;

        hand1TrackedController.PadClicked -= HandlePadClicked;
        hand1TrackedController.PadUnclicked -= HandlePadUnclicked;
        hand1TrackedController.PadTouched -= HandlePadTouched;
        hand1TrackedController.PadUntouched -= HandlePadUntouched;

        hand2TrackedController.MenuButtonClicked -= HandleMenuClicked;
        hand2TrackedController.TriggerClicked -= HandleTriggerClicked;
        hand2TrackedController.TriggerUnclicked -= HandleTriggerUnClicked;
        hand2TrackedController.Gripped -= HandleGripped;
        hand2TrackedController.Ungripped -= HandleUngripped;

        hand2TrackedController.PadClicked -= HandlePadClicked;
        hand2TrackedController.PadUnclicked -= HandlePadUnclicked;
        hand2TrackedController.PadTouched -= HandlePadTouched;
        hand2TrackedController.PadUntouched -= HandlePadUntouched;
    }

    private void HandleMenuClicked(object sender, ClickedEventArgs e)
    {
        if (MenuButtonClicked != null)
            MenuButtonClicked(sender, e);
    }

    private void HandleTriggerClicked(object sender, ClickedEventArgs e)
	{
        if (TriggerClicked != null)
            TriggerClicked(sender, e);
    }

    private void HandleTriggerUnClicked(object sender, ClickedEventArgs e)
    {
        if (TriggerLifted != null)
            TriggerLifted(sender, e);
    }

    private void HandleGripped(object sender, ClickedEventArgs e)
    {
        if (Gripped != null)
            Gripped(sender, e);
    }

    private void HandleUngripped(object sender, ClickedEventArgs e)
    {
        if (Ungripped != null)
            Ungripped(sender, e);
    }

    private void HandlePadClicked(object sender, ClickedEventArgs e)
    {
        if (PadClicked != null)
            PadClicked(sender, e);
    }

    private void HandlePadUnclicked(object sender, ClickedEventArgs e)
    {
        if (PadUnclicked != null)
            PadUnclicked(sender, e);
    }

    private void HandlePadTouched(object sender, ClickedEventArgs e)
    {
        if (PadTouched != null)
            PadTouched(sender, e);
    }

    private void HandlePadUntouched(object sender, ClickedEventArgs e)
    {
        if (PadUntouched != null)
            PadUntouched(sender, e);
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

}

