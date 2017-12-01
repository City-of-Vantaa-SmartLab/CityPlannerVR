using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scales the player object when player lets go of this object.
/// </summary>

public class ScaleObject : MonoBehaviour {

    [SerializeField]
    private GameObject objectToScale;

    [SerializeField]
    private Vector3 newScale;

    //private void OnDetachedFromHand(Valve.VR.InteractionSystem.Hand hand)
    //{
    //    // Scale player
    //    Debug.Log("ScalePlayer::OnDetachedFromHand: Scaling");
    //}

    public void Scale()
    {
        Debug.Log("ScalePlayer::Scale: Scaling");
        objectToScale.transform.localScale = newScale;
    }
}
