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
    VoiceBroadcastTrigger voiceTrigger;
    InputMaster inputMaster;
	public LaserPointer laser;

    string whisperTarget;
	string playerTag = "VRLocalPlayer";

    GameObject indicator;

    [HideInInspector]
    public string playerName;

    private void Start()
    {
        comms = GameObject.Find("DissonanceSetup").GetComponent<DissonanceComms>();
        indicator = GameObject.Find("VoiceIndicator");
        laser = GameObject.Find("Laserpointer1").GetComponent<LaserPointer>();
        
        indicator.SetActive(false);

        inputMaster = GameObject.Find("Player").GetComponent<InputMaster>();

        voiceTrigger = GetComponent<VoiceBroadcastTrigger>();

        player = comms.FindPlayer(comms.LocalPlayerName);

        playerName = comms.LocalPlayerName;

        inputMaster.Gripped += ToggleMutePlayer;

        player.OnStartedSpeaking += ToggleIndicator;
        player.OnStoppedSpeaking += ToggleIndicator;

		//These could also be two different functions,but they aren't
		laser.PointerIn += Whisper;
		laser.PointerOut += Whisper;
    }

    private void OnDestroy()
    {
        //When a player stops playing, we don't need to know if they are still talking
        player.OnStartedSpeaking -= ToggleIndicator;
        player.OnStoppedSpeaking -= ToggleIndicator;
    }

    //TODO: If a player is recording a comment, mute player
    void ToggleMutePlayer(object sender, ClickedEventArgs e)
    {
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
            if(comms.IsMuted == false)
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
        else
        {
            //Put indicator off
            indicator.SetActive(false);
        }
	}

    //TODO: how is it determined who's the target
	//TODO: test this
	void Whisper(object sender, LaserEventArgs e)
    {
		if (e.target.tag == playerTag) {
			whisperTarget = e.target.GetComponent<VoiceController>().playerName;

			voiceTrigger.ChannelType = CommTriggerTarget.Player;
            voiceTrigger.PlayerId = whisperTarget;
		}
		else{
            //Let's change it back when we are done whispering
            voiceTrigger.ChannelType = CommTriggerTarget.Self;
		}
    }
}
