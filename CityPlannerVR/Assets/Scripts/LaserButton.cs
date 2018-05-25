using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;
using UnityEditor;

/// <summary>
/// The method OnClicked is called by Inputmaster in the method SelectByLaser, hover methods are called by PhotonLaserManager.
/// </summary>


public class LaserButton : MonoBehaviour {

    public UnityEvent triggeredEvents;
    public UnityEvent hoverInEvents;
    public UnityEvent hoverOutEvents;


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

#if UNITY_EDITOR
    [CustomEditor(typeof(LaserButton))]
    public class LaserButtonEditor : Editor
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
