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

	//List of players would be better elsewhere!
	public List<GameObject> players;
	public GameObject localPlayer;
    public GameObject leftTargetedObject;
    public GameObject rightTargetedObject;


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

    //private SteamVR_Controller.Device leftDevice;
    //private SteamVR_Controller.Device rightDevice;

    public bool swapHands = false;


    [SerializeField]
    private SteamVR_LaserPointer leftLaserPointer;
    [SerializeField]
    private SteamVR_LaserPointer rightLaserPointer;

    //Put here the events broadcasted by this script (for multiplayer)
    public event ClickedEventHandler TriggerClicked;



	private void OnEnable() {
        UpdatePlayerList();

        hand1 = GameObject.Find("Player/SteamVRObjects/Hand1");
        hand2 = GameObject.Find("Player/SteamVRObjects/Hand2");

        //Swap these if eg. left trigger shows up as right trigger input!
        leftHand = hand2;
        rightHand = hand1;

        UpdateHands();
        InvokeRepeating("UpdateHands", 5, 10.0f);

        leftLaserPointer = leftHand.GetComponentInChildren<SteamVR_LaserPointer>();
        rightLaserPointer = rightHand.GetComponentInChildren<SteamVR_LaserPointer>();

        SubscriptionOn();
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

        //alternative methods
        //leftDevice = SteamVR_Controller.Input(SteamVR_Controller.GetDeviceIndex(
        //    SteamVR_Controller.DeviceRelation.Leftmost));
        //rightDevice = SteamVR_Controller.Input(SteamVR_Controller.GetDeviceIndex(
        //    SteamVR_Controller.DeviceRelation.Rightmost));

        //leftHandIndex = (uint)SteamVR_Controller.GetDeviceIndex(
        //    SteamVR_Controller.DeviceRelation.Leftmost);
        //rightHandIndex = (uint)SteamVR_Controller.GetDeviceIndex(
        //    SteamVR_Controller.DeviceRelation.Rightmost);

        CheckIndex();

        //There is a bug in the if clause!!!
        //if (hand1.GetComponent<Hand>().controller.index != leftHandIndex)
        //{
        //    //leftHand = hand1;
        //    rightHand = hand1;

        //}

        //if (hand2.GetComponent<Hand>().controller.index != rightHandIndex)
        //{
        //    leftHand = hand2;
        //    //rightHand = hand2;
        //}


        //if (leftTrackedController == null || rightTrackedController == null)
        //{
            leftTrackedController = leftHand.GetComponent<SteamVR_TrackedController>();
            rightTrackedController = rightHand.GetComponent<SteamVR_TrackedController>();
        //}

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
        leftTrackedController.TriggerClicked += HandleTriggerClicked;
        rightTrackedController.TriggerClicked += HandleTriggerClicked;


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
        leftTrackedController.TriggerClicked -= HandleTriggerClicked;
        rightTrackedController.TriggerClicked -= HandleTriggerClicked;
    }

    private void HandleTriggerClicked(object sender, ClickedEventArgs e)
	{
        if (e.controllerIndex == leftHandIndex)
        {
            //Debug.Log("Left trigger clicked!");
            SelectByLaser(leftLaserPointer, leftTargetedObject); 
        }

        if (e.controllerIndex == rightHandIndex)
        {
            //Debug.Log("Right trigger clicked!");
            SelectByLaser(rightLaserPointer, rightTargetedObject);
        }
    }

    private void SelectByLaser(SteamVR_LaserPointer laserPointer, GameObject targetedObject)
    {
        if (laserPointer.gameObject.activeSelf && targetedObject != null)
        {
            var highlightScript = targetedObject.GetComponent<HighlightSelection>();
            if (highlightScript != null)
            {
                highlightScript.ToggleSelection();
            }
            else
                Debug.Log("Could not find higlightscript!");
        }
    }

    private void HandlePointerIn(object sender, PointerEventArgs e)
    {
        if (e.controllerIndex == leftHandIndex)
            leftTargetedObject = e.target.gameObject;
        if (e.controllerIndex == rightHandIndex)
            rightTargetedObject = e.target.gameObject;

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
        if (e.controllerIndex == leftHandIndex)
            leftTargetedObject = null;
        if (e.controllerIndex == rightHandIndex)
            rightTargetedObject = null;

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

    private void SwapHands()
    {
        if (leftHand == hand2)
        {
            leftHand = hand1;
            rightHand = hand2;
        }
        else
        {
            leftHand = hand2;
            rightHand = hand1;
        }

    }

    private void Update()
    {
        if (swapHands)
        {
            SwapHands();
            swapHands = false;
        }
    }

}
