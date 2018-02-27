using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum NetworkState
{
	INITIALIZING, CONNECTING_TO_SERVER, CREATING_ROOM, ROOM_CREATED, JOINING_ROOM,
	ROOM_JOINED, PLAYING, SOME_PLAYER_CONNECTED, SOME_PLAYER_DISCONNECTED, DISCONNECTED
}

public class PhotonGameManager : Photon.PunBehaviour {

	public const string MULTIPLAYER_SCENE_NAME = "photonMultiplayer";
	public const string LAUNCHER_SCENE_NAME = "photonLauncher";

	void Awake()
	{
		DontDestroyOnLoad(this);
	}

	#region Photon Messages

	//Called when the local player left the room.
	//Load Launcher scene
	public override void OnLeftRoom()
	{
		SceneManager.LoadScene (LAUNCHER_SCENE_NAME);
	}

	public override void OnPhotonPlayerConnected(PhotonPlayer other)
	{
		Debug.Log ("OnPhotonPlayerConnected() " + other.NickName);

		if (PhotonNetwork.isMasterClient) {
			Debug.Log ("OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.isMasterClient);
			LoadWorld ();
		}
	}

	public override void OnPhotonPlayerDisconnected(PhotonPlayer other)
	{
		Debug.Log ("OnPhotonPlayerDisconnected() " + other.NickName);

		if (PhotonNetwork.isMasterClient) {
			Debug.Log ("OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.isMasterClient);
			LoadWorld ();
		}
	}

	#endregion

	#region Public Methods

	public void LeaveRoom()
	{
		PhotonNetwork.LeaveRoom ();
	}

	#endregion

	#region Private Methods

	void LoadWorld()
	{
		if (!PhotonNetwork.isMasterClient) {
			Debug.LogError ("PhotonNetwork: Trying to load level but not master client.");
		}
		Debug.Log ("PhotonNetwork: Loading Level");
		PhotonNetwork.LoadLevel (MULTIPLAYER_SCENE_NAME);
	}

	#endregion
}
