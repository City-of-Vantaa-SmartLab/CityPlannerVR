﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSelection : MonoBehaviour
{
    [Tooltip("The AreaCollider prefab. It has all the things needed to make and render the area")]
    public GameObject AreaCollider;
    [Tooltip("The DeleteAreaCollider prefab. It has all the things needed to destroy the area")]
    public GameObject AreaColliderDestroyer;

    [HideInInspector]
    public static bool areaColliderSpawned = false;

    private LaserPointer laser;
    private InputMaster inputMaster;

    ///<summary>
    ///All the objects that we can draw the area on (the table)
    ///</summary>
    private string areaTag = "AreaSelectionPlatform";

    ///<summary>
    ///The point just created
    ///</summary>
    private GameObject areaPoint;
    /// <summary>
    /// Contains reference to every point
    /// </summary>
    public static List<GameObject> areaPoints;
    /// <summary>
    /// Can be sent to others so they can construct the collider
    /// </summary>
    private List<Vector3> areaPointPositions;
    /// <summary>
    /// Vectors can't be sent over network so we store the information in the above vector to this array, when we need to send it
    /// </summary>
    private Vector3[] areaPointArray;

    /// <summary>
    /// Reference to the instantiated AreaCollider object
    /// </summary>
    GameObject areaCollider;
    /// <summary>
    /// Reference to the instantiated AreaColliderDestroyer object
    /// </summary>
    GameObject areaColliderDestroyer;

    CreateAreaCollider createAreaCollider;
    RestrictObjectInteraction restrictObjectInteraction;
    PhotonView photonView;
    CheckPlayerSize checkPlayerSize;

    /// <summary>
    /// The size of points when player is big
    /// </summary>
    Vector3 bigScale;
    /// <summary>
    /// The size of points when player is small
    /// </summary>
    Vector3 smallScale;

    /// <summary>
    /// The one who is creating this area and thus has the only right to modify stuff inside it
    /// </summary>
    private string owner;
    /// <summary>
    /// The one who is creating this area and thus has the only right to modify stuff inside it (read only)
    /// </summary>
    public string Owner
    {
        get { return owner; }
    }

    private void Start()
    {
        laser = GetComponentInChildren<LaserPointer>();
        
        inputMaster = GetComponentInParent<InputMaster>();
        areaPoints = new List<GameObject>();
        areaPointPositions = new List<Vector3>();

        checkPlayerSize = GetComponentInParent<CheckPlayerSize>();
        owner = PhotonPlayerAvatar.LocalPlayerInstance.GetComponent<PhotonView>().owner.NickName;

        bigScale = new Vector3(0.1f, 0.1f, 0.1f);
        smallScale = new Vector3(0.01f, 0.01f, 0.01f);
    }
    /// <summary>
    /// Checks if player is allowed to put a point in this position
    /// </summary>
    /// <param name="target">The object that player hits with a laser</param>
    public void ActivateCreatePoint(GameObject target)
    {
        if (target.tag == areaTag)
        {
            CreatePoint();
        }
    }
    /// <summary>
    /// Creates the point for the areaCollider
    /// </summary>
    private void CreatePoint()
    {
        //if this player has not yet spawned the AreaCollider
        if (!areaColliderSpawned)
        {
            areaCollider = (GameObject)PhotonNetwork.Instantiate(AreaCollider.name, Vector3.zero, Quaternion.identity, 0, null);
            createAreaCollider = areaCollider.GetComponent<CreateAreaCollider>();
            restrictObjectInteraction = areaCollider.GetComponent<RestrictObjectInteraction>();

            areaColliderDestroyer = Instantiate(AreaColliderDestroyer, new Vector3(laser.hitPoint.x, laser.hitPoint.y + 3f, laser.hitPoint.z), Quaternion.identity);
            areaColliderDestroyer.GetComponent<DeleteAreaCollider>().areaCollider = areaCollider;

            areaColliderSpawned = true;
        }
        //To indicate where the point is
        areaPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //Only really needed when working in the editor
        areaPoint.name = "SelectionPoint";
        areaPoint.transform.position = laser.hitPoint;

        if (checkPlayerSize.isSmall)
        {
            areaPoint.transform.localScale = smallScale;
        }
        else
        {
            areaPoint.transform.localScale = bigScale;
        }
        areaPoints.Add(areaPoint);

        //If this instance of areaPointPosition is up to date
        if (areaPoints.Count - areaPointPositions.Count == 1)
        {
            //The list is already up to date
            //Add the latest position from areaPoints to areaPointPositions
            areaPointPositions.Add(areaPoints[areaPoints.Count - 1].transform.position);
        }
        else
        {
            //The list was not yet up to date
            for (int i = areaPointPositions.Count + 1; i < areaPoints.Count - areaPointPositions.Count; i++)
            {
                areaPointPositions.Add(areaPoints[i].transform.position);
            }
        }

        areaPointArray = null;
        //areaPointArray = new Vector3[areaPointPositions.Count];
        areaPointArray = CopyListToArray(areaPoints, areaPoints.Count);
        
        createAreaCollider.CallRPC(areaPointArray, owner);
    }


    private Vector3[] CopyListToArray(List<GameObject> list, int length)
    {
        Vector3[] array = new Vector3[length];

        for (int i = 0; i < length; i++)
        {
            array[i] = list[i].transform.position;
        }

        return array;
    } 
}
