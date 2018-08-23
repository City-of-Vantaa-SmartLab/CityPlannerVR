using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attach this script to a gameobject with a misbehaving scroll rect. Assign scrollbars as needed.
/// If not using scrollbars Throttlemanager or Scroll*Direction* functions directly.
/// </summary>

public class ScrollbarManager : MonoBehaviour {

    public Scrollbar horizontalScrollbar;
    public Scrollbar verticalScrollbar;
    public ScrollRect scrollRect;
    public ThrottleManager verticalThrottle;
    public ThrottleManager horizontalThrottle;
    public GameObject ObjectList;
    public bool doNotUseScrollbars;
    private float xValue;
    private float yValue;
    private float previousXValue;
    private float previousYValue;
    private float sensitivityVertical;
    private float sensitivityHorizontal;

    public delegate void EventWithCoordinates(float xCoord, float yCoord);
    public event EventWithCoordinates UpdateSliderCoordinates;
    //public event EventWithComment ChangeCurrentComment;

    private void Start()
    {
        if (!scrollRect)
            scrollRect = gameObject.GetComponent<ScrollRect>();
        if (sensitivityVertical == 0)
            sensitivityVertical = 0.01f;
    }

    // Update is called once per frame
    void Update() {
        if (scrollRect)
        {
            if (!doNotUseScrollbars && horizontalScrollbar)
            {
                xValue = horizontalScrollbar.value;
                if (xValue != previousXValue)
                {
                    scrollRect.horizontalNormalizedPosition = xValue;
                    previousXValue = xValue;
                }
            }
            if (!doNotUseScrollbars && verticalScrollbar)
            {
                yValue = verticalScrollbar.value;
                if (yValue != previousYValue)
                {
                    scrollRect.verticalNormalizedPosition = yValue;
                    previousYValue = yValue;
                }
                
            }

            if (horizontalScrollbar || verticalScrollbar)
                OnSliderUpdate(xValue, yValue);

            if (verticalThrottle)
            {
                float temp;
                temp = verticalThrottle.normalAngle - verticalThrottle.driveAngle;
                if (!(Mathf.Abs(temp) < 1))
                {
                    //Debug.Log("Angle difference is sufficient");
                    if (temp > 0)
                    {
                        temp = (temp * sensitivityVertical) / verticalThrottle.drive.maxAngle;
                        ScrollUpOld(temp);
                    }
                    else
                    {
                        temp = (temp * sensitivityVertical) / verticalThrottle.drive.minAngle;
                        ScrollDownOld(temp);
                    }
                    

                }
            }

            if (horizontalThrottle)
            {
                float temp;
                temp = horizontalThrottle.normalAngle - horizontalThrottle.driveAngle;
                if (!(Mathf.Abs(temp) < 1))
                {
                    //Debug.Log("Angle difference is sufficient");
                    if (temp > 0)
                    {
                        temp = (temp * sensitivityHorizontal) / horizontalThrottle.drive.maxAngle;
                        ScrollRight(temp);
                    }
                    else
                    {
                        temp = (temp * sensitivityHorizontal) / horizontalThrottle.drive.minAngle;
                        ScrollLeft(temp);
                    }


                }
            }

        }
    }

    private float CalculateIncrement()
    {
        float increment = 0;

        //                                   The first child of this object is the object that has all the comments as children
        increment = (verticalScrollbar.size / ObjectList.transform.childCount) * 2;

        return increment;
    }

    public void ScrollRight(float increment)
    {
        if (xValue + increment > 1)
            xValue = 1;
        else
            xValue += increment;
        OnSliderUpdate(xValue, yValue);
    }

    public void ScrollLeft(float increment)
    {
        if (xValue - increment < 0)
            xValue = 0;
        else
            xValue -= increment;
        OnSliderUpdate(xValue, yValue);
    }
    public void ScrollDown(float increment)
    {
        if (yValue - CalculateIncrement() < 0)
            yValue = 0;
        else
            yValue -= CalculateIncrement();
        OnSliderUpdate(xValue, yValue);
    }
    public void ScrollUp(float increment)
    {
        if (yValue + CalculateIncrement() > 1)
            yValue = 1;
        else
            yValue += CalculateIncrement();
        OnSliderUpdate(xValue, yValue);
    }

    //-------------------
    public void ScrollDownOld(float increment)
    {
        if (yValue - increment < 0)
            yValue = 0;
        else
            yValue -= increment;
        OnSliderUpdate(xValue, yValue);
    }
    public void ScrollUpOld(float increment)
    {
        if (yValue + increment > 1)
            yValue = 1;
        else
            yValue += increment;
        OnSliderUpdate(xValue, yValue);
    }

    //---------------

    public void OnSliderUpdate(float xCoord, float yCoord)
    {
        if (UpdateSliderCoordinates != null)
            UpdateSliderCoordinates(xCoord, yCoord);
        if (scrollRect)
        {
            scrollRect.horizontalNormalizedPosition = xValue;
            scrollRect.verticalNormalizedPosition = yValue;
        }

        if (horizontalScrollbar)
        {
            horizontalScrollbar.value = xCoord;
        }
        if (verticalScrollbar)
        {
            verticalScrollbar.value = yCoord;
        }
    }

}
