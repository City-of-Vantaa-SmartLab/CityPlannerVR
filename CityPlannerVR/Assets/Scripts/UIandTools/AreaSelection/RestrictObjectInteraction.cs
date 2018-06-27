using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestrictObjectInteraction : MonoBehaviour {

    //TODO: muuta tämä
    string restrictionLayer = "Default";
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

    //Is called from AreaSelection script
    public void SetOwnerName(string owner)
    {
        this.owner = owner;
    }

    public void OnTriggerEnter(Collider other)
    {
        if(playerAvatarName != owner)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer(normalLayer))
            {
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
                for (int i = 0; i < objectsInCollider.Count; i++)
                {
                    if(objectsInCollider[i] == other.gameObject)
                    {
                        isInList = true;
                        return;
                    }
                }

                if (!isInList) {
                    objectsInCollider.Add(other.gameObject);
                    other.gameObject.layer = LayerMask.NameToLayer(restrictionLayer);
                    isInList = false;
                }
            }
        }  
    }

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
