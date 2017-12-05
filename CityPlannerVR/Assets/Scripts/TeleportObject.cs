using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Teleports specified object to specified location
/// </summary>

public class TeleportObject : MonoBehaviour {

    [SerializeField]
    private GameObject objectToTeleport;

    [SerializeField]
    private Transform teleportPosition;

    public void TeleportObjectTo()
    {
        objectToTeleport.transform.position = teleportPosition.position;
    }
}
