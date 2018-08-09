using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;

/// <summary>
/// Toolslot/items/buttons need to be tagged as an ItemSlot or SpawnSlot. Can be used in conjuction with PhotonSpawnableObject for automation, but then itemcontainer's tooltype must be item!.
/// Events also require that the gameobject attached has a trigger collider (checked in ToolManager) and a rigidbody, but this might change in later revisions.
/// Also remember to check that the player collides with the wanted layer in the collider matrix in project settings -> physics.
/// Use button as the tooltype, if you only use the embedded eventsystem.
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
    public InputMaster.EventWithIndex nullifyItemContainer;
    private uint subscribedControllerIndex;  //only used for OnUnclicked method



    void Start()
    {
        PhotonSpawnableObject tempCheck = GetComponent<PhotonSpawnableObject>();
        if (tempCheck)
            tool = ToolManager.ToolType.Item;

        //sphereCol = GetComponent<SphereCollider>();
        if (tool != ToolManager.ToolType.Item && tool != ToolManager.ToolType.Button)
        {
            isToolContainer = true;
            ReplaceVisibleHolder();
        }
        else
            isToolContainer = false;
    }

    private void OnDisable()
    {
        nullifyItemContainer?.Invoke((int)subscribedControllerIndex);
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

        if (tool == ToolManager.ToolType.Empty)
            return;
        GameObject prefab = null;
        if (tool == ToolManager.ToolType.EditingLaser)
            prefab = (GameObject)Resources.Load("Prefabs/ToolHolders/LaserpointerHolder");
        else if (tool == ToolManager.ToolType.Camera)
            prefab = (GameObject)Resources.Load("Prefabs/ToolHolders/CameraHolder");
        else if (tool == ToolManager.ToolType.RemoteGrabber)
            prefab = (GameObject)Resources.Load("Prefabs/ToolHolders/RemoteGrabberPlaceholder");



        if (prefab == null)
        {
            Debug.Log("Could not load prefab!");
            return;
        }

        toolHolder = Instantiate(prefab);

        Vector3 localPos = toolHolder.transform.localPosition;
        toolHolder.transform.parent = transform;
        toolHolder.transform.localScale = Vector3.one;
        toolHolder.transform.localRotation = Quaternion.identity;
        toolHolder.transform.localPosition = localPos;
    }

    public void Test()
    {
        Debug.Log("Testing itemcontainer");
    }

    //---------------------------------------------------------------------------------------------

    SpriteRenderer sprite;
    int deviceIndex;

    public void OnHoverSprite()
    {
        if(sprite == null)
        {
            sprite = GetComponent<SpriteRenderer>();
        }

        sprite.color = Color.gray;
        SteamVR_Controller.Input(deviceIndex).TriggerHapticPulse(500);
    }

    public void OnStopHoverSprite()
    {
        sprite.color = Color.white;
    }

    /// <summary>
    /// Gets the deviceIndex of a hand used to press this button for haptic feedback call
    /// </summary>
    /// <param name="other">The hand that presses this button</param>
    private void OnTriggerEnter(Collider other)
    {
        deviceIndex = (int)other.GetComponent<Hand>().controller.index;
    }

    //---------------------------------------------------------------------------------------------
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
