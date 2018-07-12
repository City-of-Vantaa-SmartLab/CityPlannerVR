using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Give this objects name and facades to the tablet
/// </summary>
public class SetName : MonoBehaviour {

    /// <summary>
    /// Name of this object
    /// </summary>
    private string objectName;
    /// <summary>
    /// Name of this object (read only)
    /// </summary>
    public string ObjectName
    {
        get
        {
            return objectName;
        }
    }
    GameObject[] facades;

    private void Start()
    {
        name = gameObject.name;
        facades = new GameObject[transform.childCount];
        for (int i = 0; i < facades.Length; i++)
        {
            facades[i] = transform.GetChild(i).gameObject;
        }
    }

    public void GiveNameAndFacades()
    {
        
    }
}
