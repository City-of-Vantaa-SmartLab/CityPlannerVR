using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Changes the layer of the props inside the collider, so that only the owner of the collider can interact with the objects
/// </summary>
public class RestrictObjectInteraction : MonoBehaviour {

    string restrictionLayer = "LockedProps";
    string normalLayer = "Props";

    string playerAvatarName;

    string owner;
    private List<GameObject> objectsInCollider;
    bool isInList = false;

    private void Start()
    {
        objectsInCollider = new List<GameObject>();
        playerAvatarName = PhotonPlayerAvatar.LocalPlayerInstance.GetComponent<PhotonView>().owner.NickName;
    }

    /// <summary>
    /// Set the name of the owner of this collider
    /// </summary>
    /// <param name="owner">The name of the owner of this collider</param>
    //(Is called from AreaSelection script)
    public void SetOwnerName(string owner)
    {
        this.owner = owner;
    }

    //Change the layer of an object entering this collider if this player is not the owner
    public void OnTriggerEnter(Collider other)
    {
        if(playerAvatarName != owner)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer(normalLayer))
            {
                //Add a reference of restricted objects to a list
                objectsInCollider.Add(other.gameObject);
                other.gameObject.layer = LayerMask.NameToLayer(restrictionLayer);
            }
        }
    }

    //To make sure, that every object in the collider are changed (even if OnTriggerEnter isn't called)
    public void OnTriggerStay(Collider other)
    {
        if (playerAvatarName != owner)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer(normalLayer))
            {
                //Check if the object is in the list
                for (int i = 0; i < objectsInCollider.Count; i++)
                {
                    if(objectsInCollider[i] == other.gameObject)
                    {
                        isInList = true;
                        return;
                    }
                }
                //Only if the object is not in the list, add it, because we don't want duplicates
                if (!isInList) {
                    objectsInCollider.Add(other.gameObject);
                    other.gameObject.layer = LayerMask.NameToLayer(restrictionLayer);
                    isInList = false;
                }
            }
        }  
    }

    //Change the objects layer back when it exits the collider
    public void OnTriggerExit(Collider other)
    {
        if (playerAvatarName != owner)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer(restrictionLayer))
            {
                objectsInCollider.Remove(other.gameObject);
                other.gameObject.layer = LayerMask.NameToLayer(normalLayer);
            }
        }
    }

    //When the collider is destroyed, change every object in it back to normal
    private void OnDestroy()
    {
        while(objectsInCollider.Count > 0)
        {
            objectsInCollider[0].gameObject.layer = LayerMask.NameToLayer(normalLayer);
            objectsInCollider.Remove(objectsInCollider[0].gameObject);
        }
    }
}
