using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Valve.VR;
using Valve.VR.InteractionSystem;
using System;

/// <summary>
/// Part of an additional layer between networking, device input and eventsystem
/// </summary>

public class InputListener : MonoBehaviour {

	//Might not be needed
	public List<GameObject> players;
	public GameObject localPlayer;
    public GameObject targetedObject;


    public GameObject leftHand;
	public GameObject rightHand;
	public uint leftHandIndex;
	public uint rightHandIndex;
    private GameObject hand1;
    private GameObject hand2;


	[SerializeField]
	private SteamVR_TrackedController leftTrackedController;
	[SerializeField]
	private SteamVR_TrackedController rightTrackedController;

    public SteamVR_Controller.Device leftDevice;
    public SteamVR_Controller.Device rightDevice;

    [SerializeField]
    private SteamVR_LaserPointer leftLaserPointer;
    [SerializeField]
    private SteamVR_LaserPointer rightLaserPointer;

    //Put here the events broadcasted by this script
    public event ClickedEventHandler TriggerClicked;



	private void OnEnable() {
        UpdatePlayerList();

        hand1 = GameObject.Find("Player/SteamVRObjects/Hand1");
        hand2 = GameObject.Find("Player/SteamVRObjects/Hand2");

        UpdateHands();
        InvokeRepeating("UpdateHands", 5, 10.0f);

        leftLaserPointer = leftHand.GetComponentInChildren<SteamVR_LaserPointer>();
        rightLaserPointer = rightHand.GetComponentInChildren<SteamVR_LaserPointer>();

        SubscriptionOn();










        //
        //
        //		leftTrackedController.controllerIndex = SteamVR_Controller.GetDeviceIndex (leftHand);

        //subscribe only to the events called by the local player's tracked object


    }


    private void OnDisable() {

        SubscriptionOff();

    }

    //Subject to change when migrating to Photon
    private void UpdatePlayerList()
    {
        localPlayer = GameObject.FindGameObjectWithTag("Player");
    }

    private void UpdateHands()
    {
        leftHandIndex = OpenVR.System.GetTrackedDeviceIndexForControllerRole
            (ETrackedControllerRole.LeftHand);
        rightHandIndex = OpenVR.System.GetTrackedDeviceIndexForControllerRole
            (ETrackedControllerRole.RightHand);

        //alternative method
        //leftHand = SteamVR_Controller.Input(SteamVR_Controller.GetDeviceIndex(
        //    SteamVR_Controller.DeviceRelation.Leftmost));
        //rightHand = SteamVR_Controller.GetDeviceIndex(
        //    SteamVR_Controller.DeviceRelation.Rightmost);

        //leftHand = localPlayer.GetComponentInChildren<GameObject>();
        //rightHand = localPlayer.GetComponentInChildren<GameObject>();

        CheckIndex();

        if (hand1)
        {
                leftHand = hand1;
                rightHand = hand2;
        }


        if(leftTrackedController == null || rightTrackedController == null)
        {
            leftTrackedController = leftHand.GetComponent<SteamVR_TrackedController>();
            rightTrackedController = rightHand.GetComponent<SteamVR_TrackedController>();
        }

        //leftTrackedController.controllerIndex = leftHandIndex;
        //rightTrackedController.controllerIndex = rightHandIndex;

        leftTrackedController.SetDeviceIndex((int)leftHandIndex);
        rightTrackedController.SetDeviceIndex((int)rightHandIndex);


        //leftDevice = SteamVR_Controller.Input((int)leftHandIndex);
        //rightDevice = SteamVR_Controller.Input((int)rightHandIndex);

    }

    private void CheckIndex()
    {
        if (leftHandIndex == 4294967295)
        {
            if (rightHandIndex == 3)
                leftHandIndex = 4;
            if (rightHandIndex == 4)
                leftHandIndex = 3;


        }

        if (rightHandIndex == 4294967295)
        {
            if (leftHandIndex == 3)
                rightHandIndex = 4;
            if (leftHandIndex == 4)
                rightHandIndex = 3;
        }
    }

    private void SubscriptionOn()
    {
        leftLaserPointer.PointerIn += HandlePointerIn;
        leftLaserPointer.PointerOut += HandlePointerOut;
        rightLaserPointer.PointerIn += HandlePointerIn;
        rightLaserPointer.PointerOut += HandlePointerOut;

        leftTrackedController.MenuButtonClicked += HandleMenuClicked;
        rightTrackedController.MenuButtonClicked += HandleMenuClicked;

    }

    private void HandleMenuClicked(object sender, ClickedEventArgs e)
    {
        //Debug.Log("Controller index: " +  e.controllerIndex);

        if (e.controllerIndex == leftHandIndex)
        {
            
            if (leftLaserPointer.gameObject.activeSelf == true)
                leftLaserPointer.gameObject.SetActive(false);
            else
                leftLaserPointer.gameObject.SetActive(true);

        }
        if (e.controllerIndex == rightHandIndex)
        {
            if (rightLaserPointer.gameObject.activeSelf == true)
                rightLaserPointer.gameObject.SetActive(false);
            else
                rightLaserPointer.gameObject.SetActive(true);
        }

    }

    private void SubscriptionOff()
    {
        leftLaserPointer.PointerIn -= HandlePointerIn;
        leftLaserPointer.PointerOut -= HandlePointerOut;
        rightLaserPointer.PointerIn -= HandlePointerIn;
        rightLaserPointer.PointerOut -= HandlePointerOut;
    }



    private void HandleTriggerClicked(object sender, ClickedEventArgs e)
	{
		//if clause might be redundant, if the left/right is innately handled
		//with correct subscription as well as networking
//		if (e.controllerIndex == SteamVR_Controller.GetDeviceIndex (leftHand)) {
//			//create an event that broadcasts for the local player's left input
//		}
//		else if (e.controllerIndex == SteamVR_Controller.GetDeviceIndex (leftHand)) {
//
//		}

	}

    private void HandlePointerIn(object sender, PointerEventArgs e)
    {
        targetedObject = e.target.gameObject;
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
        targetedObject = null;
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



}
