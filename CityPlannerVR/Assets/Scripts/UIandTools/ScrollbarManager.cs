using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Attach this script to a gameobject with a misbehaving scroll rect. Assign scrollbars as needed.
/// If not using scrollbars use Scroll*Direction* functions instead.
/// </summary>

public class ScrollbarManager : MonoBehaviour {

    public Scrollbar horizontalScrollbar;
    public Scrollbar verticalScrollbar;
    public ScrollRect scrollRect;
    private float xValue;
    private float yValue;

    public delegate void EventWithCoordinates(float xCoord, float yCoord);
    public event EventWithCoordinates UpdateSliderCoordinates;
    //public event EventWithComment ChangeCurrentComment;

    private void Start()
    {
        scrollRect = gameObject.GetComponent<ScrollRect>();
    }

    // Update is called once per frame
    void Update() {
        if (scrollRect)
        {
            if (horizontalScrollbar)
            {
                xValue = horizontalScrollbar.value;
                scrollRect.horizontalNormalizedPosition = xValue;
            }
            if (verticalScrollbar)
            {
                yValue = verticalScrollbar.value;
                scrollRect.verticalNormalizedPosition = yValue;
            }
        }
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
        if (yValue - increment < 0)
            yValue = 0;
        else
            yValue -= increment;
        OnSliderUpdate(xValue, yValue);
    }
    public void ScrollUp(float increment)
    {
        if (yValue + increment > 1)
            yValue = 1;
        else
            yValue += increment;
        OnSliderUpdate(xValue, yValue);
    }

    public void OnSliderUpdate(float xValue, float yValue)
    {
        if (UpdateSliderCoordinates != null)
            UpdateSliderCoordinates(xValue, yValue);
        if (scrollRect)
        {
            scrollRect.horizontalNormalizedPosition = xValue;
            scrollRect.verticalNormalizedPosition = yValue;
        }
    }

}
