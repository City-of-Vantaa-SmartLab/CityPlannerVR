using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteAreaCollider : MonoBehaviour {

    [HideInInspector]
    public GameObject areaCollider;

    bool canDeleteDestroyer = false;


    public void DeleteCollider()
    {
        AreaSelection.areaColliderSpawned = false;

        while(AreaSelection.areaPoints.Count > 0)
        {
            Destroy(AreaSelection.areaPoints[0]);
            AreaSelection.areaPoints.Remove(AreaSelection.areaPoints[0]);
        }

        //Destroy the area
        PhotonNetwork.Destroy(areaCollider);

        canDeleteDestroyer = true;
    }

    public void DeleteDestroyer()
    {
        //If we touch the destroyer but won't trigger it we should not destroy the destroyer eather
        if (canDeleteDestroyer)
        {
            //Destroy this object (it is only local)
            Destroy(gameObject);
        }
    }
}
