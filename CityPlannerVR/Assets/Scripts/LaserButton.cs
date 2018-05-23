using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;
using UnityEditor;

/// <summary>
/// This script is called by Inputmaster in the method SelectByLaser.
/// </summary>


public class LaserButton : MonoBehaviour {

    public UnityEvent triggeredEvents;

    PlayComment playComment;
    string commentName;

    public void OnClicked()
    {
        //Debug.Log("LASERBUTTON " + gameObject.name + ":ssa TESTAA");
        triggeredEvents.Invoke();
    }

    public void TestFunction()
    {
        //Debug.Log("Testing event!");
    }

    public void PlayCommentStart()
    {
        playComment.PlayCommentInPosition(commentName);
    }

    public void GoForward()
    {
        if (playComment == null)
        {
            playComment = GameObject.Find("CommentList").GetComponent<PlayComment>();
            commentName = GetComponentInChildren<UnityEngine.UI.Text>().text;
        }

        playComment.GoForward();
    }

    public void GoBackward()
    {
        if (playComment == null)
        {
            playComment = GameObject.Find("CommentList").GetComponent<PlayComment>();
            commentName = GetComponentInChildren<UnityEngine.UI.Text>().text;
            Debug.Log("CommentName = " + commentName);
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
        }
    }
#endif
}
