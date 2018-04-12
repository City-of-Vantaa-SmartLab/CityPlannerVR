using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// This script keeps track of what tool is in use in this hand.
/// Subscribe for the event OnToolChange in scripts that uses tools
/// and use the method ChangeTool to switch to a specific tool.
/// </summary>

public class ToolManager : MonoBehaviour {

    public int myHandNumber; //This should be set at inspector to either 1 or 2
    public enum ToolType { Empty, Eraser, Laser, Painter };
    public ToolType currentTool;

    public ToolType Tool
    {
        get
        {
            return currentTool;
        }
        set
        {
            currentTool = value;
        }
    }


    private int numberOfTools = System.Enum.GetValues(typeof(ToolType)).Length;
    [SerializeField]
    private InputListener inputListener;
    [SerializeField]
    private uint myDeviceIndex;

    public delegate void EventWithIndexTool(uint deviceIndex, ToolManager.ToolType tool);
    public event EventWithIndexTool OnToolChange;


    // Use this for initialization
    void Start () {
        if (myHandNumber == 0)
            Debug.Log("Hand number not set for toolmanager! Set at inspector to either 1 or 2");
        inputListener = GameObject.Find("Player").GetComponent<InputListener>();
        SubscriptionOn();
        currentTool = ToolType.Eraser;
    }

    private void OnDestroy()
    {
        SubscriptionOff();
    }

    private void SubscriptionOn()
    {
        inputListener.MenuButtonClicked += HandleMenuClicked;
        if (myHandNumber == 1)
            inputListener.Hand1DeviceFound += HandleMyIndexFound;
        if (myHandNumber == 2)
            inputListener.Hand2DeviceFound += HandleMyIndexFound;

    }

    private void SubscriptionOff()
    {
        inputListener.MenuButtonClicked -= HandleMenuClicked;
        if (myHandNumber == 1)
            inputListener.Hand1DeviceFound -= HandleMyIndexFound;
        if (myHandNumber == 2)
            inputListener.Hand2DeviceFound -= HandleMyIndexFound;
    }

   private void HandleMenuClicked(object sender, ClickedEventArgs e)
    {
        RotateTool(sender, e);
    }

    private void HandleMyIndexFound(uint deviceIndex)
    {
        myDeviceIndex = deviceIndex;
        //if (myHandNumber == 1)
        //    inputListener.Hand1DeviceFound -= HandleMyIndexFound;
        //if (myHandNumber == 2)
        //    inputListener.Hand2DeviceFound -= HandleMyIndexFound;
    }

    public void ChangeTool(ToolType toolType)
    {
        currentTool = toolType;
        if (myDeviceIndex != 0 && OnToolChange != null)
            OnToolChange(myDeviceIndex, currentTool);
        Debug.Log("Tool changed to " + currentTool + " on DeviceIndex " + myDeviceIndex + " on hand" + myHandNumber);
    }

    public void RotateTool(object sender, ClickedEventArgs e)
    {
        if (myDeviceIndex == e.controllerIndex)
        {
            int tool = (int)currentTool;
            tool++;
            if (tool >= numberOfTools)
                tool = 0;
            ChangeTool((ToolType)tool);
        }
    }

}
