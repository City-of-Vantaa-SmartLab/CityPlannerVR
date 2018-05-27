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

    private AudioClip tempAudioClip;
    private AudioClip finalAudioClip;

    [HideInInspector]
    public Dissonance.VoiceBroadcastTrigger voiceTrigger;

    //The object we are commenting
    [HideInInspector]
    public GameObject target;
    //The person who commented
    [HideInInspector]
    public string commenter;


    //Lasers for both hands, so it doesn't matter which hand is used
    LaserPointer laserRight;
    LaserPointer laserLeft;

    string commentLayer = "CommentTool";

    string directoryName = "VoiceComments";
    [HideInInspector]
    public char slash = Path.DirectorySeparatorChar;
    [HideInInspector]
    public string positionFileName = "positions.txt";

    [HideInInspector]
    public PositionData position;
    [HideInInspector]
    public PositionDatabase positionDB;

    private string savePath;
    private string audioSavePathExt;

    public string SavePath
    {
        get
        {
            return savePath;
        }
    }

    public string AudioExt
    {
        get
        {
            return audioSavePathExt;
        }
    }

    private void Start()
    {
#if UNITY_EDITOR
        //TODO: Buildissa on eri polku ehkä
        savePath = Application.dataPath+ slash + "Resources" + slash + "Comments" + slash + directoryName + slash;
        audioSavePathExt = "AudioFiles" + slash;
#endif
        laserLeft = GameObject.Find("Player/SteamVRObjects/Hand1/Laserpointer").GetComponentInChildren<LaserPointer>();
        laserRight = GameObject.Find("Player/SteamVRObjects/Hand2/Laserpointer").GetComponentInChildren<LaserPointer>();

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

        //Load the old data from the file, so it won't get replaced
        LoadOldSavedData();

        //laserLeft.PointerOut += StopRecord;
        //laserRight.PointerOut += StopRecord;

        laserLeft.PointerIn += FindTarget;
        laserRight.PointerIn += FindTarget;
    }


    //TODO: 
    //-pistä commentWheel oikeaan kohtaan (pienenä)
    //-indikaatio tallenuksen aloituksesta ja lopetuksesta (ja epäonnistumisesta?)
    void FindTarget(object sender, LaserEventArgs e)
    {
        if(e.target.gameObject.layer != LayerMask.NameToLayer(commentLayer))
        {
            target = e.target.gameObject;
        }
    }

    public void StartRecord()
    {
        if (micConnected)
        {
            DisableVoiceChat();

            if (!Microphone.IsRecording(null))
            {
                tempAudioClip = Microphone.Start(null, true, 30, maxFreq);
                Debug.Log("Recording started");
            }
        }
        else
        {
            Debug.LogError("Microphone not connected");
        }

    }

    public void StopRecord()
    {
            if (micConnected)
            {
                DisableVoiceChat();

                if (Microphone.IsRecording(null))
                {
                    int lastPos = Microphone.GetPosition(null);
                    if (lastPos != 0)
                    {
                        float[] samples = new float[tempAudioClip.samples];
                        tempAudioClip.GetData(samples, 0);

                        float[] finalSamples = new float[lastPos];

                        for (int i = 0; i < finalSamples.Length; i++)
                        {
                            finalSamples[i] = samples[i];
                        }

                        finalAudioClip = AudioClip.Create("FinalAudioClip", finalSamples.Length, 1, maxFreq, false);

                        finalAudioClip.SetData(finalSamples, 0);

                        Microphone.End(null);
                        Debug.Log("Recording stopped");
                        SaveRecordedAudio();
                        //Enable voice chat again
                        voiceTrigger.Mode = Dissonance.CommActivationMode.VoiceActivation;
                    }
                }
            }
    }

    //void StopRecord(object sender, LaserEventArgs e)
    //{
    //    if (e.target.gameObject.layer == LayerMask.NameToLayer(commentLayer) && e.target.name == "VoiceComment")
    //    {
    //        if (micConnected)
    //        {
    //            DisableVoiceChat();

    //            if (Microphone.IsRecording(null))
    //            {
    //                int lastPos = Microphone.GetPosition(null);
    //                if (lastPos != 0)
    //                {
    //                    float[] samples = new float[tempAudioClip.samples];
    //                    tempAudioClip.GetData(samples, 0);

    //                    float[] finalSamples = new float[lastPos];

    //                    for (int i = 0; i < finalSamples.Length; i++)
    //                    {
    //                        finalSamples[i] = samples[i];
    //                    }

    //                    finalAudioClip = AudioClip.Create("FinalAudioClip", finalSamples.Length, 1, maxFreq, false);

    //                    finalAudioClip.SetData(finalSamples, 0);

    //                    Microphone.End(null);
    //                    Debug.Log("Recording stopped");
    //                    SaveRecordedAudio();
    //                    //Enable voice chat again
    //                    voiceTrigger.Mode = Dissonance.CommActivationMode.VoiceActivation;
    //                }
    //            }
    //        }
    //    }
    //}

    void DisableVoiceChat()
    {
        if(voiceTrigger.Mode != Dissonance.CommActivationMode.None)
        {
            voiceTrigger.Mode = Dissonance.CommActivationMode.None;
            Microphone.End(null);
        }
    }
//-----------------------------------------------------------------------------------------------------------------------------------------------------

    void LoadOldSavedData()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(PositionDatabase));
        FileStream file = File.Open(SavePath + slash + positionFileName, FileMode.Open);
        Debug.Log("File lenght = " + file.Length);
        if (file.Length > 0)
        {
            positionDB = (PositionDatabase)serializer.Deserialize(file);
            file.Close();
        }       
    }


void SaveRecordedAudio()
    {
        string filename = commenter + "_VoiceComment_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") ;

        SavWav.Save(filename, finalAudioClip, savePath + audioSavePathExt);

        XmlSerializer serializer = new XmlSerializer(typeof(PositionDatabase));
        FileStream file = File.Create(savePath + positionFileName);

        positionDB.list.Add(new PositionData());

        positionDB.list[positionDB.list.Count - 1].commenterName = commenter;
        positionDB.list[positionDB.list.Count - 1].recordName = filename;
        positionDB.list[positionDB.list.Count - 1].targetName = target.name;
           
        positionDB.list[positionDB.list.Count - 1].position[0] = target.transform.position.x;
        positionDB.list[positionDB.list.Count - 1].position[1] = target.transform.position.y;
        positionDB.list[positionDB.list.Count - 1].position[2] = target.transform.position.z;
        
        serializer.Serialize(file, positionDB);
        file.Close();

    }
}

[System.Serializable]
public class PositionData
{
    public string commenterName;
    public string recordName;
    public string targetName;
    public float[] position = new float[3];
}

[System.Serializable]
public class PositionDatabase
{
    public List<PositionData> list = new List<PositionData>();
}