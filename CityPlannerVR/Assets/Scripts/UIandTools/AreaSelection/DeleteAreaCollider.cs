using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteAreaCollider : MonoBehaviour {

    [HideInInspector]
    public GameObject areaCollider;


    public void DeleteCollider()
    {
        for (int i = 0; i < AreaSelection.areaPoints.Count; i++)
        {
            Destroy(AreaSelection.areaPoints[i]);
        }

        //Destroy the area
        PhotonNetwork.Destroy(areaCollider);
        //Destroy this object (it is only local)
        Destroy(gameObject);
    }
}
