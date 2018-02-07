using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeasureDistance : MonoBehaviour {

    public GameObject startPoint;
    public GameObject endPoint;

	Pathfinding path;

    float distance;

    void CalculateDistance()
    {
		distance = path.GetDistance (startPoint, endPoint);
		Debug.Log ("Distance is " + distance);
    }
}
