using System.Collections;
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
    public uint hand1Index;
    public uint hand2Index;

    [SerializeField]
	private SteamVR_TrackedController hand1TrackedController;
	[SerializeField]
	private SteamVR_TrackedController hand2TrackedController;
    [SerializeField]
    private SelectionList selectionList;

    public SteamVR_TrackedObject hand1TrackedObject;
    public SteamVR_TrackedObject hand2TrackedObject;

    //Put here the events broadcasted by this script
    public event ClickedEventHandler TriggerClicked;
    public event ClickedEventHandler TriggerLifted;
    public event ClickedEventHandler MenuButtonClicked;
    public event ClickedEventHandler Gripped;
    public event ClickedEventHandler Ungripped;

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
                    hand1Index = system.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.LeftHand);
                if (!foundHand2)
                    hand2Index = system.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.RightHand);
                Debug.Log("Index for left/hand1: " + (int)hand1Index + " and for right/hand2: " + (int)hand2Index);
            }
            else
            {
                Debug.Log("OpenVR.System not found!");
            }
            
            if ((int)hand1Index != -1) //-1 is the same as none
            {
                if (!foundHand1)
                {
                    SetIndexForComponents((int)hand1Index, hand1TrackedController, hand1TrackedObject);
                    foundHand1 = true;
                }
                //"Hand1Found != null" checks that there are subscribers for the event
                if (foundHand1 && Hand1DeviceFound != null)
                {
                    //Debug.Log("Broadcasting Hand1Found");
                    Hand1DeviceFound(hand1Index);
                }

            }

            if ((int)hand2Index != -1)
            {
                if (!foundHand2)
                {
                    SetIndexForComponents((int)hand2Index, hand2TrackedController, hand2TrackedObject);
                    foundHand2 = true;
                }

                if (foundHand2 && Hand2DeviceFound != null)
                {
                    //Debug.Log("Broadcasting Hand2Found");
                    Hand2DeviceFound(hand2Index);
                }
            }
            yield return new WaitForSeconds(2);
        }
        //Debug.Log("Both hand indexes found!");
        Invoke("FinalBroadcast", 1); //some scripts might have not had enough time to subscribe
    }

    private void FinalBroadcast()
    {
        if (Hand1DeviceFound != null)
            Hand1DeviceFound(hand1Index);
        if (Hand2DeviceFound != null)
            Hand2DeviceFound(hand2Index);
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
        //OnGripped(e);

        hand2TrackedController.MenuButtonClicked += HandleMenuClicked;
        hand2TrackedController.TriggerClicked += HandleTriggerClicked;
        hand2TrackedController.TriggerUnclicked += HandleTriggerUnClicked;
        hand2TrackedController.Gripped += HandleGripped;
        hand2TrackedController.Ungripped += HandleUngripped;

    }

    private void SubscriptionOff()
    {
        hand1TrackedController.MenuButtonClicked -= HandleMenuClicked;
        hand1TrackedController.TriggerClicked -= HandleTriggerClicked;
        hand1TrackedController.TriggerUnclicked -= HandleTriggerUnClicked;
        hand1TrackedController.Gripped -= HandleGripped;
        hand1TrackedController.Ungripped -= HandleUngripped;

        hand2TrackedController.MenuButtonClicked -= HandleMenuClicked;
        hand2TrackedController.TriggerClicked -= HandleTriggerClicked;
        hand2TrackedController.TriggerUnclicked -= HandleTriggerUnClicked;
        hand2TrackedController.Gripped -= HandleGripped;
        hand2TrackedController.Ungripped -= HandleUngripped;
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

    public void LaserIsOff()
    {
        if (OnClearSelections != null)
        {
            SteamVR_LaserPointer temp1, temp2;
            temp1 = hand1.GetComponent<SteamVR_LaserPointer>();
            temp2 = hand2.GetComponent<SteamVR_LaserPointer>();
            if ((temp1 == null || !temp1.active) && (temp2 == null || !temp2.active))
                OnClearSelections(0);
        }
    }

    public void SelectByLaser(SteamVR_LaserPointer laserPointer, GameObject targetedObject)
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

