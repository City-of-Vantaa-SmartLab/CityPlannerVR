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
    public enum ToolType { Empty, Eraser, EditingLaser, Painter, Camera, CommentLaser };  //includes modes for tools
    public int toolRights;
    public BitArray toolRights2;
    public int toolStatus;
    public enum ToolType { Empty, Eraser, Laser, Painter, Camera, VideoCamera };

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

        OnToolChange += PreventInteraction;
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
        //int[] boolArray;
        int magic;
        switch (tool)
        {
            case ToolType.Empty:
                magic = 1; // same as "0000000000000001" as in empty
                break;

            case ToolType.Painter:
                magic = Convert.ToInt32("0000000000010000", 2);
                break;

            case ToolType.Eraser:
                magic = Convert.ToInt32("0000000000100000", 2);
                break;

            case ToolType.Camera:
                magic = Convert.ToInt32("0000000001000000", 2);
                break;

            case ToolType.EditingLaser:
                magic = Convert.ToInt32("0000000000000010", 2);
                break;

            case ToolType.CommentLaser:
                magic = Convert.ToInt32("0000000010000000", 2);
                break;

            default:
                magic = 1; // same as "0000000000000001" as in empty
                Debug.Log("Invalid tool: " + tool);
                break;
        }
        return magic;
    }

    // toolRights table v0.5
    // 0000 0000 0000 0001 = empty
    // 0000 0000 0000 0010 = duunarilaser
    // 0000 0000 0000 0100 = peukutus (included in commentlaser?)
    // 0000 0000 0000 1000 = commenting (included in commentlaser?)

    // 0000 0000 0001 0000 = painter
    // 0000 0000 0010 0000 = eraser
    // 0000 0000 0100 0000 = camera
    // 0000 0000 1000 0000 = commentlaser

    // 0000 0001 0000 0000 = 
    // 0000 0010 0000 0000 = 
    // 0000 0100 0000 0000 = 
    // 0000 1000 0000 0000 = 

    // 0001 0000 0000 0000 = move objects
    // 0010 0000 0000 0000 = spawn objects
    // 0100 0000 0000 0000 = reset/change scene
    // 1000 0000 0000 0000 = change rights (rights from roles/hats?)

    // tools by role v0.5
    // 0000 0000 1000 0101 : Bystander
    // 0010 0000 1111 1111 : Worker
    // 1111 1111 1111 1111 : Admin

    // tool input properties v0.5
    // 0000 0000 1111 1110 : Teleport // currently every tool except "empty"
    // 0000 0000 1111 1110 : Grab


    private int GetIntForRole(InputMaster.RoleType newRole)
    {
        int magic;
        switch (newRole)
        {
            case InputMaster.RoleType.Spectator:
                magic = Convert.ToInt32("0000000010000101", 2);
                break;

            case InputMaster.RoleType.Worker:
                magic = Convert.ToInt32("0010000011111111", 2);
                break;

            case InputMaster.RoleType.Admin:
                magic = Convert.ToInt32("1111111111111111", 2);
                break;

            default:
                Debug.LogError("Invalid role!");
                magic = 1; // same as "0000000000000001" as in empty
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
