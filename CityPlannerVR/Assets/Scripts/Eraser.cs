using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eraser : MonoBehaviour {

    [SerializeField]
    private uint myDeviceIndex;
    private InputListener inputListener;
    private ToolManager toolManager;
    private ToolManager.ToolType myTool;
    public int myHandNumber;

    MeshRenderer myMesh;
    CapsuleCollider myCollider;

    public delegate void EventWithEraser(uint deviceIndex, Eraser eraser);
    public event EventWithEraser DestroyObjects;
    public event EventWithEraser RemoveFromList;


    // Use this for initialization
    void Start () {
        InitOwn();
        Subscribe();
	}

    private void OnDestroy()
    {
        Unsubscribe();
    }

    private bool InitOwn()
    {
        
        if (myHandNumber == 0)
            Debug.Log("Hand number not set for Eraser! Set at inspector to either 1 or 2");
        inputListener = GameObject.Find("Player").GetComponent<InputListener>();
        toolManager = gameObject.GetComponentInParent<ToolManager>();
        if (toolManager)
            myTool = toolManager.currentTool;

        if (!inputListener || !toolManager)
            return false;
        return true;
    }

    private void Subscribe()
    {
        if (!inputListener)
            inputListener = GameObject.Find("Player").GetComponent<InputListener>();

        if (inputListener)
        {
            inputListener.TriggerClicked += HandleTriggerClicked;
            inputListener.TriggerLifted += HandleTriggerLifted;
            if (myHandNumber == 1)
                inputListener.Hand1DeviceFound += HandleMyIndexFound;
            if (myHandNumber == 2)
                inputListener.Hand2DeviceFound += HandleMyIndexFound;
        }
        else
        {
            Debug.Log("Did not find inputlistener!");
        }
        toolManager.OnToolChange += HandleToolChange;

    }

    private void Unsubscribe()
    {
        if (inputListener)
        {
            inputListener.TriggerClicked -= HandleTriggerClicked;
            inputListener.TriggerLifted -= HandleTriggerLifted;
            if (myHandNumber == 1)
                inputListener.Hand1DeviceFound -= HandleMyIndexFound;
            if (myHandNumber == 2)
                inputListener.Hand2DeviceFound -= HandleMyIndexFound;
        }
        else
        {
            Debug.Log("Did not find inputlistener!");
        }
        toolManager.OnToolChange -= HandleToolChange;
    }

    private void HandleTriggerClicked(object sender, ClickedEventArgs e)
    {
        if (DestroyObjects != null)
        {
            DestroyObjects(e.controllerIndex, this);
        }
    }

    private void HandleTriggerLifted(object sender, ClickedEventArgs e)
    {

    }

    private void HandleToolChange(uint deviceIndex, ToolManager.ToolType tool)
    {
        if (myDeviceIndex == deviceIndex)
        {
            myTool = tool;
            if (tool == ToolManager.ToolType.Eraser)
                ToggleEraser(true);
            else
            {
                ToggleEraser(false);
                if (RemoveFromList != null)
                    RemoveFromList(deviceIndex, this);
            }
        }
            
    }

    private void HandleMyIndexFound(uint deviceIndex)
    {
        myDeviceIndex = deviceIndex;
    }


    private void ToggleEraser(bool truth)
    {
        myMesh.enabled = truth;
        myCollider.enabled = truth;
    }

}
