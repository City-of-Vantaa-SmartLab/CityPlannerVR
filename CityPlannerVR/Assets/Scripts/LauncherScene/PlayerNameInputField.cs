using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Player name input field. Name shows up in the game above player. Saved from session to session.
/// </summary>

public class PlayerNameInputField : MonoBehaviour {

	#region Private Variables

	// Store the PlayerPref Key to avoid typos
	static string playerNamePrefKey = "PlayerName";

	#endregion

	// Use this for initialization
	void Start () 
	{

		string defaultName = "";
		InputField inputField = this.GetComponent<InputField> ();
		if (inputField != null) {
			if (PlayerPrefs.HasKey (playerNamePrefKey)) {
				defaultName = PlayerPrefs.GetString (playerNamePrefKey);
				inputField.text = defaultName;
			}
		}

		PhotonNetwork.playerName = defaultName;

	}
	
	#region Public Methods

	// Set the name of the player and save it in PlaerPrefs for future sessions.
	public void SetPlayerName(string value)
	{
		// force a trailing space string in case value is an empty string, 
		// else playerName would not be updated
		PhotonNetwork.playerName = value + " ";

		PlayerPrefs.SetString (playerNamePrefKey, value);
	}

	#endregion
}
