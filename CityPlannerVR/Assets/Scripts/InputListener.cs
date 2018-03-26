using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Valve.VR;
using Valve.VR.InteractionSystem;
using System;

/// <summary>
/// Part of an additional layer between networking and eventsystem based on various inputs
/// </summary>

public class InputListener : MonoBehaviour {

    public GameObject laser1TargetedObject;
    public GameObject laser2TargetedObject;
    public GameObject hand1HoldObject;
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
    public event ClickedEventHandler TriggerClicked; //
    public event ClickedEventHandler TriggerLifted;
    public event ClickedEventHandler LasersAreOff;   //event for highlightselection
    public event EventHandler Hand1Found;
    public event EventHandler Hand2Found;



    private void OnEnable()
    {
        hand1 = GameObject.Find("Player/SteamVRObjects/Hand1");
        hand2 = GameObject.Find("Player/SteamVRObjects/Hand2");
        laserPointer1 = hand1.GetComponentInChildren<SteamVR_LaserPointer>();
        laserPointer2 = hand2.GetComponentInChildren<SteamVR_LaserPointer>();
        UpdateHands();
        SubscriptionOn();
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
        laserPointer1.PointerIn += HandlePointerIn;
        laserPointer1.PointerOut += HandlePointerOut;
        laserPointer2.PointerIn += HandlePointerIn;
        laserPointer2.PointerOut += HandlePointerOut;

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
        ToggleLaser(sender, e);

    }

    private void SubscriptionOff()
    {
        laserPointer1.PointerIn -= HandlePointerIn;
        laserPointer1.PointerOut -= HandlePointerOut;
        laserPointer2.PointerIn -= HandlePointerIn;
        laserPointer2.PointerOut -= HandlePointerOut;

        hand1TrackedController.MenuButtonClicked -= HandleMenuClicked;
        hand1TrackedController.TriggerClicked -= HandleTriggerClicked;
        hand2TrackedController.TriggerUnclicked -= HandleTriggerUnClicked;


        hand2TrackedController.MenuButtonClicked -= HandleMenuClicked;
        hand2TrackedController.TriggerClicked -= HandleTriggerClicked;
        hand2TrackedController.TriggerUnclicked -= HandleTriggerUnClicked;

    }

    private void HandleTriggerClicked(object sender, ClickedEventArgs e)
	{

        if (e.controllerIndex == hand1Index)
        {
            SelectByLaser(laserPointer1, laser1TargetedObject);
            TriggerClicked(sender, e);
        }

        if (e.controllerIndex == hand2Index)
        {
            SelectByLaser(laserPointer2, laser2TargetedObject);
            TriggerClicked(sender, e);
        }
    }

    private void HandleTriggerUnClicked(object sender, ClickedEventArgs e)
    {

            TriggerLifted(sender, e);

    }


    private void SelectByLaser(SteamVR_LaserPointer laserPointer, GameObject targetedObject)
    {
        if (laserPointer.gameObject.activeSelf && targetedObject != null)
        {
            var highlightScript = targetedObject.GetComponent<HighlightSelection>();
            if (highlightScript != null)
            {
                highlightScript.ToggleSelection(this.gameObject);
            }
            else
            {
                //Debug.Log("Could not find higlightscript!");
            }
        }
    }

    private void HandlePointerIn(object sender, PointerEventArgs e)
    {
        if (e.controllerIndex == hand1Index)
            laser1TargetedObject = e.target.gameObject;
        if (e.controllerIndex == hand2Index)
            laser2TargetedObject = e.target.gameObject;

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

    private void HandlePointerOut(object sender, PointerEventArgs e)
    {
        if (e.controllerIndex == hand1Index)
            laser1TargetedObject = null;
        if (e.controllerIndex == hand2Index)
            laser2TargetedObject = null;

        var highlightScript = e.target.GetComponent<HighlightSelection>();
        if (highlightScript != null)
        {
            if (highlightScript.isHighlighted)
                highlightScript.ToggleHighlight();
        }

        var button = e.target.GetComponent<Button>();
        if (button != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            Debug.Log("HandlePointerOut", e.target.gameObject);
        }
    }

    private void ToggleLaser(object sender, ClickedEventArgs e)
    {
        if (e.controllerIndex == hand1Index)
        {
            if (laserPointer1.gameObject.activeSelf == true)
            {
                laserPointer1.gameObject.SetActive(false);
                if (laserPointer2.gameObject.activeSelf == false)
                {
                    lasersAreActive = false;
                    LasersAreOff(this, e);
                }
            }
            else
            {
                laserPointer1.gameObject.SetActive(true);
                lasersAreActive = true;
            }
        }
        else if (e.controllerIndex == hand2Index)
        {
            if (laserPointer2.gameObject.activeSelf == true)
            {
                laserPointer2.gameObject.SetActive(false);
                if (laserPointer1.gameObject.activeSelf == false)
                {
                    lasersAreActive = false;
                    LasersAreOff(this, e);
                }
            }
            else
            {
                laserPointer2.gameObject.SetActive(true);
                lasersAreActive = true;
            }
        }
        else
        {
            Debug.Log("Laser toggle input not recognized!");
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



    private void Start()
    {
        //ClickedEventArgs tempEvent;
        //tempEvent.controllerIndex = 0;
        //tempEvent.flags = 0;
        //tempEvent.padX = 0;
        //tempEvent.padY = 0;

        if (laserPointer1)
        {
            laserPointer1.gameObject.SetActive(false);
        }
        if (laserPointer2)
        {
            laserPointer2.gameObject.SetActive(false);
        }
        lasersAreActive = false;

    }

}

