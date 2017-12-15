using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTeleportProperties : MonoBehaviour {

    [SerializeField]
    private Valve.VR.InteractionSystem.Teleport teleportScript;

    [SerializeField]
    private Valve.VR.InteractionSystem.TeleportArc teleportAreaScript;

    [SerializeField]
    private float newArcDistance = 1.0f;

    [SerializeField]
    private float newArcThickness = 0.001f;

    public void ChangeProperties()
    {
        teleportScript.arcDistance = newArcDistance;
        teleportAreaScript.thickness = newArcThickness;
    }
}