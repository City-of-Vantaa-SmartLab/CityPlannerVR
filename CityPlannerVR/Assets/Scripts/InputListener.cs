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

    public enum ToolState { Empty, Laser, Painter };

    ToolState state = ToolState.Empty;

    public ToolState State
    {
        get
        {
            return state;
        }
        set
        {
            state = value;
        }
    }


    public GameObject hand1HoldObject; //not yet implemented
    public GameObject hand2HoldObject;
    private GameObject hand1;
    private GameObject hand2;
    public uint hand1Index = 3;
    public uint hand2Index = 4;
    private uint leftHandIndex = 0;
    private uint rightHandIndex = 0;
    public bool lasersAreActive;
    
    

    [SerializeField]
	private SteamVR_TrackedController hand1TrackedController;
	[SerializeField]
	private SteamVR_TrackedController hand2TrackedController;

    public SteamVR_TrackedObject hand1TrackedObject;
    public SteamVR_TrackedObject hand2TrackedObject;

    [SerializeField]
    private SteamVR_LaserPointer laserPointer1;
    [SerializeField]
    private SteamVR_LaserPointer laserPointer2;
    [SerializeField]
    private bool leftHandExists = true;
    [SerializeField]
    private bool rightHandExists = true;


    //Put here the events broadcasted by this script
    public event ClickedEventHandler TriggerClicked;
    public event ClickedEventHandler TriggerLifted;
    public event ClickedEventHandler MenuButtonClicked;
    public event ClickedEventHandler LasersAreOff;   //event for highlightselection
    public event EventHandler ToolChanged;

    private void OnEnable()
    {
        hand1 = GameObject.Find("Player/SteamVRObjects/Hand1");
        hand2 = GameObject.Find("Player/SteamVRObjects/Hand2");
        laserPointer1 = hand1.GetComponentInChildren<SteamVR_LaserPointer>();
        laserPointer2 = hand2.GetComponentInChildren<SteamVR_LaserPointer>();
        UpdateHands();
        SubscriptionOn();
        lasersAreActive = false;

    }

    private void OnDisable()
    {
        SubscriptionOff();
    }

    
    private void UpdateHands()
    {
        hand1TrackedController = hand1.GetComponent<SteamVR_TrackedController>();
        hand2TrackedController = hand2.GetComponent<SteamVR_TrackedController>();
        hand1TrackedObject = hand1.GetComponent<SteamVR_TrackedObject>();
        hand2TrackedObject = hand2.GetComponent<SteamVR_TrackedObject>();

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
        TriggerClicked(sender, e);

    }

    private void HandleTriggerUnClicked(object sender, ClickedEventArgs e)
    {
        TriggerLifted(sender, e);
    }

    public void InvokeLasersAreOff(object sender, ClickedEventArgs e)
    {
        LasersAreOff(sender, e);
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

