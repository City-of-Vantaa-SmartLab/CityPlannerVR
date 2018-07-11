using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dissonance;

/// <summary>
/// Controls the voice communication properties like muting and unmuting the mic and indicating others when player is speaking
/// </summary>

public class VoiceController : MonoBehaviour
{

    DissonanceComms comms;
    VoicePlayerState localPlayer;
    VoiceBroadcastTrigger voiceTrigger;
    InputMaster inputMaster;
    /// <summary>
    /// Left hand laser
    /// </summary>
    LaserPointer laser1;
    /// <summary>
    /// Right hand laser
    /// </summary>
    LaserPointer laser2;

    PhotonView photonView;
    
    string playerTag = "VRLocalPlayer";

    [Tooltip("The object with the particle system to indicate who is speaking")]
    public GameObject indicator;
    AudioSource source;

    [HideInInspector]
    public string playerName;

    string whisperTarget;
    bool playerIsSpeaking = false;
    public bool PlayerIsSpeaking
    {
        get { return playerIsSpeaking; }
        set
        {
            playerIsSpeaking = value;

            if (playerIsSpeaking)
            {
                indicator.SetActive(true);
            }
            else
            {
                indicator.SetActive(false);
            }
        }
    }

    private void Start()
    {
        comms = GameObject.Find("DissonanceSetup").GetComponent<DissonanceComms>();
        laser1 = GameObject.Find("Hand1/Laserpointer").GetComponent<LaserPointer>();
        laser2 = GameObject.Find("Hand2/Laserpointer").GetComponent<LaserPointer>();

        indicator.SetActive(false);

        inputMaster = GameObject.Find("Player").GetComponent<InputMaster>();

        voiceTrigger = GetComponent<VoiceBroadcastTrigger>();

        localPlayer = comms.FindPlayer(comms.LocalPlayerName);

        source = GetComponent<AudioSource>();

        photonView = GetComponent<PhotonView>();

        inputMaster.Gripped += ToggleMutePlayer;

        localPlayer.OnStartedSpeaking += ToggleIndicator;
        localPlayer.OnStoppedSpeaking += ToggleIndicator;

        //These could also be two different functions,but they aren't
        laser1.PointerIn += Whisper;
        laser1.PointerOut += Whisper;
        laser2.PointerIn += Whisper;
        laser2.PointerOut += Whisper;
    }

    private void OnDestroy()
    {
        //When a player stops playing, we don't need to know if they are still talking
        localPlayer.OnStartedSpeaking -= ToggleIndicator;
        localPlayer.OnStoppedSpeaking -= ToggleIndicator;
    }

    void ToggleMutePlayer(object sender, ClickedEventArgs e)
    {
        if (comms.IsMuted == false)
        {
            comms.IsMuted = true;
            //indikoi pelaajille mute
            //source.Play();
        }

        else
        {
            comms.IsMuted = false;
            //indikoi pelaajille unmute
            //source.Play();
        }
    }
    /// <summary>
    /// Toggle the voice indicator on and off when player starts and stops talking
    /// </summary>
    /// <param name="player">The player who is speaking</param>
    void ToggleIndicator(VoicePlayerState player)
    {
        if (player.IsSpeaking && player.Name == localPlayer.Name)
        {
            if (comms.IsMuted == false)
            {
                //Put indicator on
                photonView.RPC("ChangePlayerIsSpeaking", PhotonTargets.All, new object[] { true, voiceTrigger.Priority });

            }

            else
            {

                //Put indicator off
                photonView.RPC("ChangePlayerIsSpeaking", PhotonTargets.All, new object[] { false, voiceTrigger.Priority });

            }
        }
        else
        {
            if (player.Name == localPlayer.Name)
            {
                //Put indicator off
                photonView.RPC("ChangePlayerIsSpeaking", PhotonTargets.All, new object[] { false, voiceTrigger.Priority });
            }
        }
    }

    /// <summary>
    /// Send the message to everyone if this player is speaking
    /// </summary>
    /// <param name="isSpeaking">The bool that tells whether or not the player is speaking</param>
    /// <param name="info">info about the speaker</param>
    [PunRPC]
    void ChangePlayerIsSpeaking(bool isSpeaking, ChannelPriority priority, PhotonMessageInfo info)
    {
        //Debug.Log(string.Format("Info: {0} {1} {2}", info.sender, info.photonView, info.timestamp));
        if (photonView.owner.NickName == info.sender.NickName)
        {
            PlayerIsSpeaking = isSpeaking;
            voiceTrigger.Priority = priority;
        }
    }

    void Whisper(object sender, LaserEventArgs e)
    {
        if (e.target.tag == playerTag)
        {
            photonView.RPC("SetPlayerDissonanceName", PhotonTargets.All, comms.LocalPlayerName);
            
            whisperTarget = e.target.GetComponent<VoiceController>().playerName;

            voiceTrigger.ChannelType = CommTriggerTarget.Player;
            voiceTrigger.PlayerId = whisperTarget;
        }
        else
        {
            //Let's change it back when we are done whispering
            voiceTrigger.ChannelType = CommTriggerTarget.Self;
        }
    }
    /// <summary>
    /// Sets players dissonance name
    /// </summary>
    /// <param name="name">The dissonance name of the local player for whispering</param>
    /// <param name="info">info about the speaker</param>
    [PunRPC]
    void SetPlayerDissonanceName(string name, PhotonMessageInfo info)
    {
        if (photonView.owner.NickName == info.sender.NickName)
        {
            playerName = name;
        }
    }

}
