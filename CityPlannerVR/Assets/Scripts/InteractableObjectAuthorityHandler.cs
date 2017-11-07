using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class InteractableObjectAuthorityHandler : NetworkBehaviour
{

    private void OnAttachedToHand(Valve.VR.InteractionSystem.Hand hand)
    {
        Debug.Log("InteractableObjectAuthorityHandler::OnGrab: Grabbed!");
        var player = GameObject.FindGameObjectWithTag("VRLocalPlayer");
        var playerID = player.GetComponent<NetworkIdentity>();

        this.GetComponent<NetworkIdentity>().AssignClientAuthority(player.GetComponent<NetworkIdentity>().connectionToClient);

        //player.GetComponent<PlayerAvatar>().CmdSetAuth(netId, playerID);
    }
}
