using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A test class for thumbs button, will be removed?
/// </summary>

public class ThumbManager : MonoBehaviour {

    private void Start()
    {
    }

	public void UpdateThumbText()
	{
		int oldNo = int.Parse (this.gameObject.GetComponentInChildren<Text> ().text);
		int newNo = oldNo + 1;

		this.gameObject.GetComponentInChildren<Text> ().text = newNo.ToString ();
	}

	public void CreateThumbUp()
	{
		//NÄILLE EI VIELÄ TEHDÄ MITÄÄN
		Comment newThumb = Comment.GenerateThumbComment ("1", HoverTabletManager.CommentTarget, null);
	}

	public void CreateThumbDown()
	{
		Comment newThumb = Comment.GenerateThumbComment ("0", HoverTabletManager.CommentTarget, null);
	}
}
