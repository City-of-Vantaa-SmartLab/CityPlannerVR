using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Photon Networking launcher where players join a room and connect to photon cloud.
/// </summary>

public class Launcher : MonoBehaviour {

	#region Public Variables

	// Photon connection object
	public PhotonConnection connection;

	//The maximum number of players per room. When room is full, a new one is created.
	[Tooltip("The maximum number of players per room. If room is full, new one is created")]
	public byte MaxPlayersPerRoom = 8;

	[Tooltip("The UI Panel to let the user enter name and connect to game")]
	public GameObject controlPanel;

	[Tooltip("The UI Label to inform user that they are connecting to cloud")]
	public GameObject progressLabel;

	#endregion

	#region Private Variables

	// This client's version number.
	// Users are separated from each other by gameversion 
	// (which allows you to make breaking changes).
	string gameVersion = "1";

	#endregion


	void Awake()
	{


	}

	// Use this for initialization
	void Start () 
	{
		connection.Init ();
		progressLabel.SetActive (false);
		controlPanel.SetActive (true);
	}
	
	// Start the connection process.
	// If already connected, attempt to join room
	// If not connected, connect app instance to Photon Cloud
	public void Connect()
	{
		progressLabel.SetActive (true);
		controlPanel.SetActive (false);

		connection.ConnectToPhoton (gameVersion);
	}

}
