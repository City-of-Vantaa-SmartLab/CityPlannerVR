using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Changes the voice chat priority of a player when holding this object (only for admin)
/// </summary>
public class ChangePriority : MonoBehaviour {

    InputMaster inputMaster;
    Dissonance.VoiceBroadcastTrigger voiceTrigger;

    /// <summary>
    /// Get InputMaster and VoiceBroadcastTrigger every time tis object is picked up
    /// </summary>
    public void GetInputMaster()
    {
        inputMaster = GetComponentInParent<InputMaster>();
        voiceTrigger = PhotonPlayerAvatar.LocalPlayerInstance.GetComponent<Dissonance.VoiceBroadcastTrigger>();
    }

    /// <summary>
    /// Change the admin priority when they pick up and put down the microphone
    /// </summary>
    public void ChangeSpeakingPriority()
    {
        //If the player holding this object is admin
        if (inputMaster.Role == InputMaster.RoleType.Admin)
        {
            if(voiceTrigger.Priority == Dissonance.ChannelPriority.None)
            {
                voiceTrigger.Priority = Dissonance.ChannelPriority.High;
            }
            else
            {
                voiceTrigger.Priority = Dissonance.ChannelPriority.None;
            }
        }
    }
}
