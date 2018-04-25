using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// This script keeps track of what tool is in use in this hand.
/// Subscribe for the event OnToolChange in scripts that uses tools
/// and use the method ChangeTool to switch to a specific tool.
/// </summary>

// toolRights table v0.4
// 0000 0000 0000 0001 = moving objects
// 0000 0000 0000 0010 = laser
// 0000 0000 0000 0100 = peukutus
// 0000 0000 0000 1000 = commenting

// 0000 0000 0001 0000 = painter
// 0000 0000 0010 0000 = eraser
// 0000 0000 0100 0000 = camera
// 0000 0000 1000 0000 = 

// 0000 0001 0000 0000 = 
// 0000 0010 0000 0000 = 
// 0000 0100 0000 0000 = 
// 0000 1000 0000 0000 = 

// 0001 0000 0000 0000 = 
// 0010 0000 0000 0000 = spawn objects
// 0100 0000 0000 0000 = reset/change scene
// 1000 0000 0000 0000 = change rights

public class ToolManager : MonoBehaviour {

    public int myHandNumber; //This should be set at inspector to either 1 or 2
    public enum ToolType { Empty, Eraser, Laser, Painter, Camera };
    public int toolRights;
    public BitArray toolRights2;
    public int toolStatus;

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
    private InputMaster inputMaster;
    [SerializeField]
    private ToolType currentTool;
    [SerializeField]
    private bool handhoverEnabled;
    [SerializeField]
    private bool teleportEnabled;


    public delegate void EventWithIndexTool(uint handNumber, ToolManager.ToolType tool);
    public event EventWithIndexTool OnToolChange;

    // Use this for initialization
    void Start () {
        FindHandNumber();
        inputMaster = GameObject.Find("Player").GetComponent<InputMaster>();
        SubscriptionOn();
        currentTool = ToolType.Empty;
    }

private void OnDestroy()
    {
        SubscriptionOff();
    }

    private void SubscriptionOn()
    {
        inputMaster.MenuButtonClicked += HandleMenuClicked;
        inputMaster.ToolRights += HandleNewRights;
    }



    private void SubscriptionOff()
    {
        inputMaster.MenuButtonClicked -= HandleMenuClicked;
        inputMaster.ToolRights -= HandleNewRights;

    }

    private void FindHandNumber()
    {
        if (gameObject.name == "Hand1")
            myHandNumber = 1;
        else if (gameObject.name == "Hand2")
            myHandNumber = 2;
        if (myHandNumber == 0)
            Debug.Log("Hand number could not be determined for toolmanager!");
    }

    private void HandleMenuClicked(object sender, ClickedEventArgs e)
    {
        RotateTool(sender, e);
    }

    private void HandleNewRights(BitArray newRights)
    {
        toolRights = GetIntFromBitArray(newRights);
        toolRights2 = newRights;
        Debug.Log("New Rights int: " + toolRights + " and bitarray: " + toolRights2.Count);
    }

    // move under Tool property?
    public bool ChangeTool(ToolType toolType)
    {
        int test;
        BitArray test2;
        if (toolType != ToolType.Empty)
        {
            test2 = GetBitMaskForTool(toolType);
            test = GetIntFromBitArray(test2);
            //test = 1;
            //test = test << (int)toolType - 1 ; //bitmask for toolrights changed
        }
        else
        {
            test = 0;
            test2 = null;
        }

        Debug.Log("Tool enum entered:" + test + " and testresult: " + (toolRights & test));
        //if (toolType == ToolType.Empty || (toolRights & test) != 0)
        if (toolType == ToolType.Empty || GetIntFromBitArray((test2.And(toolRights2))) != 0)

        {
            Tool = toolType;
            if (myHandNumber != 0 && OnToolChange != null)
            {
                OnToolChange((uint)myHandNumber, Tool);
            }
            Debug.Log("Tool changed to " + Tool + " on hand" + myHandNumber);
            return true;
        }
        else
        {
            Debug.Log("No right for tooltype: " + toolType);
            return false;
        }

    }

    public void RotateTool(object sender, ClickedEventArgs e)
    {
        if (myHandNumber == e.controllerIndex)
        {
            int toolToBe = (int)Tool;
            toolToBe++;
            if (toolToBe >= numberOfTools)
                toolToBe = 0;
            while (!ChangeTool((ToolType)toolToBe))
            {
                toolToBe++;
                if (toolToBe >= numberOfTools)
                    toolToBe = 0;
            }


        }
    }

    private int GetIntFromBitArray(BitArray bitArray)
    {
        if (bitArray.Length > 32)
            throw new ArgumentException("Argument length shall be at most 32 bits.");

        int[] array = new int[1];
        bitArray.CopyTo(array, 0);
        return array[0];
    }

    private BitArray GetBitMaskForTool(ToolType tool)
    {
        int[] boolArray;
        switch (tool)
        {
            case ToolType.Painter:
                boolArray = new int[16] { 0,0,0,0, 0,0,0,0, 0,0,0,1, 0,0,0,0 };
                break;

            case ToolType.Eraser:
                boolArray = new int[16] { 0,0,0,0, 0,0,0,0, 0,0,1,0, 0,0,0,0 };
                break;

            case ToolType.Camera:
                boolArray = new int[16] { 0,0,0,0, 0,0,0,0, 0,1,0,0, 0,0,0,0 };
                break;

            default:
                boolArray = new int[16] { 0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0 };
                Debug.Log("Invalid tool!");
                break;
        }
        BitArray temp = new BitArray(boolArray);
        return temp;
    }

    // 0000 0000 0001 0000 = painter
    // 0000 0000 0010 0000 = eraser
    // 0000 0000 0100 0000 = camera

}
