using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eraser : MonoBehaviour {

    [SerializeField]
    private uint myDeviceIndex;
    private InputMaster inputMaster;
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
        CheckTool();
	}


    private void OnDestroy()
    {
        Unsubscribe();
    }

    private bool InitOwn()
    {
        toolManager = GetComponentInParent<ToolManager>();
        if (toolManager)
            myTool = toolManager.currentTool;

        myHandNumber = toolManager.myHandNumber;
        myMesh = gameObject.GetComponent<MeshRenderer>();
        myCollider = gameObject.GetComponent<CapsuleCollider>();

        inputMaster = GameObject.Find("Player").GetComponent<InputMaster>();
        if (!inputMaster || !toolManager)
            return false;
        return true;
    }



    private void Subscribe()
    {
        if (!inputMaster)
            inputMaster = GameObject.Find("Player").GetComponent<InputMaster>();

        if (inputMaster)
        {
            inputMaster.TriggerClicked += HandleTriggerClicked;
            inputMaster.TriggerUnclicked += HandleTriggerUnclicked;
        }
        else
        {
            Debug.Log("Did not find inputlistener!");
        }
        toolManager.OnToolChange += HandleToolChange;

    }

    private void Unsubscribe()
    {
        if (inputMaster)
        {
            inputMaster.TriggerClicked -= HandleTriggerClicked;
            inputMaster.TriggerUnclicked -= HandleTriggerUnclicked;
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

    private void HandleTriggerUnclicked(object sender, ClickedEventArgs e)
    {
        //Keeping trigger pressed would add objects to a list, releasing trigger would destroy them
    }

    private void HandleToolChange(uint handNumber, ToolManager.ToolType tool)
    {
        if (myHandNumber == handNumber)
        {
            myTool = tool;
            if (tool == ToolManager.ToolType.Eraser)
                ToggleEraser(true);
            else
            {
                ToggleEraser(false);
                if (RemoveFromList != null)
                    RemoveFromList(handNumber, this);
            }
        }
            
    }


    private void ToggleEraser(bool truth)
    {
        myMesh.enabled = truth;
        myCollider.enabled = truth;
    }

    private void CheckTool()
    {
        myTool = toolManager.currentTool;
        ToggleEraser(myTool == ToolManager.ToolType.Eraser);
    }

}
