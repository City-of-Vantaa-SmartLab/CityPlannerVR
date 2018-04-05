using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Valve.VR;
using Valve.VR.InteractionSystem;
using System;
using Photon;


public class PhotonLaserManager : PunBehaviour {

    [SerializeField]
    private GameObject myHand;
    [SerializeField]
    private GameObject otherHand;
    [SerializeField]
    private InputListener inputListener;
    [SerializeField]
    private GameObject laserCube;

    public uint hand1Index = 3;
    public uint hand2Index = 4;
    public uint myHandIndex;
    public GameObject myTargetedObject;
    [SerializeField]
    private PhotonLaserManager otherLaserManager;
    [SerializeField]
    private SteamVR_LaserPointer myPointer;
    [SerializeField]
    private SteamVR_LaserPointer otherPointer;

    private void Awake()
    {

    }

    private void Start()
    {
        myHandIndex = gameObject.GetComponentInParent<SteamVR_TrackedController>().controllerIndex;
        WhichHandIsMine();
        myPointer = myHand.GetComponentInChildren<SteamVR_LaserPointer>();
        inputListener = GameObject.Find("Player").GetComponent<InputListener>();
        SubscriptionOn();


        Invoke("FindOther", 1);


    }

    private void OnDestroy()
    {
        SubscriptionOff();
    }

    private void FindOther()
    {
        otherPointer = otherHand.GetComponentInChildren<SteamVR_LaserPointer>();
        otherLaserManager = otherPointer.gameObject.GetComponent<PhotonLaserManager>();
        laserCube = gameObject.transform.GetChild(0).GetChild(0).gameObject;
        photonView.RPC("ActivateObject", PhotonTargets.All, false);
    }

    private void SubscriptionOn()
    {
        inputListener.MenuButtonClicked += HandleMenuClicked;
        inputListener.TriggerClicked += HandleTriggerClicked;
        myPointer.PointerIn += HandlePointerIn;
        myPointer.PointerOut += HandlePointerOut;

    }

    private void SubscriptionOff()
    {
        inputListener.MenuButtonClicked -= HandleMenuClicked;
        inputListener.TriggerClicked -= HandleTriggerClicked;
        myPointer.PointerIn -= HandlePointerIn;
        myPointer.PointerOut -= HandlePointerOut;
    }

    private void WhichHandIsMine()
    {
        if (myHandIndex == hand1Index)
        {
            myHand = GameObject.Find("Player/SteamVRObjects/Hand1");
            otherHand = GameObject.Find("Player/SteamVRObjects/Hand2");
        }
        else if (myHandIndex == hand2Index)
        {
            otherHand = GameObject.Find("Player/SteamVRObjects/Hand1");
            myHand = GameObject.Find("Player/SteamVRObjects/Hand2");
        }
        else
            Debug.Log("Could not determine hand for " + gameObject.name + " with index: " + myHandIndex);
        if (!otherHand)
            Debug.Log("Could not determine other hand!");

    }

    private void ToggleLaser(object sender, ClickedEventArgs e)
    {
        if (e.controllerIndex == myHandIndex)
        {
            if (myPointer.active == true)
            {
                photonView.RPC("ActivateObject", PhotonTargets.All, false);
                if (otherPointer == null || otherPointer.active == false)
                {
                    inputListener.lasersAreActive = false;
                    inputListener.InvokeLasersAreOff(sender, e);
                }
            }
            else
            {
                photonView.RPC("ActivateObject", PhotonTargets.All, true);
                inputListener.lasersAreActive = true;
            }
        }
    }

    private void HandleMenuClicked(object sender, ClickedEventArgs e)
    {
        ToggleLaser(sender, e);
    }

    private void HandlePointerOut(object sender, PointerEventArgs e)
    {
        if (e.controllerIndex == myHandIndex)
        {
            myTargetedObject = null;
            var highlightScript = e.target.GetComponent<HighlightSelection>();
            if (highlightScript != null)
            {
                if (highlightScript.isHighlighted)
                {
                    if (!otherLaserManager || otherLaserManager.myTargetedObject != myTargetedObject)
                        highlightScript.ToggleHighlight();
                }
            }

            var button = e.target.GetComponent<Button>();
            if (button != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                Debug.Log("HandlePointerOut: ", e.target.gameObject);
            }
        }

    }


    private void HandlePointerIn(object sender, PointerEventArgs e)
    {
        if (e.controllerIndex == myHandIndex)
        {
            myTargetedObject = e.target.gameObject;

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
        
    }

    [PunRPC]
    public void ActivateObject(Boolean active)
    {
        myPointer.active = active;
        laserCube.SetActive(active);
        //gameObject.SetActive(active);
    }

    private void HandleTriggerClicked(object sender, ClickedEventArgs e)
    {
        if (e.controllerIndex == myHandIndex && inputListener)
        {
            inputListener.SelectByLaser(myPointer, myTargetedObject);
        }

    }

}
