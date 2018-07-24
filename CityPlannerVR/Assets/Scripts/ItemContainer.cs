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
    //[HideInInspector]
    public bool isToolContainer;

    //private SphereCollider sphereCol;

    public UnityEvent clickedEvents;
    public UnityEvent unclickedEvents;
    public UnityEvent hoverInEvents;
    public UnityEvent hoverOutEvents;
    public uint subscribedControllerIndex;



    void Start()
    {
        //sphereCol = GetComponent<SphereCollider>();
        if (tool != ToolManager.ToolType.Item)
            isToolContainer = true;
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
                Debug.LogWarning("Sender not recognized as inputmaster when firing TriggerUnclicked, laserbutton did not unsubscribe!");
                OnUnclicked();
            }
        }
    }

    public void TestingIsFun(int test)
    {
        Debug.Log("I AM TEST " + test);
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
