using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class InteractableObjectAuthorityHandler : NetworkBehaviour
{
    private void OnAttachedToHand(Valve.VR.InteractionSystem.Hand hand)
    {
        Debug.Log("InteractableObjectAuthorityHandler::OnGrab: Grabbed!");
        var player = FindLocalPlayer();
        var playerID = player.GetComponent<NetworkIdentity>();

        player.GetComponent<PlayerAvatar>().SetAuthForObj(netId, playerID);
    }

    private GameObject FindLocalPlayer()
    {
        GameObject localPlayer = null;

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
