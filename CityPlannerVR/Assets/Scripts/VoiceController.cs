using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dissonance;

/// <summary>
/// Controls the voice communication properties like muting and unmuting the mic and indicating others when player is speaking
/// </summary>

public class VoiceController : MonoBehaviour {

	DissonanceComms comms;
    VoicePlayerState player;


    GameObject indicator;

    private void Start()
    {
        comms = GameObject.Find("DissonanceSetup").GetComponent<DissonanceComms>();
        indicator = GameObject.Find("VoiceIndicator");
        indicator.SetActive(false);

        player = comms.FindPlayer(comms.LocalPlayerName);

        player.OnStartedSpeaking += ToggleIndicator;
        player.OnStoppedSpeaking += ToggleIndicator;
    }

    private void OnDestroy()
    {
        //When a player stops playing, we don't need to know if they are still talking
        player.OnStartedSpeaking -= ToggleIndicator;
        player.OnStoppedSpeaking -= ToggleIndicator;
    }

    //TODO: Subscribe to a button which will be used to toggle mute
    void ToggleMutePlayer(){
		if (comms.IsMuted == false) {
			comms.IsMuted = true;
            //indikoi pelaajille mute
		}

		else {
			comms.IsMuted = false;
            //indikoi pelaajille unmute
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
