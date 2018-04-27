using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolContainer : MonoBehaviour {

    public ToolManager.ToolType tool;
    public GameObject toolGOShown;
    private SphereCollider sphereCol;


	// Use this for initialization
	void Start () {
        sphereCol = GetComponent<SphereCollider>();
        
        

	}

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(this.name + "'s space has been invaded by: " + other.name);
        //did not detect hands!
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log(other.name + " is no longer inside " + this.name);
    }

}
