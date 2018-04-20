using UnityEngine;

/// <summary>
/// This script keeps track of what tool is in use in this hand.
/// Subscribe for the event OnToolChange in scripts that uses tools
/// and use the method ChangeTool to switch to a specific tool.
/// </summary>

public class ToolManager : MonoBehaviour {

    public int myHandNumber; //This should be set at inspector to either 1 or 2
    public enum ToolType { Empty, Eraser, Laser, Painter, Camera };
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
    private InputMaster inputMaster;
    [SerializeField]

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
    }

    private void SubscriptionOff()
    {
        inputMaster.MenuButtonClicked -= HandleMenuClicked;
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

    public void ChangeTool(ToolType toolType)
    {
        currentTool = toolType;
        if (myHandNumber != 0 && OnToolChange != null)
            OnToolChange((uint)myHandNumber, currentTool);
        Debug.Log("Tool changed to " + currentTool + " on hand" + myHandNumber);
    }

    public void RotateTool(object sender, ClickedEventArgs e)
    {
        if (myHandNumber == e.controllerIndex)
        {
            int tool = (int)currentTool;
            tool++;
            if (tool >= numberOfTools)
                tool = 0;
            ChangeTool((ToolType)tool);
        }
    }

}
