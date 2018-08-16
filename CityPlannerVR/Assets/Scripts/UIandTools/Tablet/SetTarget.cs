using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetTarget : MonoBehaviour {

    public Text targetText;

    private void OnEnable()
    {
        HoverTabletManager.OnTargetChanged += ChangeText;
        targetText.text = HoverTabletManager.CommentTarget.name;
    }

    private void OnDisable()
    {
        HoverTabletManager.OnTargetChanged -= ChangeText;
    }

    public void ChangeText()
    {
        targetText.text = HoverTabletManager.CommentTarget.name;
    }
}
