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

    private Dissonance.VoiceBroadcastTrigger voiceTrigger;

    //The object we are commenting
    GameObject target;
    //The person who commented
    string commenter;

    LaserPointer laser;

    string commentLayer = "CommentTool";

    string directoryName = "VoiceComments";
    char slash = Path.DirectorySeparatorChar;

    DirectoryInfo files;
    FileInfo[] fileInfo;

    [HideInInspector]
    public PositionData position;

    public PositionDatabase positionDB;

    private string savePath;

    public string SavePath
    {
        get
        {
            return savePath;
        }
    }

    private void Start()
    {

        laser = GameObject.Find("Player").GetComponentInChildren<LaserPointer>();

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

        commenter = gameObject.GetComponent<PhotonView>().owner.NickName;

        voiceTrigger = gameObject.GetComponent<Dissonance.VoiceBroadcastTrigger>();

        laser.PointerIn += StartRecord;
        laser.PointerOut += StopRecord;

        laser.PointerIn += FindTarget;
#if UNITY_EDITOR
        savePath = "C:"+ slash + "Users" + slash + "SmartLabVantaa" + slash + "Desktop" + slash + "Projects" + slash + "CityPlannerVR" + slash + "CityPlannerVR" + slash + "Assets" + slash + "Resources" + slash + "Comments";
#endif
        //TODO: Buildissa on eri polku ehkä
    }


    //TODO: 
    //-avaa commentWheel (kun osotetaan taloa)
    //-pistä commentWheel oikeaan kohtaan (isona ja pienenä)
    //-piilota commentWheel
    //-estä kommentoimasta vääriä asioita
    //-indikaatio tallenuksen aloituksesta ja lopetuksesta (ja epäonnistumisesta?)
    void FindTarget(object sender, LaserEventArgs e)
    {
        if(e.target.gameObject.layer != LayerMask.NameToLayer(commentLayer))
        {
            target = e.target.gameObject;
        }
    }

    void StartRecord(object sender, LaserEventArgs e)
    {
        target = e.target.gameObject;
        if(e.target.gameObject.layer == LayerMask.NameToLayer(commentLayer) && e.target.name == "VoiceComment")
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
    }

    void StopRecord(object sender, LaserEventArgs e)
    {
        if (e.target.gameObject.layer == LayerMask.NameToLayer(commentLayer) && e.target.name == "VoiceComment")
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

void SaveRecordedAudio()
    {
        string filename = commenter + "_VoiceComment_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") ;

        SavWav.Save(filename, finalAudioClip, savePath + slash + directoryName + slash);

        XmlSerializer serializer = new XmlSerializer(typeof(PositionDatabase));
        FileStream file = File.Create(savePath + slash + directoryName + slash + "positions.txt");

        for (int i = 0; i < ObjectContainer.objects.Count; i++)
        {
            positionDB.list.Add(new PositionData());

            positionDB.list[i].commenterName = commenter;
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