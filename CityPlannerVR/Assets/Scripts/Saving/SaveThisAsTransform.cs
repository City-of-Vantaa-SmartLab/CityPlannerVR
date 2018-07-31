using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Adds the gameobject and its children to a list in savedata static class. If this is marked as a part of startup,
/// it will be added to a different list and thus saved to a different file from the others.
/// If this is marked as a holder, it will also save its children.
/// </summary>


public class SaveThisAsTransform : MonoBehaviour {

    public bool isPartOfStartup;
    public Transform parent;
    public TransformData initData;

	void Start () {
        parent = transform.parent;
        if (isPartOfStartup)
        {
            initData = SaveAndLoadTransforms.GenerateTransformData(transform);
            //SaveData.startupObjectsList.Add(gameObject);
        }
        else
        {
            //SaveData.gameObjectsToBeSaved.Add(gameObject);
        }
        SaveData.amountOfTransforms++;
        SubscriptionOn();
	}

    private void OnDestroy()
    {
        //SaveData.gameObjectsToBeSaved.Remove(gameObject);
        SaveData.amountOfTransforms--;
        SubscriptionOff();
    }

    private void SubscriptionOn()
    {
        SaveData.OnQuickResetScene += HandleQuickReset;
        SaveData.OnSlowResetScene += HandleSlowReset;
        SaveData.OnBeforeSaveTransforms += HandleSaveAnnouncement;
    }

    private void SubscriptionOff()
    {
        SaveData.OnQuickResetScene -= HandleQuickReset;
        SaveData.OnSlowResetScene -= HandleSlowReset;
        SaveData.OnBeforeSaveTransforms -= HandleSaveAnnouncement;
    }

    private void HandleQuickReset()
    {
        if (isPartOfStartup)
        {
            SaveAndLoadTransforms.RestoreTransform(gameObject, initData, parent, false);
        }
        else
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void HandleSlowReset()
    {
        PhotonNetwork.Destroy(gameObject);
    }

    private void HandleSaveAnnouncement()
    {
        //Debug.Log(gameObject.name + " received Before save event!");
        SaveAndLoadTransforms.StoreData(transform);
        //Debug.Log(gameObject.name + " was saved as transform!");
        SaveData.transformCount++;
        //SaveData.AddData(SaveAndLoadTransforms.GenerateTransformData(transform));
    }


}
