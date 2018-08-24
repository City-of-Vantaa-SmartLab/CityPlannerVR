using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A test class for thumbs button, will be removed?
/// </summary>

public class ThumbManager : MonoBehaviour {

	public bool isThumbsUp;

    private void Start()
    {
		
    }

	private void OnEnable()
	{
		this.GetThumbs (isThumbsUp);
		HoverTabletManager.OnTargetChanged += this.TargetChanged;
	}

	private void OnDisable()
	{
		HoverTabletManager.OnTargetChanged -= this.TargetChanged;
	}

	private void TargetChanged()
	{
		this.GetThumbs (isThumbsUp);
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

	public void GetThumbs(bool isUpThumbs)
	{
		List<Comment> allthumbs = SaveData.commentLists.thumbComments;
		int counter = 0;
		Debug.LogWarning ("Found " + allthumbs.Count.ToString () + " thumbs");
		foreach (Comment com in allthumbs) {
			Debug.LogWarning (com.data.dataString + " Thumb found for " + com.data.commentedObjectName);
			if (com.data.commentedObjectName.Equals (HoverTabletManager.CommentTarget.name)) {
				if (isUpThumbs && com.data.dataString.Equals ("1")) {
					counter++;
				} else if (!isUpThumbs && com.data.dataString.Equals ("0")) {
					counter++;
				}
			}

		}
		Debug.LogWarning ("Total thumbs for this: " + counter);
		this.gameObject.GetComponentInChildren<Text> ().text = counter.ToString ();
	}
}
