using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;
using Valve.VR.InteractionSystem;

/// <summary>
/// Part of an additional layer between networking, device input and eventsystem
/// </summary>

public class InputListener : MonoBehaviour {

	//Might not be needed
	public List<GameObject> players;
	public GameObject localPlayer;

	public GameObject leftHand;
	public GameObject rightHand;
	public uint leftHandIndex;
	public uint rightHandIndex;


	[SerializeField]
	private SteamVR_TrackedController leftTrackedController;
	[SerializeField]
	private SteamVR_TrackedController rightTrackedController;

    [SerializeField]
    private SteamVR_LaserPointer leftLaserPointer;
    [SerializeField]
    private SteamVR_LaserPointer rightLaserPointer;

    //Put here the events broadcasted by this script
    public event ClickedEventHandler TriggerClicked;



	private void OnEnable() {
        localPlayer = GameObject.FindGameObjectWithTag("Player");
        leftHandIndex = OpenVR.System.GetTrackedDeviceIndexForControllerRole
            (ETrackedControllerRole.LeftHand);
		rightHandIndex = OpenVR.System.GetTrackedDeviceIndexForControllerRole
            (ETrackedControllerRole.RightHand);

        leftHand = localPlayer.GetComponentInChildren<GameObject>();
        rightHand = localPlayer.GetComponentInChildren<GameObject>();



        //		leftHand = SteamVR_Controller.Input(SteamVR_Controller.GetDeviceIndex (
        //			SteamVR_Controller.DeviceRelation.Leftmost));
        //		rightHand = SteamVR_Controller.GetDeviceIndex (
        //			SteamVR_Controller.DeviceRelation.Rightmost);
        leftTrackedController = leftHand.GetComponentInChildren<SteamVR_TrackedController> ();
		rightTrackedController = rightHand.GetComponentInChildren<SteamVR_TrackedController> ();

		leftTrackedController.controllerIndex = leftHandIndex;
		rightTrackedController.controllerIndex = rightHandIndex;

        

//
//
//		leftTrackedController.controllerIndex = SteamVR_Controller.GetDeviceIndex (leftHand);

		//subscribe only to the events called by the local player's tracked object


	}


	private void OnDisable() {


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


}
