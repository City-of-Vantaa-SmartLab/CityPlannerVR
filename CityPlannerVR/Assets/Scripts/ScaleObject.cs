using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Scales the player object when player lets go of this object.
/// </summary>

public class ScaleObject : MonoBehaviour {

    [SerializeField]
    private GameObject objectToScale;

    [SerializeField]
    private Vector3 newScale;

    private GameObject localPlayer = null;

    public void Scale()
    {
        Debug.Log("ScalePlayer::Scale: Scaling");
        objectToScale.transform.localScale = newScale;
    }

    public void ScaleNetworkedPlayerAvatar()
    {
        if(localPlayer == null)
        {
            FindLocalPlayer();
        }

        PlayerAvatar pa = localPlayer.GetComponent<PlayerAvatar>();
        if(pa != null)
        {
            //pa.CmdUpdateScale(newScale);
            pa.objScale = newScale;
        } else
        {
            Debug.Log("ScaleObject::ScaleNetworkedPlayerAvatar: Player avatar was null");
        }
    }

    private GameObject FindLocalPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("VRLocalPlayer");
        Debug.Log("ScaleObject::FindLocalPlayer: Players found: " + players.Length);

        foreach (GameObject player in players)
        {
            if (player.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                localPlayer = player;
                Debug.Log("ScaleObject::FindLocalPlayer: Local player found!");
            }
        }

        if (localPlayer == null)
        {
            Debug.LogError("ScaleObject::FindLocalPlayer: Could not find local player!");
        }

        return localPlayer;
    }
}
