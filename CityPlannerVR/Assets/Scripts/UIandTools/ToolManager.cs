using System;
using System.Collections;
using UnityEngine;
using Valve.VR.InteractionSystem;

/// <summary>
/// This script keeps track of what tool is in use in this hand.
/// Subscribe for the event OnToolChange in scripts that uses tools
/// and use the method ChangeTool to switch to a specific tool.
/// </summary>

public class ToolManager : MonoBehaviour {

    public int myHandNumber;
    public enum ToolType { Empty, Camera, CommentTester, EditingLaser, Eraser, Painter, PathCamera, VideoCamera, Item};  //includes modes for tools
    public int toolRights;

    public ToolType Tool
    {
        get
        {
            return currentTool;
        }
        private set
        {
            if (ChangeToolTest(value))
            {
                SetInputPropertiesByToolType(value);
                Debug.Log("Tool changed from " + Tool + " to " + value + " on hand" + myHandNumber);
                currentTool = value;
                if (myHandNumber != 0 && AnnounceToolChanged != null)
                {
                    AnnounceToolChanged((uint)myHandNumber, currentTool);
                }
            }
            else
                Debug.Log("No right for tooltype: " + value);
        }
    }

    //For disabling object interactions when holding a tool in a hand
    //Values come from the layer list
    int buildingLayer = 9;
    int measurePointLayer = 11;
    int finalMask;
    Hand hand;
    AllowTeleportWhileAttachedToHand teleportHover;


    private int numberOfTools = System.Enum.GetValues(typeof(ToolType)).Length;
    [SerializeField]
    private InputMaster inputMaster;
    [SerializeField]
    private ToolType currentTool;  //used only through property Tool
    [SerializeField]
    private ItemContainer activeItemContainer;
    [SerializeField]
    private bool grabEnabled;
    [SerializeField]
    private bool teleportEnabled;
    private Teleport teleport;

    public delegate void EventWithIndexTool(uint handNumber, ToolManager.ToolType tool);
    public event EventWithIndexTool AnnounceToolChanged;

    private void Awake()
    {
        FindHandNumber();
        inputMaster = GameObject.Find("Player").GetComponent<InputMaster>();
        SubscriptionOn();
    }

    void Start () {
        int buildingLayerMask = 1 << buildingLayer;
        int measureLayerMask = 1 << measurePointLayer;
        finalMask = ~(buildingLayerMask | measureLayerMask);

        hand = gameObject.GetComponent<Hand>();
        teleportHover = gameObject.GetComponentInChildren<AllowTeleportWhileAttachedToHand>();

        Tool = ToolType.Empty;
        teleport = GameObject.Find("Teleporting").GetComponent<Teleport>();
    }

    private void OnDestroy()
    {
        SubscriptionOff();
    }

    private void SubscriptionOn()
    {
        inputMaster.TriggerClickedInsideToolbelt += HandleTriggerClickedInsideToolbelt;
        inputMaster.RoleChanged += HandleNewRole;
        inputMaster.ClearActiveItemSlots += HandleClearItemSlots;
    }



    private void SubscriptionOff()
    {
        inputMaster.TriggerClickedInsideToolbelt -= HandleTriggerClickedInsideToolbelt;
        inputMaster.RoleChanged -= HandleNewRole;
        inputMaster.ClearActiveItemSlots -= HandleClearItemSlots;

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

    private void HandleTriggerClickedInsideToolbelt(object sender, ClickedEventArgs e)
    {
        if (myHandNumber == 0)
            FindHandNumber();
        if (e.controllerIndex == myHandNumber && activeItemContainer != null)
        {
            if (activeItemContainer.isToolContainer)
            {
                if (activeItemContainer.persistentContainer)
                {
                    Tool = activeItemContainer.tool;
                }
                else if(!(Tool == activeItemContainer.tool))
                    SwapTools();

            }
            else
            {
                if (activeItemContainer.tool == ToolType.Item)
                {
                    PhotonSpawnableObject photonObject =
                        activeItemContainer.gameObject.GetComponent<PhotonSpawnableObject>();
                    if (photonObject)
                        photonObject.InstantiateRealItem(myHandNumber);
                }
            }
            activeItemContainer.OnClicked(sender, e, inputMaster);
        }
    }

    private void SwapTools()
    {
        ToolType tempTool = Tool;
        Tool = activeItemContainer.tool;

        activeItemContainer.tool = tempTool;

        activeItemContainer.ReplaceVisibleHolder();
    }

    private void HandleNewRole(int index)
    {
        toolRights = GetIntForRole(inputMaster.Role);
        //Debug.Log("New Rights int: " + toolRights);
    }

    private void HandleClearItemSlots(int index)
    {
        activeItemContainer = null;
    }

    public bool ChangeToolTest(ToolType toolType)
    {
        if (toolType == ToolType.Item)
            return false;
        int test;
        test = GetBitMaskForTool(toolType);

        if ((toolRights & test) != 0)
            return true;
        else
            return false;
    }

    public void RotateTool(object sender, ClickedEventArgs e)
    {
        if (myHandNumber == e.controllerIndex)
        {
            int toolToBe = (int)Tool;
            toolToBe++;
            if (toolToBe >= numberOfTools)
                toolToBe = 0;
            while (toolToBe != 0 || !ChangeToolTest((ToolType)toolToBe))
            {
                toolToBe++;
                if (toolToBe >= numberOfTools)
                    toolToBe = 0;
            }
            Tool = (ToolType)toolToBe;
        }
    }

    private int GetBitMaskForTool(ToolType tool)
    {
        int magic;
        magic = 1 << (int)tool;
        return magic;
    }

    // toolRights table v0.7
    // 0000 0001 = Empty
    // 0000 0010 = Camera
    // 0000 0100 = CommentTester
    // 0000 1000 = EditingLaser

    // 0001 0000 = Eraser
    // 0010 0000 = Painter
    // 0100 0000 = PathCamera
    // 1000 0000 = VideoCamera

    // tools by role v0.6
    // 0000 0101 : Spectator
    // 0111 1111 : Worker
    // 1111 1111 : Admin

    // tool input properties v0.6 // every tool except "empty"
    // 1111 1110 : Teleport disabled
    // 1111 1110 : Grab disabled

    private int GetIntForRole(InputMaster.RoleType newRole)
    {
        int magic;
        switch (newRole)
        {
            case InputMaster.RoleType.TEST:
                magic = Convert.ToInt32("11001111", 2);
                break;

            case InputMaster.RoleType.Bystander:
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

    private void SetInputPropertiesByToolType(ToolType toolType)
    {
        bool teleport;
        bool grab;
        int toolMask = GetBitMaskForTool(toolType);
        //int teleportMask = Convert.ToInt32("00000001", 2); //enabled on empty
        //int grabMask = Convert.ToInt32("00000001", 2); 
        int teleportMask = 1;  
        int grabMask = 1; //faster than converting, change if necessary

        if ((toolMask & teleportMask) != 0)
            teleport = true;
        else
            teleport = false;

        if ((toolMask & grabMask) != 0)
            grab = true;
        else
            grab = false;

        ActivateTeleporting(teleport);
        ActivateGrabbing(grab);
    }

    private void ActivateTeleporting(bool status)
    {
        //teleport.disableTeleport = !status;
        if (!teleportHover)
            teleportHover = gameObject.GetComponentInChildren<AllowTeleportWhileAttachedToHand>();

        if (teleportHover)
            teleportHover.teleportAllowed = status;
        else
            Debug.LogError("Could not find teleportHover script!");

        //Debug.Log("Teleporting active: " + status);
    }

    private void ActivateGrabbing(bool status)
    {
        //If there is no tool in players hand, they can interact with objects
        //if (Tool == ToolType.Empty)

        if (status)
        {
            hand.hoverLayerMask = -1;
        }
        else
        {
            hand.hoverLayerMask = finalMask;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ItemSlot") || other.CompareTag("SpawnSlot"))
        {
            //Debug.Log(this.name + " has been entered by " + other.name);
            activeItemContainer = other.gameObject.GetComponent<ItemContainer>();
            if (activeItemContainer == null)
            {
                Debug.LogError("Could not find ItemContainer script from a gameobject tagged as ItemSlot!");
                return;
            }
            else
                activeItemContainer.OnHoverIn();

            if (myHandNumber == 1)
                inputMaster.hand1InsideToolbelt = true;
            else if (myHandNumber == 2)
                inputMaster.hand2InsideToolbelt = true;
            else
                Debug.LogError("Could not determine hand number for toolmanager!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ItemSlot") || other.CompareTag("SpawnSlot"))
        {
            //Debug.Log(this.name + " has left " + other.name);
            if (activeItemContainer != null)
                activeItemContainer.OnHoverOut();

            if (myHandNumber == 1)
                inputMaster.hand1InsideToolbelt = false;
            else if (myHandNumber == 2)
                inputMaster.hand2InsideToolbelt = false;
            else
                Debug.LogError("Could not determine hand number for toolmanager!");
            activeItemContainer = null;
        }
    }
}
