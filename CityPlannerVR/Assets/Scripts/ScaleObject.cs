using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Scales the player object when player lets go of this object.
/// </summary>

public class ScaleObject : MonoBehaviour {

    //[SerializeField]
	//[Tooltip("Poistetaan, sitten kun uusi ratkaisu on valmis")]
    //private GameObject objectToScale;

	[Tooltip("Put all objects to be scaled with this instance here")]
	public GameObject[] objectsToScale;

    //[SerializeField]
    //private Vector3 newScale;
    //public Vector3 NewScale
    //{
    //    get
    //    {
    //        return newScale;
    //    }
    //}

    public Vector3[] newScales;

    private GameObject localPlayer = null;

	ChangeTeleportProperties changeTeleport;

	void Start(){
		changeTeleport = GetComponent<ChangeTeleportProperties> ();
	}

	//Called from teleport script, when teleported to teleportPoint
	public void ScalePlayer(){
		
		Scale ();
		changeTeleport.ChangeProperties ();
		ScaleNetworkedPlayerAvatar ();
	}

    public void Scale()
    {
        //Debug.Log("ScalePlayer::Scale: Scaling " + objectToScale.gameObject.name);
        //objectToScale.transform.localScale = newScale;

        for (int i = 0; i < objectsToScale.Length; i++) {
            objectsToScale[i].transform.localScale = newScales[i];
		}
    }

    public void ScaleNetworkedPlayerAvatar()
    {
        if(localPlayer == null)
        {
            FindLocalPlayer();
        }
                                                 
        PhotonPlayerAvatar pa = localPlayer.GetComponent<PhotonPlayerAvatar>();
        if(pa != null)
        {
            //Debug.Log("ScaleObject::ScaleNetworkedPlayerAvatar: Scaling player! (" + pa.gameObject.name + ")");
            //pa.UpdateScale(newScale);

            PhotonView photonView;
            photonView = pa.GetComponent<PhotonView>();


			//pa.UpdateScale(newScale);							first element is players size (or should be)
            photonView.RPC("UpdateScale", PhotonTargets.AllBuffered, newScales[0]);
            
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
}
