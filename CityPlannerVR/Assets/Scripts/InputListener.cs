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

    public GameObject leftTargetedObject;
    public GameObject rightTargetedObject;

    public GameObject leftHand;
	public GameObject rightHand;
	public uint leftHandIndex = 0;
	public uint rightHandIndex = 0;
    private GameObject hand1;
    private GameObject hand2;

	[SerializeField]
	private SteamVR_TrackedController leftTrackedController;
	[SerializeField]
	private SteamVR_TrackedController rightTrackedController;
    [SerializeField]
    private SteamVR_TrackedObject leftTrackedObject;
    [SerializeField]
    private SteamVR_TrackedObject rightTrackedObject;



    public bool swapHands = false;
    [SerializeField]
    private bool handIndexesSet = false;

    [SerializeField]
    private SteamVR_LaserPointer laserPointer1;
    [SerializeField]
    private SteamVR_LaserPointer laserPointer2;

    //Put here the events broadcasted by this script (for multiplayer)
    public event ClickedEventHandler TriggerClicked;



	private void OnEnable() {

        hand1 = GameObject.Find("Player/SteamVRObjects/Hand1");
        hand2 = GameObject.Find("Player/SteamVRObjects/Hand2");

        FindHandIndexes();
        handIndexesSet = CheckIndex();
        if (handIndexesSet)
        {
            leftHand = hand1;
            rightHand = hand2;
            UpdateHands();
        }
 

        laserPointer1 = hand1.GetComponentInChildren<SteamVR_LaserPointer>();
        laserPointer2 = hand2.GetComponentInChildren<SteamVR_LaserPointer>();

        SubscriptionOn();
    }

    private void FindHandIndexes()
    {
        var system = OpenVR.System;
        if (system != null)
        {
            leftHandIndex = system.GetTrackedDeviceIndexForControllerRole
                (ETrackedControllerRole.LeftHand);
            rightHandIndex = system.GetTrackedDeviceIndexForControllerRole
                (ETrackedControllerRole.RightHand);
        }
    }

    private void OnDisable() {
        SubscriptionOff();
    }


    private void UpdateHands()
    {

        leftTrackedController = leftHand.GetComponent<SteamVR_TrackedController>();
        rightTrackedController = rightHand.GetComponent<SteamVR_TrackedController>();
        leftTrackedObject = leftHand.GetComponent<SteamVR_TrackedObject>();
        rightTrackedObject = rightHand.GetComponent<SteamVR_TrackedObject>();

        leftTrackedController.SetDeviceIndex((int)leftHandIndex);
        rightTrackedController.SetDeviceIndex((int)rightHandIndex);
        leftTrackedObject.SetDeviceIndex((int)leftHandIndex);
        rightTrackedObject.SetDeviceIndex((int)rightHandIndex);

    }

    private bool CheckIndex()
    {
        bool check = true;
        if (leftHandIndex == 4294967295 || leftHandIndex == 0)
        {
            if (rightHandIndex == 3)
            {
                leftHandIndex = 4;
            }
            else if (rightHandIndex == 4)
            {
                leftHandIndex = 3;
            }
            else
            {
                Debug.Log("Left hand index not set correctly!");
                check = false;
            }
        }

        if (rightHandIndex == 4294967295 || rightHandIndex == 0)
        {
            if (leftHandIndex == 3)
            {
                rightHandIndex = 4;
            }
            else if (leftHandIndex == 4)
            {
                rightHandIndex = 3;
            }
            else
            {
                Debug.Log("Right hand index not set correctly!");
                check = false;
            }
        }
        return check;
    }
        

    private void SubscriptionOn()
    {
        laserPointer1.PointerIn += HandlePointerIn;
        laserPointer1.PointerOut += HandlePointerOut;
        laserPointer2.PointerIn += HandlePointerIn;
        laserPointer2.PointerOut += HandlePointerOut;

        if (handIndexesSet)
        {
            leftTrackedController.MenuButtonClicked += HandleMenuClicked;
            rightTrackedController.MenuButtonClicked += HandleMenuClicked;
            leftTrackedController.TriggerClicked += HandleTriggerClicked;
            rightTrackedController.TriggerClicked += HandleTriggerClicked;
        }



    }

    private void HandleMenuClicked(object sender, ClickedEventArgs e)
    {
        //Debug.Log("Controller index: " +  e.controllerIndex);

        if (e.controllerIndex == leftHandIndex)
        {
            
            if (laserPointer1.gameObject.activeSelf == true)
                laserPointer1.gameObject.SetActive(false);
            else
                laserPointer1.gameObject.SetActive(true);

        }
        else if (e.controllerIndex == rightHandIndex)
        {
            if (laserPointer2.gameObject.activeSelf == true)
                laserPointer2.gameObject.SetActive(false);
            else
                laserPointer2.gameObject.SetActive(true);
        }
        else
        {

        }

    }

    private void SubscriptionOff()
    {
        laserPointer1.PointerIn -= HandlePointerIn;
        laserPointer1.PointerOut -= HandlePointerOut;
        laserPointer2.PointerIn -= HandlePointerIn;
        laserPointer2.PointerOut -= HandlePointerOut;
        if (handIndexesSet)
        {
            if (leftTrackedController)
                leftTrackedController.TriggerClicked -= HandleTriggerClicked;
            if (rightTrackedController)
                rightTrackedController.TriggerClicked -= HandleTriggerClicked;
        }

    }

    private void HandleTriggerClicked(object sender, ClickedEventArgs e)
	{
        if (e.controllerIndex == leftHandIndex)
        {
            //Debug.Log("Left trigger clicked!");
            SelectByLaser(laserPointer1, leftTargetedObject); 
        }

        if (e.controllerIndex == rightHandIndex)
        {
            //Debug.Log("Right trigger clicked!");
            SelectByLaser(laserPointer2, rightTargetedObject);
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
        SubscriptionOff();
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
        UpdateHands();
        SubscriptionOn();

    }

    private void Update()
    {
        if (swapHands)
        {
            SwapHands();
            swapHands = false;
        }

        if (!handIndexesSet)
        {
            FindHandIndexes();
            handIndexesSet = CheckIndex();
            if (handIndexesSet)
                UpdateHands();
        }


    }

}
