using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour {

    public ToolManager.ToolType tool;
    public GameObject toolGOShown;
    public bool isToolContainer;

    //private SphereCollider sphereCol;


    void Start()
    {
        //sphereCol = GetComponent<SphereCollider>();
        if (tool != ToolManager.ToolType.Empty)
            isToolContainer = true;
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    //Debug.Log(this.name + "'s space has been invaded by: " + other.name);
    //    //did not detect hands! Other objects ok
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    //Debug.Log(other.name + " is no longer inside " + this.name);
    //}

}
