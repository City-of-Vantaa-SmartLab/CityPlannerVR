using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonConnection : Photon.PunBehaviour {

	#region Public Variables

	//The PUN loglevel
	public PhotonLogLevel Loglevel = PhotonLogLevel.Informational;

	#endregion

	#region Private Variables

	// Keep track of the current process
	// Since connection is asynchronous and based on callbacks, 
	// we need to keep track of this property to adjust behaviour.
	// Typically used for the OnConnectedToMaster() callback
	bool isConnecting;

	#endregion

	#region Public Methods

	public void Init()
	{
		// No need to join lobby to get list of the rooms.
		PhotonNetwork.autoJoinLobby = false;

		//Allows us to use PhotonNetwork.LoadLevel() on master client
		//and all clients in the same room sync their level automatically
		PhotonNetwork.automaticallySyncScene = true;

		//Force LogLevel
		PhotonNetwork.logLevel = Loglevel;
	}

	public void ConnectToPhoton(string gameVersion)
	{
		isConnecting = true;

		//Check if connected
		if (PhotonNetwork.connected) {
			// Attempt joining a Random Room. If it fails, notified in OnPhotonRandomJoinFailed() and create one.
			PhotonNetwork.JoinRandomRoom();
		} 
		else {
			// Connect to Photon Online Server.
			PhotonNetwork.ConnectUsingSettings(gameVersion);
		}
	}

	#endregion

	#region Photon.PunBehaviour CallBacks

	public override void OnConnectedToMaster()
	{
		Debug.Log("Launcher: OnConnectedToMaster() was called by PUN");

		//Only join room if we actually want to connect
		if (isConnecting) {
			// Join a potential existing room. 
			//If there is, good, if not, callback with OnPhotonRandomJoinFailed()  
			PhotonNetwork.JoinRandomRoom ();
		}
	}

	public override void OnDisconnectedFromPhoton()
	{
		GetComponentInParent<Launcher> ().progressLabel.SetActive (false);
		GetComponentInParent<Launcher> ().controlPanel.SetActive (true);

		Debug.LogWarning ("Launcher: OnDisconnedtedFromPhoton() was called by PUN");
	}

	public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
	{
		Debug.Log("Launcher:OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one.");
		// Failed to join a random room. Create a new room.
		PhotonNetwork.CreateRoom(null, new RoomOptions() {MaxPlayers = GetComponentInParent<Launcher>().MaxPlayersPerRoom}, null);
	}

	public override void OnJoinedRoom()
	{
		Debug.Log ("Launcher: OnJoinedRoom() called by PUN. This client is in a room.");

		//Load world if we are the first player
		//Otherwise rely on PhotonNetwork.automaticallySyncScene to sync the instance
		if(PhotonNetwork.room.PlayerCount == 1) {
			Debug.Log ("First player loading the world");

			PhotonNetwork.LoadLevel ("photonMultiplayer");
		}
	}

	#endregion
}
