using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;

/// <summary>
/// The method OnClicked is called by Inputmaster in the method SelectByLaser, hover methods are called by PhotonLaserManager.
/// </summary>


public class LaserButton : MonoBehaviour {

    public UnityEvent triggeredEvents;
    public UnityEvent hoverInEvents;
    public UnityEvent hoverOutEvents;

    MeshRenderer meshRenderer;
    Material material;
    Color materialColor;

    UnityEngine.UI.Image image;

    PlayComment playComment;
    string commentName;

    public void OnClicked()
    {
        //Debug.Log("LASERBUTTON " + gameObject.name + ":ssa TESTAA");
        triggeredEvents.Invoke();
    }

    public void OnHoverIn()
    {
        hoverInEvents.Invoke();
    }

    public void OnHoverOut()
    {
        hoverOutEvents.Invoke();
    }
    public void TestFunction()
    {
        //Debug.Log("Testing event!");
    }

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

    public void SetUI()
    {
        if(image == null)
        {
            image = GetComponent<UnityEngine.UI.Image>();
            materialColor = image.color;
        }
    }

    public void OnHoverUI()
    {
        image.color = Color.blue;
    }

    public void OnStopHoverUI()
    {
        image.color = materialColor;
    }

    public void SetMaterial()
    {
        if(meshRenderer == null)
        {
            meshRenderer = GetComponent<MeshRenderer>();
            material = meshRenderer.material;
            materialColor = material.color;
        }
    }

    public void OnHoverButton()
    {
        material.color = Color.yellow;
    }

    public void OnStopHoverButton()
    {
        material.color = materialColor;
    }

#if UNITY_EDITOR
    //using UnityEditor;

    [UnityEditor.CustomEditor(typeof(LaserButton))]
    public class LaserButtonEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            LaserButton laserButtonScript = (LaserButton)target;
            if (GUILayout.Button("Test Triggered events"))
            {
                laserButtonScript.OnClicked();
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
