using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Records and saves an audio 
/// 
/// Made using tutorial from http://www.41post.com/4884/programming/unity-capturing-audio-from-a-microphone
/// </summary>

public class RecordComment : MonoBehaviour
{

    //Tells if there are no microphones connected
    private bool micConnected = false;

    private int minFreq;
    private int maxFreq;

    private AudioClip audioClip;

    private Dissonance.VoiceBroadcastTrigger voiceTrigger;

    private void Start()
    {
        if (Microphone.devices.Length <= 0)
        {
            Debug.LogWarning("Microphone not connected");
        }

        else
        {
            micConnected = true;

            Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);

            //If both are 0, mic supports any frequency
            if (minFreq == 0 && maxFreq == 0)
            {
                maxFreq = 44100;
            }
        }

        voiceTrigger = gameObject.GetComponent<Dissonance.VoiceBroadcastTrigger>();
    }

    int temp = 0;

    private void Update()
    {

        if(temp == 4)
        {
            voiceTrigger.Mode = Dissonance.CommActivationMode.None;
        }
        if(temp == 10)
        {
            
            RecordAudio();
            temp++;
        }

        else if(temp == 20)
        {
            RecordAudio();
            temp++;
        }
        else
        {
            temp++;
        }
    }

    void RecordAudio()
    {
        if (micConnected)
        {
            if (!Microphone.IsRecording(null))
            {
                audioClip = Microphone.Start(null, true, 20, maxFreq);
                Debug.Log("Recording started");
            }
            else
            {
                Microphone.End(null);
                Debug.Log("Recording stopped");
                SaveRecordedAudio();
                voiceTrigger.Mode = Dissonance.CommActivationMode.VoiceActivation;
            }
        }
        else
        {
            Debug.LogError("Microphone not connected");
        }
    }

    string directoryName = "VoiceComments";
    char slash = System.IO.Path.DirectorySeparatorChar;

    void SaveRecordedAudio()
    {
        string filename = "VoiceComment_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

        SavWav.Save(filename, audioClip, directoryName, slash);
    }
}