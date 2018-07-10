using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Scales the player object when player lets go of this object.
/// </summary>

public class ScaleObject : MonoBehaviour
{

    [SerializeField]
    [Tooltip("Object to be scaled")]
    private GameObject objectToScale;

    [SerializeField]
    [Tooltip("The new scale of the object")]
    private Vector3 newScale;
    /// <summary>
    /// The new scale of the object (read only)
    /// </summary>
    public Vector3 NewScale
    {
        get
        {
            return newScale;
        }
    }

    private GameObject localPlayer = null;

    ChangeTeleportProperties changeTeleport;

    /// <summary>
    /// Gravity when player is big
    /// </summary>
    readonly Vector3 normalGravity = new Vector3(0, -9.81f, 0);
    /// <summary>
    /// Gravity when player is small
    /// </summary>
    readonly Vector3 smallGravity = new Vector3(0, -0.5f, 0);

    void Start()
    {
        changeTeleport = GetComponent<ChangeTeleportProperties>();
    }

    //Called from teleport script, when teleported to teleportPoint
    public void ScalePlayer()
    {
        Scale();

        if(objectToScale.gameObject.name == "Player")
        {
            changeTeleport.ChangeProperties();
            ScaleNetworkedPlayerAvatar();
            ScalePoints();
            ChangeGravity();
        }
    }

    /// <summary>
    /// Scales the object locally
    /// </summary>
    public void Scale()
    {
        //Debug.Log("ScalePlayer::Scale: Scaling " + objectToScale.gameObject.name);
        objectToScale.transform.localScale = newScale;
    }
    /// <summary>
    /// Scales the player avatar through network
    /// </summary>
    public void ScaleNetworkedPlayerAvatar()
    {
        if (localPlayer == null)
        {
            FindLocalPlayer();
        }

        PhotonPlayerAvatar pa = localPlayer.GetComponent<PhotonPlayerAvatar>();
        if (pa != null)
        {
            //Debug.Log("ScaleObject::ScaleNetworkedPlayerAvatar: Scaling player! (" + pa.gameObject.name + ")");
            //pa.UpdateScale(newScale);

            PhotonView photonView;
            photonView = pa.GetComponent<PhotonView>();


            //pa.UpdateScale(newScale);
            photonView.RPC("UpdateScale", PhotonTargets.AllBuffered, newScale);

        }

        else
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
            if (player.GetComponent<PhotonView>().isMine)
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

    /// <summary>
    /// Scales area points
    /// </summary>
    private void ScalePoints()
    {
        //Scale points and line when player shrinks down and grows up

        //Player is small and the points should be scaled down
        for (int i = 0; i < AreaSelection.areaPoints.Count; i++)
        {
            if(newScale.x >= 1)
            {
                AreaSelection.areaPoints[i].transform.localScale = newScale * 0.05f;
            }
            else
            {
                AreaSelection.areaPoints[i].transform.localScale = newScale;
            }
        }
    }

    /// <summary>
    /// To allow players to throw objects better, when they are small
    /// </summary>
    private void ChangeGravity()
    {
        CheckPlayerSize checkSize = objectToScale.GetComponent<CheckPlayerSize>();
        if (checkSize.isSmall)
        {
            Physics.gravity = smallGravity;
        }
        else
        {
            Physics.gravity = normalGravity;
        }
    }
}
