using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Photon Networking launcher where players join a room and connect to photon cloud.
/// </summary>

public class Launcher : MonoBehaviour {

	#region Public Variables

	[Tooltip("The UI Panel to let the user enter name and connect to game")]
	public GameObject controlPanel;

	[Tooltip("The UI Label to inform user that they are connecting to cloud")]
	public GameObject progressLabel;

	#endregion

	void Awake()
	{


	}

	// Use this for initialization
	void Start () 
	{
		EnableUI ();
	}

	public void EnableUI()
	{
		progressLabel.SetActive (false);
		controlPanel.SetActive (true);
	}

	public void DisableUI()
	{
		progressLabel.SetActive (true);
		controlPanel.SetActive (false);

	}

}
