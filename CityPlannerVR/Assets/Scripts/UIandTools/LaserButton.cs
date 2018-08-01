using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;
using UnityEngine.UI;

/// <summary>
/// The method OnClicked is called by Inputmaster in the method SelectByLaser, hover methods are called by PhotonLaserManager.
/// OnUnclicked can be called directly or by eventsystem using one of the overloaded OnClicked methods for automatic subscribing.
/// </summary>


public class LaserButton : MonoBehaviour {

    public UnityEvent clickedEvents;
    public UnityEvent unclickedEvents;
    public UnityEvent hoverInEvents;
    public UnityEvent hoverOutEvents;
    uint subscribedControllerIndex;

    MeshRenderer meshRenderer;
    Material material;
    Color materialColor;

    Image image;
    SpriteRenderer sprite;


    PlayComment playComment;
    string commentName;

    RecordComment recordComment;

    public void OnClicked()
    {
        clickedEvents.Invoke();
    }

    /// <summary>
    /// Used to subscribe the OnUnclicked events to inputMaster. Only e.controllerIndex is used from ClickedEventArgs parameter.
    /// </summary>

    public void OnClicked(ClickedEventArgs e, InputMaster inputMaster)
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

    //--------------------------------------------------------------------------------------------------------------------------------
    //ButtonBackground
    public void PlayCommentStart()
    {
        if (playComment == null)
        {
            playComment = GameObject.Find("CommentList").GetComponent<PlayComment>();
        }
        if (commentName == null)
        {
            commentName = GetComponentInChildren<UnityEngine.UI.Text>().text;
        }

        playComment.PlayCommentInPosition(commentName);
    }

    //ForwardButton 
    public void GoForward()
    {
        if (playComment == null)
        {
            playComment = GameObject.Find("CommentList").GetComponent<PlayComment>();
        }

        playComment.GoForward();
    }

    //BackwardButton
    public void GoBackward()
    {
        if (playComment == null)
        {
            playComment = GameObject.Find("CommentList").GetComponent<PlayComment>();   
        }

        playComment.GoBackward();
    }

    //--------------------------------------------------------------------------------------------------------------------------------
    //ButtonBackground prefab
    public void OnHoverUI()
    {
        SetUI();
        image.color = Color.green;
    }

    public void OnUIPressed()
    {
        SetUI();
        image.color = Color.blue;

    }

    private void SetUI()
    {
        if (image == null)
        {
            image = GetComponent<UnityEngine.UI.Image>();
            materialColor = image.color;
        }
    }
    //ButtonBackground prefab
    public void OnStopHoverUI()
    {
        image.color = materialColor;
    }
    //--------------------------------------------------------------------------------------------------------------------------------

    //Different commentTool buttons
    public void OnHoverButton()
    {
        SetMaterial();
        material.color = Color.yellow;
    }

    private void SetMaterial()
    {
        if(meshRenderer == null)
        {
            meshRenderer = GetComponent<MeshRenderer>();
            material = meshRenderer.material;
            materialColor = material.color;
        }
    }

    //Different commentTool buttons
    public void OnStopHoverButton()
    {
        material.color = materialColor;
    }
    //--------------------------------------------------------------------------------------------------------------------------------
    //VoiceComment
    public void StartRecording()
    {
        if(recordComment == null)
        {
            recordComment = gameObject.GetComponent<RecordComment>();
        }

        recordComment.StartRecord();
    }
    //--------------------------------------------------------------------------------------------------------------------------------

    //All sorts of buttons in the hover tablet
    public void OnHoverSprite()
    {
        SetSprite();
        sprite.color = Color.gray;
    }

    private void SetSprite()
    {
        if (sprite == null)
        {
            sprite = GetComponent<SpriteRenderer>();
            materialColor = sprite.color;
        }
    }
    //All sorts of buttons in the hover tablet
    public void OnStopHoverSprite()
    {
        sprite.color = materialColor;
    }


    //--------------------------------------------------------------------------------------------------------------------------------
#if UNITY_EDITOR
    //using UnityEditor;

    [UnityEditor.CustomEditor(typeof(LaserButton))]
    public class LaserButtonEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            LaserButton laserButtonScript = (LaserButton)target;
            if (GUILayout.Button("Test clicked events"))
            {
                laserButtonScript.OnClicked();
            }
            if (GUILayout.Button("Test unclicked events"))
            {
                laserButtonScript.OnUnclicked();
            }
            if (GUILayout.Button("Test hover in"))
            {
                laserButtonScript.OnHoverIn();
            }
            if (GUILayout.Button("Test hover out"))
            {
                laserButtonScript.OnHoverOut();
            }
        }
    }
#endif
}
