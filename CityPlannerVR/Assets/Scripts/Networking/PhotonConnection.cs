using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonConnection : Photon.PunBehaviour {

	#region Public Variables

	//The PUN loglevel
	public PhotonLogLevel Loglevel = PhotonLogLevel.Informational;
	public int count = 0;

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

	// Start the connection process.
	// If already connected, attempt to join room
	// If not connected, connect app instance to Photon Cloud
	public void ConnectToPhoton(string gameVersion)
	{
		isConnecting = true;

		//Check if connected
		if (PhotonNetwork.connected) {
			OnConnectedToMaster ();
		} 
		else {
			// Connect to Photon Online Server.
			PhotonNetwork.ConnectUsingSettings(gameVersion);
		}
	}

	public void LeaveRoom()
	{
		PhotonNetwork.LeaveRoom ();
	}

	public void Disconnect()
	{
		PhotonNetwork.Disconnect ();
	}

	#endregion

	#region Photon.PunBehaviour CallBacks

	public override void OnConnectedToMaster()
	{
		//Only join room if we actually want to connect
		if (isConnecting) {
			isConnecting = false;
			if (!PhotonNetwork.inRoom) {
				PhotonGameManager.Instance.ChangeState (NetworkState.JOINING_ROOM);
				// Join a potential existing room. 
				//If there is, good, if not, callback with OnPhotonRandomJoinFailed()  
				PhotonNetwork.JoinRoom ("tikkuraittiRoom");
			}
		}
	}

	public override void OnDisconnectedFromPhoton()
	{
		Debug.LogWarning ("Launcher: OnDisconnedtedFromPhoton() was called by PUN");
		PhotonGameManager.Instance.ChangeState (NetworkState.DISCONNECTED);
		PhotonGameManager.Instance.LoadLauncher ();
	}

	public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
	{
		Debug.Log("Launcher:OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one.");
		PhotonGameManager.Instance.ChangeState (NetworkState.CREATING_ROOM);
		// Failed to join a random room. Create a new room.
		PhotonNetwork.CreateRoom("tikkuraittiRoom", new RoomOptions() {MaxPlayers = GetComponentInParent<PhotonGameManager>().MaxPlayersPerRoom}, null);
	}

	public override void OnCreatedRoom()
	{
		PhotonGameManager.Instance.ChangeState (NetworkState.ROOM_CREATED);
	}

	public override void OnJoinedRoom()
	{
		Debug.Log ("Launcher: OnJoinedRoom() called by PUN. This client is in a room.");
		//PhotonNetwork.isMessageQueueRunning = false;
		//Load world if we are the first player
		//Otherwise rely on PhotonNetwork.automaticallySyncScene to sync the instance
		if (PhotonNetwork.room.PlayerCount == 1) {
			Debug.Log ("First player loading the world");

			PhotonGameManager.Instance.LoadWorld ();
		}
		PhotonGameManager.Instance.ChangeState (NetworkState.ROOM_JOINED);
	}

	//Called when the local player left the room.
	//Load Launcher scene
	public override void OnLeftRoom()
	{
		PhotonGameManager.Instance.LoadLauncher ();
	}

	public override void OnPhotonPlayerConnected(PhotonPlayer other)
	{
		Debug.Log ("OnPhotonPlayerConnected() " + other.NickName);
		PhotonGameManager.Instance.ChangeState (NetworkState.SOME_PLAYER_CONNECTED, other);

		if (PhotonNetwork.isMasterClient) {
			Debug.Log ("OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.isMasterClient);

		}
	}

	public override void OnPhotonPlayerDisconnected(PhotonPlayer other)
	{
		Debug.Log ("OnPhotonPlayerDisconnected() " + other.NickName);
		PhotonGameManager.Instance.ChangeState (NetworkState.SOME_PLAYER_DISCONNECTED, other);

		if (PhotonNetwork.isMasterClient) {
			Debug.Log ("OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.isMasterClient);
		}
	}

	public override void OnPhotonInstantiate(PhotonMessageInfo info) {
		//Checks after instantiation
	}
		
	#endregion

	public int GetNumberOfClients()
	{
		int countreturn;
		if (count >= 3) {
			count = 0;
			countreturn = count;
			count++;
			return countreturn;
		} 

		else {
			countreturn = count;
			count++;
			return countreturn;
		}

	}
}
