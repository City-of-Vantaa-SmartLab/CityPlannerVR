using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightLine : MonoBehaviour {

    public GameObject go1;
    public GameObject go2;
    private LineRenderer line;
    public float lineWidth;
    public bool alreadySet = false;

    void Awake()
    {
        line = this.gameObject.AddComponent<LineRenderer>();
        if (lineWidth == 0)
            lineWidth = 0.005F;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.positionCount = 2;
    }

    public void UpdateLine()
    {
        if (go1 != null && go2 != null && line != null)
        {
            line.SetPosition(0, go1.transform.position);
            line.SetPosition(1, go2.transform.position);
            alreadySet = true;
        }
        else
            Debug.Log("At least one object is null!");
    }
}