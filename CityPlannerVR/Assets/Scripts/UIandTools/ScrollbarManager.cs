using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Attach this script to a gameobject with a misbehaving scroll rect. Assign scrollbars as needed.
/// </summary>

public class ScrollbarManager : MonoBehaviour {

    public Scrollbar horizontalScrollbar;
    public Scrollbar verticalScrollbar;
    public ScrollRect scrollRect;
    private float xValue;
    private float yValue;

    //public delegate void EventWithCoordinates(float xCoord, float yCoord);
    //public event EventWithCoordinates UpdateSliderCoordinates;
    //public event EventWithComment ChangeCurrentComment;

    private void Start()
    {
        scrollRect = gameObject.GetComponent<ScrollRect>();
    }

    // Update is called once per frame
    void Update() {
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

    //public void ScrollUp(float increment)
    //{
    //    xValue += increment;
    //    OnSliderUpdate(xValue, yValue);
    //}

    //public void ScrollDown(float increment)
    //{
    //    xValue -= increment;
    //    OnSliderUpdate(xValue, yValue);
    //}
    //public void ScrollLeft(float increment)
    //{
    //    yValue -= increment;
    //    OnSliderUpdate(xValue, yValue);
    //}
    //public void ScrollRight(float increment)
    //{
    //    yValue += increment;
    //    OnSliderUpdate(xValue, yValue);
    //}

    //private void OnSliderUpdate(float xCoord, float yCoord)
    //{
    //    if (UpdateSliderCoordinates != null)
    //        UpdateSliderCoordinates(xCoord, yCoord);
    //}

}
