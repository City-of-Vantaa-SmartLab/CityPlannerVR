using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dissonance;

/// <summary>
/// Controls the voice communication properties like muting and unmuting the mic and indicating others when player is speaking
/// </summary>

public class VoiceController : MonoBehaviour {

	DissonanceComms comms;
    VoicePlayerState localPlayer;
    VoiceBroadcastTrigger voiceTrigger;
    InputMaster inputMaster;
	//public LaserPointer laser;

    string whisperTarget;
	string playerTag = "VRLocalPlayer";

    [Tooltip("The object with the particle system to indicate who is speaking")]
    public GameObject indicator;
    AudioSource source;

    [HideInInspector]
    public string playerName;

    private void Start()
    {
        comms = GameObject.Find("DissonanceSetup").GetComponent<DissonanceComms>();
        //laser = GameObject.Find("Laserpointer1").GetComponent<LaserPointer>();
        
        indicator.SetActive(false);

        inputMaster = GameObject.Find("Player").GetComponent<InputMaster>();

        voiceTrigger = GetComponent<VoiceBroadcastTrigger>();

        localPlayer = comms.FindPlayer(comms.LocalPlayerName);

        playerName = comms.LocalPlayerName;

        source = GetComponent<AudioSource>();

        inputMaster.Gripped += ToggleMutePlayer;

        localPlayer.OnStartedSpeaking += ToggleIndicator;
        localPlayer.OnStoppedSpeaking += ToggleIndicator;

		//These could also be two different functions,but they aren't
		//laser.PointerIn += Whisper;
		//laser.PointerOut += Whisper;
    }

    private void OnDestroy()
    {
        //When a player stops playing, we don't need to know if they are still talking
        localPlayer.OnStartedSpeaking -= ToggleIndicator;
        localPlayer .OnStoppedSpeaking -= ToggleIndicator;
    }

    void ToggleMutePlayer(object sender, ClickedEventArgs e)
    {
		if (comms.IsMuted == false) {
			comms.IsMuted = true;
            //indikoi pelaajille mute
            //source.Play();
		}

		else {
			comms.IsMuted = false;
            //indikoi pelaajille unmute
            //source.Play();
		}
	}

    void ToggleIndicator(VoicePlayerState player)
    {
        if (player.IsSpeaking)
        {
            if(comms.IsMuted == false)
            {
                if (player.Name == localPlayer.Name)
                {
                    //Put indicator on
                    indicator.SetActive(true);
                }
            }

            else
            {
                if (player.Name == localPlayer.Name)
                {
                    //Put indicator off
                    indicator.SetActive(false);
                }
            }
        }
        else
        {
            if (player.Name == localPlayer.Name)
            {
                //Put indicator off
                indicator.SetActive(false);
            }
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
