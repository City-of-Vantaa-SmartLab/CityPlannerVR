using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class ToolManager : MonoBehaviour {

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
    private uint currentHandIndex;

    public delegate void EventChangeTool(uint handIndex);
    public event EventChangeTool OnToolChange;
    public event ClickedEventHandler ToolChangeWithCE;


    // Use this for initialization
    void Start () {
        inputListener = gameObject.GetComponentInParent<InputListener>(); //should find components in grandparents, did not work
        inputListener = GameObject.Find("Player").GetComponent<InputListener>();
        SubscriptionOn();
        currentTool = ToolType.Eraser;
        Invoke("GetHandIndex", 1);
    }

    private void OnDestroy()
    {
        SubscriptionOff();
    }

    private void SubscriptionOn()
    {
        inputListener.MenuButtonClicked += HandleMenuClicked;
    }

    private void SubscriptionOff()
    {
        inputListener.MenuButtonClicked -= HandleMenuClicked;
    }

    private void HandleMenuClicked(object sender, ClickedEventArgs e)
    {
        RotateTool(sender, e);
    }

    private void GetHandIndex()
    {
        currentHandIndex = gameObject.GetComponent<SteamVR_TrackedController>().controllerIndex;

    }

    public void ChangeTool(ToolType toolType, uint handIndex)
    {
        if (currentHandIndex == handIndex)
        {
            currentTool = toolType;
            OnToolChange(handIndex);
        }

    }

    public void RotateTool(object sender, ClickedEventArgs e)
    {
        if (currentHandIndex == e.controllerIndex)
        {
            //Debug.Log("Starting to change tool");
            int tool = (int)currentTool;
            tool++;
            if (tool >= numberOfTools)
                tool = 0;
            currentTool = (ToolType)tool;
            ToolChangeWithCE(sender, e);
            Debug.Log("Tool changed to index: " + tool + " " + currentTool);
        }
    }

}
