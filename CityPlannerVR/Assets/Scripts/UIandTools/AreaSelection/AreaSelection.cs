﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSelection : MonoBehaviour
{

    private LaserPointer laser;
    private InputMaster inputMaster;

    //TODO: muuta tämä
    private string areaTag = "Untagged";
    private static int index;

    GameObject areaPoint;
    public static List<GameObject> areaPoints;

    //Can be sent to others so they can construct the collider
    private List<Vector3> areaPointPositions;

    GameObject areaCollider;
    CreateAreaCollider createAreaCollider;
    RestrictObjectInteraction restrictObjectInteraction;

    //The one who is creating this area and thus has the only right to modify stuff inside it
    private string owner;
    public string Owner
    {
        get { return owner; }
    }

    private void Start()
    {
        laser = GetComponentInChildren<LaserPointer>();
        areaCollider = GameObject.Find("AreaCollider");
        createAreaCollider = areaCollider.GetComponent<CreateAreaCollider>();
        restrictObjectInteraction = areaCollider.GetComponent<RestrictObjectInteraction>();
        inputMaster = GetComponentInParent<InputMaster>();

        areaPoints = new List<GameObject>();
        areaPointPositions = new List<Vector3>();

        owner = PhotonPlayerAvatar.LocalPlayerInstance.GetComponent<PhotonView>().owner.NickName;

        index = 0;
    }

    public void ActivateCreatePoint(LaserPointer laser, GameObject target)
    {
        if (target.tag == areaTag)
        {
            CreatePoint();
        }
    }

    private void CreatePoint()
    {
        areaPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        areaPoint.name = "SelectionPoint";
        areaPoint.transform.position = laser.hitPoint;
        areaPoint.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
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

        CreateMesh(areaPointPositions, owner);
    }

    //[PunRPC]
    void CreateMesh(List<Vector3> app, string owner)
    {
        createAreaCollider.MakeProceduralMesh(app);
        restrictObjectInteraction.GetOwnerName(owner);
    }


    //TODO: scale area points
    private void ScalePoints()
    {
        //Scale points and line when player shrinks down and grows up

    }
}
