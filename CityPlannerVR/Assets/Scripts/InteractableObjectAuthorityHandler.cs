﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Description: Change the authority of an object depending on who is touching it
/// TODO: Move FindLocalPlayer to someplace where it makes more sense. We have to find
/// local player in multiple scripts already.
/// </summary>

public class InteractableObjectAuthorityHandler : NetworkBehaviour
{
    GameObject localPlayer = null;

    private void OnAttachedToHand(Valve.VR.InteractionSystem.Hand hand)
    {
        //Debug.Log("InteractableObjectAuthorityHandler::OnGrab: Grabbed!");

        if(localPlayer == null)
        {
            localPlayer = FindLocalPlayer();
        }

        var playerID = localPlayer.GetComponent<NetworkIdentity>();

        localPlayer.GetComponent<PlayerAvatar>().CmdSetAuth(netId, playerID);
    }

    // TODO: This function could be in some kind of GameManager script
    private GameObject FindLocalPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("VRLocalPlayer");
        //Debug.Log("InteractableObjectAuthorityHandler::FindLocalPlayer: Players found: " + players.Length);

        foreach (GameObject player in players)
        {
            if(player.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                localPlayer = player;
                //Debug.Log("InteractableObjectAuthorityHandler::FindLocalPlayer: Local player found!");
            }
        }

        if(localPlayer == null)
        {
            Debug.LogError("InteractableObjectAuthorityHandler::FindLocalPlayer: Could not find local player!");
        }

        return localPlayer;
    }
}
