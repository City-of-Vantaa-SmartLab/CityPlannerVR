﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum NetworkState
{
	INITIALIZING, CONNECTING_TO_SERVER, CREATING_ROOM, ROOM_CREATED, JOINING_ROOM,
	ROOM_JOINED, PLAYING, SOME_PLAYER_CONNECTED, SOME_PLAYER_DISCONNECTED, DISCONNECTED
}

public enum NetworkEvent
{
	
}

public class PhotonGameManager : MonoBehaviour {

	#region Public Variables

	public const string MULTIPLAYER_SCENE_NAME = "PhotonDemo";
	public const string LAUNCHER_SCENE_NAME = "photonLauncher";

	//The maximum number of players per room. When room is full, a new one is created.
	[Tooltip("The maximum number of players per room. If room is full, new one is created")]
	public byte MaxPlayersPerRoom = 4;

	//Network related events
	public static event Action<NetworkState> OnNetworkStateChange;
	public static event Action OnGameConnected;
	public static event Action<PhotonPlayer> OnSomePlayerConnected;
	public static event Action<PhotonPlayer> OnSomePlayerDisconnected;

	//Scene Loaded event, so everyone doesn't need to listen to SceneManager
	//Mainly for object loading, etc.
	public static event Action OnMultiplayerSceneLoaded;
	public bool isMultiplayerSceneLoaded { get; private set; }

	public static PhotonGameManager Instance { get; private set; }

	//Prefab for the player avatar and related functionalities
	public GameObject playerPrefab;
	public Vector3 playerSpawnPoint;

	#endregion

	#region Private Variables

	//Track current network state
	NetworkState state = NetworkState.DISCONNECTED;

	public PhotonConnection connection;

	// This client's version number.
	// Users are separated from each other by gameversion 
	// (which allows you to make breaking changes).
	string gameVersion = "1";

	#endregion

	void Awake()
	{
		DontDestroyOnLoad(this);
		Instance = this;
		connection = GetComponent<PhotonConnection> ();
		isMultiplayerSceneLoaded = false;
	}

	void OnEnable()
	{
		PhotonNetwork.OnEventCall += this.ProcessNetworkEvent;
	}

	void OnDisable()
	{
		PhotonNetwork.OnEventCall -= this.ProcessNetworkEvent;
	}

	void Start()
	{
		ChangeState (NetworkState.INITIALIZING);
		Debug.Log ("PhotonNetwork: Initializing network connection");
		connection.Init ();

		SceneManager.sceneLoaded += (scene, loadingMode) =>
		{
			SceneLoaded(scene, loadingMode);
		};
	}

	void SceneLoaded(Scene scene, LoadSceneMode mode)
	{
		//PhotonNetwork.isMessageQueueRunning = true;
		if (scene.name.Equals (MULTIPLAYER_SCENE_NAME)) {
			if (OnMultiplayerSceneLoaded != null) {
				OnMultiplayerSceneLoaded ();
			}
			isMultiplayerSceneLoaded = true;

			if(PhotonPlayerAvatar.LocalPlayerInstance == null) {
				Debug.Log ("Instantiating player");
				InstantiatePlayer ();
			}
		}
	}

	#region Public Methods

	public void Connect()
	{
		ChangeState (NetworkState.CONNECTING_TO_SERVER);
		Debug.Log ("PhotonNetwork: Connecting to server");
		connection.ConnectToPhoton (gameVersion);
	}

	public void LoadLauncher()
	{
		Debug.Log ("PhotonNetwork: Left Room, loading Launcher");
		SceneManager.LoadScene (LAUNCHER_SCENE_NAME);
	}

	public void LoadWorld()
	{
		if (!PhotonNetwork.isMasterClient) {
			Debug.LogError ("PhotonNetwork: Trying to load level but not master client.");
		}
		Debug.Log ("PhotonNetwork: Loading Level");
		PhotonNetwork.LoadLevel (MULTIPLAYER_SCENE_NAME);

	}

	public void ChangeState(NetworkState newstate, object stateData = null)
	{
		Debug.Log ("PhotonNetwork: State change to " + newstate.ToString ());
		state = newstate;

		if (OnNetworkStateChange != null) {
			OnNetworkStateChange (state);
		}

		switch (state) {
			case NetworkState.ROOM_CREATED:
				break;
			case NetworkState.ROOM_JOINED:
				
				ChangeState (NetworkState.PLAYING);
				break;
			case NetworkState.SOME_PLAYER_CONNECTED:
				if (OnSomePlayerConnected != null) {
					OnSomePlayerConnected ((PhotonPlayer)stateData);
				}
				ChangeState (NetworkState.PLAYING);
				break;
			case NetworkState.SOME_PLAYER_DISCONNECTED:
				if (OnSomePlayerDisconnected != null) {
					OnSomePlayerDisconnected ((PhotonPlayer)stateData);
				}
				ChangeState (NetworkState.PLAYING);
				break;
		}
	}

	public NetworkState CurrentNetworkState()
	{
		return state;
	}

	#endregion

	#region Private Methods

	private void InstantiatePlayer()
	{
		if (playerPrefab == null) 
		{
			Debug.LogError("Missing playerPrefab Reference. Please set it up in GameObject 'Game Manager'");
		} 
		else
		{
			Debug.Log("We are Instantiating LocalPlayer from "+Application.loadedLevelName);
			// Player is in a room. Spawn a character for the local player. 
			// It gets synced by using PhotonNetwork.Instantiate
			GameObject player = PhotonNetwork.Instantiate(this.playerPrefab.name, playerSpawnPoint, Quaternion.identity, 0);
			Debug.Log ("Player instantiated at: " + playerSpawnPoint.x.ToString () + "," + playerSpawnPoint.y.ToString () + "," + playerSpawnPoint.z.ToString ());
			Debug.Log ("Actual location: " + player.transform.position.x.ToString () + "," + player.transform.position.y.ToString () + "," + player.transform.position.z.ToString ());
		}
	}

	private void InstantiateObject(string prefabName, Vector3 location, Quaternion rotation){
	
		PhotonNetwork.Instantiate(prefabName, location, rotation, 0);
	}

	private void ProcessNetworkEvent(byte eventcode, object content, int senderid)
	{
		Debug.Log ("Photon Network: Network event received: " + eventcode);
		NetworkEvent receivedNetworkEvent = (NetworkEvent)eventcode;

		switch (receivedNetworkEvent) {
			default:
				break;
		}
	}

	#endregion
}
