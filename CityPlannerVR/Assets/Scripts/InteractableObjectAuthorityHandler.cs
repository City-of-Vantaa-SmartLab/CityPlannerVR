using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class InteractableObjectAuthorityHandler : NetworkBehaviour
{
    GameObject localPlayer = null;

    private void OnAttachedToHand(Valve.VR.InteractionSystem.Hand hand)
    {
        Debug.Log("InteractableObjectAuthorityHandler::OnGrab: Grabbed!");

        GameObject player = null;
        if(localPlayer == null)
        {
            player = FindLocalPlayer();
        }

        var playerID = player.GetComponent<NetworkIdentity>();

        Debug.Log("Object authority status before: " + hasAuthority);
        player.GetComponent<PlayerAvatar>().CmdSetAuth(netId, playerID);
        Debug.Log("Object authority status after: " + hasAuthority);
    }

    private GameObject FindLocalPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("VRLocalPlayer");
        Debug.Log("Players found: " + players.Length);

        foreach (GameObject player in players)
        {
            if(player.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                localPlayer = player;
                Debug.Log("Local player found!");
            }
        }

        if(localPlayer == null)
        {
            Debug.LogError("InteractableObjectAuthorityHandler::FindLocalPlayer: Could not find local player!");
        }

        return localPlayer;
    }
}
