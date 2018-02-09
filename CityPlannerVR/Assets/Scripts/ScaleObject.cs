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
        //Debug.Log("ScalePlayer::Scale: Scaling " + objectToScale.gameObject.name);
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
            //Debug.Log("ScaleObject::ScaleNetworkedPlayerAvatar: Scaling player! (" + pa.gameObject.name + ")");
            pa.CmdUpdateScale(newScale);
        } else
        {
            Debug.LogError("ScaleObject::ScaleNetworkedPlayerAvatar: Player avatar was null");
        }
    }

    private void FindLocalPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("VRLocalPlayer");
        //Debug.Log("ScaleObject::FindLocalPlayer: Players found: " + players.Length);

        foreach (GameObject player in players)
        {
            if (player.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                localPlayer = player;
                //Debug.Log("ScaleObject::FindLocalPlayer: Local player found! netID: " + player.GetComponent<NetworkIdentity>().netId);
            }
        }

        if (localPlayer == null)
        {
            Debug.LogError("ScaleObject::FindLocalPlayer: Could not find local player!");
        }
    }
}
