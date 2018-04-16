using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dissonance;

/// <summary>
/// Controls the voice communication properties like muting and unmuting the mic and indicating others when player is speaking
/// </summary>

public class VoiceController : MonoBehaviour {

	DissonanceComms comms;

	GameObject indicator;

    private void Start()
    {
        indicator = GetComponent<GameObject>();

        VoicePlayerState player = comms.FindPlayer(comms.LocalPlayerName);

        player.OnStartedSpeaking += ToggleIndicator;
        player.OnStoppedSpeaking += ToggleIndicator;
    }

    //TODO: Subscribe to a button which will be used to toggle mute
    void ToggleMutePlayer(){
		if (comms.IsMuted == false) {
			comms.IsMuted = true;
		}

		else {
			comms.IsMuted = false;
		}
	}

    void ToggleIndicator(VoicePlayerState player)
    {
        if (player.IsSpeaking)
        {
            //Put indicator on
            indicator.SetActive(true);
        }
        else
        {
            //Put indicator off
            indicator.SetActive(false);
        }
	}

	//TODO: puheen kohdennus (sitten kun tiedetään yksityiskohtia toteutuksesta)
}
