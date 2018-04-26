using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// This script keeps track of what tool is in use in this hand.
/// Subscribe for the event OnToolChange in scripts that uses tools
/// and use the method ChangeTool to switch to a specific tool.
/// </summary>

public class ToolManager : MonoBehaviour {

    public int myHandNumber; //This should be set at inspector to either 1 or 2
    public enum ToolType { Empty, Camera, CommentLaser, EditingLaser, Eraser, Painter, VideoCamera };  //includes modes for tools
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
            if (myHandNumber != 0 && AnnounceToolChanged != null)
            {
                AnnounceToolChanged((uint)myHandNumber, currentTool);
            }
        }
    }

    //For disabling object interactions when holding a tool in a hand
    //Values come from the layer list
    int buildingLayer = 9;
    int measurePointLayer = 11;
    int finalMask;
    Valve.VR.InteractionSystem.Hand hand;


    private int numberOfTools = System.Enum.GetValues(typeof(ToolType)).Length;
    [SerializeField]
    private InputMaster inputMaster;
    [SerializeField]
    private ToolType currentTool;  //used only through property Tool
    [SerializeField]
    private bool grabEnabled;
    [SerializeField]
    private bool teleportEnabled;
    private Valve.VR.InteractionSystem.Teleport teleport;

    public delegate void EventWithIndexTool(uint handNumber, ToolManager.ToolType tool);
    public event EventWithIndexTool AnnounceToolChanged;

    // Use this for initialization
    void Start () {
        FindHandNumber();
        inputMaster = GameObject.Find("Player").GetComponent<InputMaster>();
        SubscriptionOn();
        Tool = ToolType.Empty;
        teleport = GameObject.Find("Teleporting").GetComponent<Valve.VR.InteractionSystem.Teleport>();
        currentTool = ToolType.Empty;

        int buildingLayerMask = 1 << buildingLayer;
        int measureLayerMask = 1 << measurePointLayer;
        finalMask = ~(buildingLayerMask | measureLayerMask);

        hand = gameObject.GetComponent<Valve.VR.InteractionSystem.Hand>();

        AnnounceToolChanged += PreventInteraction;
    }

    private void OnDestroy()
    {
        SubscriptionOff();
    }

    private void SubscriptionOn()
    {
        inputMaster.MenuButtonClicked += HandleMenuClicked;
        inputMaster.RoleChanged += HandleNewRole;
    }

    private void SubscriptionOff()
    {
        inputMaster.MenuButtonClicked -= HandleMenuClicked;
        inputMaster.RoleChanged -= HandleNewRole;
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

    private void HandleNewRole(int index)
    {
        toolRights = GetIntForRole(inputMaster.Role);
        Debug.Log("New Rights int: " + toolRights);
    }

    // move under Tool property?
    public bool ChangeTool(ToolType toolType)
    {
        int test;
        test = GetBitMaskForTool(toolType);

        Debug.Log("Tool " + toolType +" test:" + test + " and testresult: " + (toolRights & test));
        if ((toolRights & test) != 0)
        //if (toolType == ToolType.Empty || GetIntFromBitArray((test2.And(toolRights2))) != 0)
        {
            Tool = toolType;  //setting value triggers event onToolChange
            SetInputPropertiesByTool();
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


    void PreventInteraction(uint deviceIndex, ToolType tool)
    {
        //If there is no tool in players hand, they can interact with objects
        if(Tool == ToolType.Empty)
        {
            hand.hoverLayerMask = -1;
        }
        else
        {
            hand.hoverLayerMask = finalMask;
        }
    }

    private int GetBitMaskForTool(ToolType tool)
    {
        int magic;
        magic = 1 << (int)tool;
        return magic;
    }

    // toolRights table v0.6
    // 0000 0001 = Empty
    // 0000 0010 = Camera
    // 0000 0100 = CommentLaser
    // 0000 1000 = EditingLaser

    // 0001 0000 = Eraser
    // 0010 0000 = Painter
    // 0100 0000 = VideoCamera
    // 1000 0000 = Admin specific stuff (resetting)

    // tools by role v0.6
    // 0000 0101 : Spectator
    // 0111 1111 : Worker
    // 1111 1111 : Admin

    // tool input properties v0.6 // every tool except "empty"
    // 1111 1110 : Teleport
    // 1111 1110 : Grab

    private int GetIntForRole(InputMaster.RoleType newRole)
    {
        int magic;
        switch (newRole)
        {
            case InputMaster.RoleType.Spectator:
                magic = Convert.ToInt32("00000101", 2);
                break;

            case InputMaster.RoleType.Worker:
                magic = Convert.ToInt32("01111111", 2);
                break;

            case InputMaster.RoleType.Admin:
                magic = Convert.ToInt32("11111111", 2);
                break;

            default:
                Debug.LogError("Invalid role!");
                magic = 1; // same as "00000001" as in empty
                break;
        }
        //Debug.Log("Changed to role " + newRole + ": " + magic);
        return magic;
    }

    private void SetInputPropertiesByTool()
    {
        bool teleport;
        bool grab;
        int toolMask = GetBitMaskForTool(Tool);
        int teleportMask = Convert.ToInt32("0000000011111110", 2);
        int grabMask = Convert.ToInt32("0000000011111110", 2); 

        if ((toolMask & teleportMask) != 0)
            teleport = false;
        else
            teleport = true;

        if ((toolMask & grabMask) != 0)
            grab = false;
        else
            grab = true;

        ActivateTeleporting(teleport);
        ActivateGrabbing(grab);
    }

    private void ActivateTeleporting(bool status)
    {
        teleport.disableTeleport = !status;
        Debug.Log("Teleporting active: " + status);
    }

    private void ActivateGrabbing(bool status)
    {

        Debug.Log("Grabbing active(not implemented!): " + status);
    }


}
