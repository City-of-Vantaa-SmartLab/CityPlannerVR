using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Toolslot and items need to be tagged as ItemSlots or SpawnSlot. Can be used in conjuction with PhotonSpawnableObject, but then itemcontainer's tooltype must be item!.
/// Events also require that the gameobject attached has a trigger collider (checked in ToolManager) and a rigidbody,
/// but this might change in later revisions.
/// </summary>


public class ItemContainer : MonoBehaviour {

    public ToolManager.ToolType tool;
    [HideInInspector]
    public bool isToolContainer;
    public bool persistentContainer;
    private GameObject toolHolder;

    //private SphereCollider sphereCol;

    public UnityEvent clickedEvents;
    public UnityEvent unclickedEvents;
    public UnityEvent hoverInEvents;
    public UnityEvent hoverOutEvents;
    private uint subscribedControllerIndex;  //only used for OnUnclicked method



    void Start()
    {
        PhotonSpawnableObject tempCheck = GetComponent<PhotonSpawnableObject>();
        if (tempCheck)
            tool = ToolManager.ToolType.Item;

        //sphereCol = GetComponent<SphereCollider>();
        if (tool != ToolManager.ToolType.Item)
        {
            isToolContainer = true;
            ReplaceVisibleHolder();
        }
        else
            isToolContainer = false;
    }

    public void OnClicked()
    {
        clickedEvents.Invoke();
    }

    public void OnClicked(object sender, ClickedEventArgs e, InputMaster inputMaster)
    {
        inputMaster.TriggerUnclicked += HandleUnclicked;
        subscribedControllerIndex = e.controllerIndex;
        OnClicked();
    }

    public void OnUnclicked()
    {
        unclickedEvents.Invoke();
    }

    private void OnUnclicked(ClickedEventArgs e, InputMaster inputMaster)
    {
        inputMaster.TriggerUnclicked -= HandleUnclicked;
        subscribedControllerIndex = 0;
        OnUnclicked();
    }

    public void OnHoverIn()
    {
        hoverInEvents.Invoke();
    }

    public void OnHoverOut()
    {
        hoverOutEvents.Invoke();
    }

    private void HandleUnclicked(object sender, ClickedEventArgs e)
    {
        if (subscribedControllerIndex == e.controllerIndex)
        {
            if (sender is InputMaster)
            {
                OnUnclicked(e, sender as InputMaster);
            }
            else
            {
                Debug.LogWarning("Sender not recognized as inputmaster when firing TriggerUnclicked, itemcontainer did not unsubscribe!");
                OnUnclicked();
            }
        }
    }

    public void ReplaceVisibleHolder()
    {
        if (toolHolder != null)
            Destroy(toolHolder);

        GameObject prefab = null;
        if (tool == ToolManager.ToolType.EditingLaser)
            prefab = (GameObject)Resources.Load("Prefabs/ToolHolders/LaserpointerHolder");
        else if (tool == ToolManager.ToolType.Camera)
            prefab = (GameObject)Resources.Load("Prefabs/ToolHolders/CameraHolder");


        if (prefab == null)
        {
            Debug.Log("Could not load prefab!");
            return;
        }

        toolHolder = Instantiate(prefab);

        Vector3 localPos = toolHolder.transform.localPosition;
        toolHolder.transform.parent = transform;
        toolHolder.transform.localScale = Vector3.one;
        toolHolder.transform.localPosition = localPos;
    }

#if UNITY_EDITOR
    //using UnityEditor;

    [UnityEditor.CustomEditor(typeof(ItemContainer))]
    public class ItemContainerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            ItemContainer itemContainerScript = (ItemContainer)target;
            if (GUILayout.Button("Test clicked events"))
            {
                itemContainerScript.OnClicked();
            }
            if (GUILayout.Button("Test unclicked events"))
            {
                itemContainerScript.OnUnclicked();
            }
            if (GUILayout.Button("Test hover in"))
            {
                itemContainerScript.OnHoverIn();
            }
            if (GUILayout.Button("Test hover out"))
            {
                itemContainerScript.OnHoverOut();
            }
        }
    }
#endif


}
