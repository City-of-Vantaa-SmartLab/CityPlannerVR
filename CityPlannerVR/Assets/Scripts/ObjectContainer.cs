using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Will contain all sorts of static variables for easy preserve and use info
/// </summary>

public class ObjectContainer : MonoBehaviour {

	[Tooltip("Table where all the objects are moved if they are removed from the table")]
	public GameObject trashTablePoint;
	public GameObject gridObject;

    //Contains all the objects that can snap to the grid
    public static List<GameObject> objects;

	//This is easier to call from other scripts, and its value won't change during runtime
    public static Vector3 trashPoint;
	//For the references to the grid (there is only one at a time)
	public static GameObject grid;

    void Awake()
    {
        objects = new List<GameObject>();
        trashPoint = trashTablePoint.transform.position;
		grid = gridObject;
    }
}
