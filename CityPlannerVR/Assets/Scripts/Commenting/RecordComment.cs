using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

using System.Xml;
using System.Xml.Serialization;

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

    //The object we are commenting
    GameObject target;

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
        target = FindObjectOfType<GameObject>();
    }

    int temp = 0;

    private void Update()
    {
        if (temp == 10)
        {
            RecordAudio();
            temp++;
        }

        else if (temp == 30)
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
            DisableVoiceChat();

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

    void DisableVoiceChat()
    {
        if(voiceTrigger.Mode != Dissonance.CommActivationMode.None)
        {
            voiceTrigger.Mode = Dissonance.CommActivationMode.None;
            Microphone.End(null);
        }
    }
//-----------------------------------------------------------------------------------------------------------------------------------------------------
    string directoryName = "VoiceComments";
    char slash = Path.DirectorySeparatorChar;

    DirectoryInfo files;
    FileInfo[] fileInfo;

    [HideInInspector]
    public PositionData position;

    public PositionDatabase positionDB;

    void SaveRecordedAudio()
    {
        string filename = "VoiceComment_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

        SavWav.Save(filename, audioClip, directoryName, slash);

        string path = Application.persistentDataPath + slash + directoryName + slash + "Positions.txt";

        XmlSerializer serializer = new XmlSerializer(typeof(PositionDatabase));
        FileStream file = File.Create(path);

        for (int i = 0; i < ObjectContainer.objects.Count; i++)
        {
            positionDB.list.Add(new PositionData());

            positionDB.list[i].recordName = filename;
            positionDB.list[i].targetName = target.name;

            positionDB.list[i].position[0] = target.transform.position.x;
            positionDB.list[i].position[1] = target.transform.position.y;
            positionDB.list[i].position[2] = target.transform.position.z;
        }


        serializer.Serialize(file, positionDB);
        file.Close();

    }
}

[System.Serializable]
public class PositionData
{
    public string recordName;
    public string targetName;
    public float[] position = new float[3];
}

[System.Serializable]
public class PositionDatabase
{
    public List<PositionData> list = new List<PositionData>();
}