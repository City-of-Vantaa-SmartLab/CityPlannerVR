using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using UnityEngine.EventSystems;
using Valve.VR.InteractionSystem;
using System;
using Photon;

/// <summary>
/// Replaces inputlistener. Controller index in eventargs e is used for hand index (1 or 2!).
/// Subscribe for the clickedevents in the scripts that require input.
/// </summary>
/// 

public class InputMaster : PunBehaviour {

    public enum RoleType { Bystander, TEST, Worker, Admin };

    public RoleType Role
    {
        get
        {
            return currentRole;
        }
        set
        {
            currentRole = value;
            AnnounceRoleChanged();
        }
    }

    private Hand hand1;
    private Hand hand2;
    //[SerializeField]
    //private CheckPlayerSize playerSize;
    //[SerializeField]
    //private UnityEngine.XR.XRNode leftHandNode;
    //[SerializeField]
    //private UnityEngine.XR.XRNode rightHandNode;
    [SerializeField]
    private RoleType currentRole;

    public bool hand1Found;
    public bool hand2Found;
    public bool hand1InsideToolbelt;
    public bool hand2InsideToolbelt;

    //Ints below are for debugging in inspector's debug mode: 0 means none, 1 and 2 means the active hand
    private int trigger;
    private int grip;
    private int padclicked;
    private int padtouched;
    private int menubutton;
    private float padX;
    private float padY;
    private bool trackCoordinates;

    //Put here the events broadcasted by this script
    public event ClickedEventHandler TriggerClicked;
    public event ClickedEventHandler TriggerClickedInsideToolbelt;
    public event ClickedEventHandler TriggerUnclicked;
    public event ClickedEventHandler MenuButtonClicked;
    public event ClickedEventHandler MenuButtonUnclicked;
    public event ClickedEventHandler Gripped;
    public event ClickedEventHandler Ungripped;
    public event ClickedEventHandler PadClicked;
    public event ClickedEventHandler PadUnclicked;
    public event ClickedEventHandler PadTouched;
    public event ClickedEventHandler PadUntouched;

    public delegate void EventWithIndex(int deviceIndex);
    public event EventWithIndex ClearSelections; //event for highlightselection
    public event EventWithIndex RoleChanged;
    public event EventWithIndex ClearActiveItemSlots;
    //public event EventWithIndex Hand1DeviceFound;
    //public event EventWithIndex Hand2DeviceFound;

    private void Awake()
    {
        GameObject hand1GO;
        GameObject hand2GO;
        hand1GO = GameObject.Find("Player/SteamVRObjects/Hand1");
        hand2GO = GameObject.Find("Player/SteamVRObjects/Hand2");
        hand1 = hand1GO.GetComponent<Hand>();
        hand2 = hand2GO.GetComponent<Hand>();
        if (hand1)
            hand1Found = true;
        if (hand2)
            hand2Found = true;
    }

    private void Start()
    {
        Role = RoleType.TEST;
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

    #region OnEvents

    private void OnTriggerClicked(ClickedEventArgs e)
    {
        if ((hand1InsideToolbelt && e.controllerIndex == 1) || (hand2InsideToolbelt && e.controllerIndex == 2))
        {
            TriggerClickedInsideToolbelt?.Invoke(this, e);
        }
        else
            TriggerClicked?.Invoke(this, e);  //Is the same as: if (TriggerClicked != null) TriggerClicked(this, e);
    }

    private void OnTriggerUnclicked(ClickedEventArgs e)
    {
        TriggerUnclicked?.Invoke(this, e);  //Checks if the event has any subscribers before firing the event
    }

    private void OnMenuClicked(ClickedEventArgs e)
    {
        MenuButtonClicked?.Invoke(this, e);
    }

    private void OnMenuUnclicked(ClickedEventArgs e)
    {
        MenuButtonUnclicked?.Invoke(this, e);
    }

    private void OnPadClicked(ClickedEventArgs e)
    {
        PadClicked?.Invoke(this, e);
    }

    private void OnPadUnclicked(ClickedEventArgs e)
    {
        PadUnclicked?.Invoke(this, e);
    }

    private void OnPadTouched(ClickedEventArgs e, Hand hand)
    {
        trackCoordinates = true;
        StartCoroutine(TrackCoordinates(e, hand));
    }

    private void OnPadUntouched(ClickedEventArgs e)
    {
        trackCoordinates = false;
        PadUntouched?.Invoke(this, e);
    }

    private void OnGripped(ClickedEventArgs e)
    {
        Gripped?.Invoke(this, e);
    }

    private void OnUngripped(ClickedEventArgs e)
    {
        Ungripped?.Invoke(this, e);
    }

    private void AnnounceRoleChanged()
    {
        RoleChanged?.Invoke(0);
    }

    public void ClearItemSlots()
    {
        ClearActiveItemSlots?.Invoke(0);
    }

    #endregion

    public void LaserIsOff()
    {
        if (ClearSelections != null)
        {
            LaserPointer temp1, temp2;
            temp1 = hand1.GetComponent<LaserPointer>();
            temp2 = hand2.GetComponent<LaserPointer>();
            if ((temp1 == null || !temp1.active) && (temp2 == null || !temp2.active))
                ClearSelections(0);
        }
    }

    public void SelectByLaser(LaserPointer laserPointer, GameObject targetedObject, ClickedEventArgs e)
    {
        if (laserPointer.gameObject.activeSelf && targetedObject != null)
        {
            var highlightScript = targetedObject.GetComponent<HighlightSelection>();
            var laserButton = targetedObject.GetComponent<LaserButton>();
            if (highlightScript != null)
            {
                //highlightScript.ToggleSelection(this.gameObject);
            }
            if (laserButton != null)
            {
                laserButton.OnClicked(e, this);
            }
            //laserPointer.gameObject.GetComponent<OpenCommentTool>().ActivateCommentTool(laserPointer, targetedObject);
            laserPointer.gameObject.transform.parent.GetComponentInChildren<AreaSelection>().ActivateCreatePoint(targetedObject);
            
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

            PadTouched?.Invoke(this, e);
            //Debug.Log("tracking...");
            yield return new WaitForSeconds(.1f);
        }
    }




}
