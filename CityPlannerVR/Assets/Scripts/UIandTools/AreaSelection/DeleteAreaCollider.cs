using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Destroy the area mesh
/// </summary>
public class DeleteAreaCollider : MonoBehaviour {

    /// <summary>
    /// The area Collider object
    /// </summary>
    [HideInInspector]
    public GameObject areaCollider;

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

        Destroy(gameObject);
    }
}
