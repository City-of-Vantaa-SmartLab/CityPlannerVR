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
/// Part of an additional layer between networking and eventsystem based on various inputs
/// </summary>

public class InputListener : PunBehaviour {

    public GameObject hand1HoldObject; //not yet implemented
    public GameObject hand2HoldObject;
    private GameObject hand1;
    private GameObject hand2;
    public uint hand1Index;
    public uint hand2Index;
    private uint leftHandIndex;
    private uint rightHandIndex;

    public bool lasersAreActive;

    [SerializeField]
	private SteamVR_TrackedController hand1TrackedController;
	[SerializeField]
	private SteamVR_TrackedController hand2TrackedController;
    [SerializeField]
    private SelectionList selectionList;

    public SteamVR_TrackedObject hand1TrackedObject;
    public SteamVR_TrackedObject hand2TrackedObject;

    [SerializeField]
    private bool leftHandExists = true;
    [SerializeField]
    private bool rightHandExists = true;


    //Put here the events broadcasted by this script
    public event ClickedEventHandler TriggerClicked;
    public event ClickedEventHandler TriggerLifted;
    public event ClickedEventHandler MenuButtonClicked;

    public delegate void EventClearSelections(uint handIndex); //event for highlightselection
    public event EventClearSelections OnClearSelections;

    private void Start()
    {
        hand1 = GameObject.Find("Player/SteamVRObjects/Hand1");
        hand2 = GameObject.Find("Player/SteamVRObjects/Hand2");
        UpdateHands();
        SubscriptionOn();
        lasersAreActive = false;
    }

    private void OnDestroy()
    {
        SubscriptionOff();
    }

    private void GetCorrectHandIndices()
    {
        //hand1Index = (uint)hand1TrackedObject.index; //stays at none for some reason
        //hand2Index = (uint)hand2TrackedObject.index;
        var system = OpenVR.System;
        if (system != null)
        {
            hand1Index = system.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.LeftHand);
            hand2Index = system.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.RightHand);
            Debug.Log("Index for left/hand1: " + hand1Index + " and for right/hand2: " + hand2Index);
        }

        //hand1Index = hand1Script.controller.index;
        //hand2Index = hand2Script.controller.index;

    }

    private void UpdateHands()
    {
        hand1TrackedController = hand1.GetComponent<SteamVR_TrackedController>();
        hand2TrackedController = hand2.GetComponent<SteamVR_TrackedController>();
        hand1TrackedObject = hand1.GetComponent<SteamVR_TrackedObject>();
        hand2TrackedObject = hand2.GetComponent<SteamVR_TrackedObject>();

        GetCorrectHandIndices();

        hand1TrackedController.SetDeviceIndex((int)hand1Index);
        hand2TrackedController.SetDeviceIndex((int)hand2Index);
        hand1TrackedObject.SetDeviceIndex((int)hand1Index);
        hand2TrackedObject.SetDeviceIndex((int)hand2Index);


    }



    private void SubscriptionOn()
    {
        hand1TrackedController.MenuButtonClicked += HandleMenuClicked;
        hand1TrackedController.TriggerClicked += HandleTriggerClicked;
        hand1TrackedController.TriggerUnclicked += HandleTriggerUnClicked;
        //OnGripped(e);

        hand2TrackedController.MenuButtonClicked += HandleMenuClicked;
        hand2TrackedController.TriggerClicked += HandleTriggerClicked;
        hand2TrackedController.TriggerUnclicked += HandleTriggerUnClicked;

    }

    private void HandleMenuClicked(object sender, ClickedEventArgs e)
    {
        if(MenuButtonClicked != null)
            MenuButtonClicked(sender, e);
    }

    private void SubscriptionOff()
    {
        hand1TrackedController.MenuButtonClicked -= HandleMenuClicked;
        hand1TrackedController.TriggerClicked -= HandleTriggerClicked;
        hand1TrackedController.TriggerUnclicked -= HandleTriggerUnClicked;

        hand2TrackedController.MenuButtonClicked -= HandleMenuClicked;
        hand2TrackedController.TriggerClicked -= HandleTriggerClicked;
        hand2TrackedController.TriggerUnclicked -= HandleTriggerUnClicked;

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

    public void InvokeLasersAreOff(uint handIndex)
    {
        if (OnClearSelections != null)
            OnClearSelections(handIndex);
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

    private void FindHandIndexes()
    {
        var system = OpenVR.System;
        if (system != null)
        {
            if (!leftHandExists)
            {
                leftHandIndex = system.GetTrackedDeviceIndexForControllerRole
                    (ETrackedControllerRole.LeftHand);

            }

            if (!rightHandExists)
            {
                rightHandIndex = system.GetTrackedDeviceIndexForControllerRole
                    (ETrackedControllerRole.RightHand);
            }

        }
    }

    private bool CompareHandToControllers(uint handNumber)
    {
        if (handNumber == leftHandIndex || handNumber == rightHandIndex)
            return true;
        return false;
    }

}

