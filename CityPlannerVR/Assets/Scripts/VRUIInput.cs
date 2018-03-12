using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; 
using UnityEngine.UI; 
 
/// <summary> 
/// Handles events called in SteamVR_LaserPointer AND SteamVR_TrackedController scripts. 
/// </summary> 
 
 
[RequireComponent(typeof(SteamVR_LaserPointer))]
public class VRUIInput : MonoBehaviour
{
    public SteamVR_LaserPointer laserPointer;
    public SteamVR_TrackedController trackedController;
    public GameObject targetedObject;


    private void OnEnable()
    {
        laserPointer = GetComponent<SteamVR_LaserPointer>();
        //laserPointer.PointerIn -= HandlePointerIn; 
        laserPointer.PointerIn += HandlePointerIn;
        //laserPointer.PointerOut -= HandlePointerOut; 
        laserPointer.PointerOut += HandlePointerOut;

        trackedController = GetComponent<SteamVR_TrackedController>();
        if (trackedController == null)
        {
            trackedController = GetComponentInParent<SteamVR_TrackedController>();
        }
        //trackedController.TriggerClicked -= HandleTriggerClicked; 
        trackedController.TriggerClicked += HandleTriggerClicked;
        //trackedController.MenuButtonClicked -= HandleMenuClicked; 
        trackedController.MenuButtonClicked += HandleMenuClicked;


    }

    private void OnDisable()
    {
        laserPointer.PointerIn -= HandlePointerIn;
        laserPointer.PointerOut -= HandlePointerOut;
        trackedController.TriggerClicked -= HandleTriggerClicked;
        trackedController.MenuButtonClicked -= HandleMenuClicked;

    }

    //This is how it should work with delegates 
    private void HandleTriggerClicked(object sender, ClickedEventArgs e)
    {
        Debug.Log("Trigger clicked!");
        if (laserPointer.active && targetedObject != null)
        {
            var highlightScript = targetedObject.GetComponent<HighlightSelection>();

            if (highlightScript != null)
            {
                highlightScript.ToggleSelection();
            }
            else
                Debug.Log("Could not find higlightscript!");


        }
        //    if (EventSystem.current.currentSelectedGameObject != null) 
        //    { 
        //      ExecuteEvents.Execute(EventSystem.current.currentSelectedGameObject, new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler); 
        //    } 
    }





    //This is how it should work with delegates 
    private void HandleMenuClicked(object sender, ClickedEventArgs e)
    {
        Debug.Log("Menu clicked!");
        if (laserPointer.enabled == true)
            laserPointer.enabled = false;
        else
            laserPointer.active = true;
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