using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSelection : MonoBehaviour
{
    public GameObject AreaCollider;
    public GameObject AreaColliderDestroyer;

    [HideInInspector]
    public static bool areaColliderSpawned = false;

    private LaserPointer laser;
    private InputMaster inputMaster;

    //All the objects that we can draw the area (the table)
    private string areaTag = "AreaSelectionPlatform";
    private static int index;

    GameObject areaPoint;
    public static List<GameObject> areaPoints;

    //Can be sent to others so they can construct the collider
    private List<Vector3> areaPointPositions;
    private Vector3[] areaPointArray;

    GameObject areaCollider;
    GameObject areaColliderDestroyer;
    CreateAreaCollider createAreaCollider;
    RestrictObjectInteraction restrictObjectInteraction;

    PhotonView photonView;

    //The one who is creating this area and thus has the only right to modify stuff inside it
    private string owner;
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
        if (!areaColliderSpawned)
        {
            areaCollider = (GameObject)PhotonNetwork.Instantiate(AreaCollider.name, Vector3.zero, Quaternion.identity, 0, null);
            createAreaCollider = areaCollider.GetComponent<CreateAreaCollider>();
            restrictObjectInteraction = areaCollider.GetComponent<RestrictObjectInteraction>();

            areaColliderDestroyer = Instantiate(AreaColliderDestroyer, new Vector3(laser.hitPoint.x, laser.hitPoint.y + 3f, laser.hitPoint.z), Quaternion.identity);
            areaColliderDestroyer.GetComponent<DeleteAreaCollider>().areaCollider = areaCollider;

            areaColliderSpawned = true;
        }
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

        areaPointArray = null;
        //areaPointArray = new Vector3[areaPointPositions.Count];
        areaPointArray = CopyListToArray(areaPoints, areaPoints.Count);
        
        createAreaCollider.CallRPC(areaPointArray, owner);
    }

    //TODO: scale area points
    private void ScalePoints()
    {
        //Scale points and line when player shrinks down and grows up

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
