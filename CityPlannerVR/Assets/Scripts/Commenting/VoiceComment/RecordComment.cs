using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

using System.Xml;
using System.Xml.Serialization;

/// <summary>
/// Records and saves an audio 
/// </summary>

//Made using tutorial from http://www.41post.com/4884/programming/unity-capturing-audio-from-a-microphone

public class RecordComment : MonoBehaviour
{
    /// <summary>
    /// Tells if there are no microphones connected
    /// </summary>
    private bool micConnected = false;

    /// <summary>
    /// The minimum Frequency got from the microphone
    /// </summary>
    private int minFreq;

    /// <summary>
    /// The maximum Frequency got from the microphone
    /// </summary>
    private int maxFreq;

    /// <summary>
    /// The recorded audio is saved into this variable for trimming (the audio is always 30 sek long before trimming)
    /// </summary>
    private AudioClip tempAudioClip;
    /// <summary>
    /// Final version of audio after trimming
    /// </summary>
    private AudioClip finalAudioClip;

    [HideInInspector]
    public Dissonance.VoiceBroadcastTrigger voiceTrigger;

    /// <summary>
    /// The person who commented
    /// </summary>
    [HideInInspector]
    public string commenter;

    const string directoryName = "VoiceComments";
    /// <summary>
    /// / or \ depending on the used operating system
    /// </summary>
    [HideInInspector]
    public static char slash = Path.DirectorySeparatorChar;
    [HideInInspector]
    public static string positionFileName = "positions.txt";

    /// <summary>
    /// The position where the target object was during recording
    /// </summary>
    [HideInInspector]
    public PositionData position;
    /// <summary>
    /// Storage for an instance of a PositionDatabase class which is used to record information on text file
    /// </summary>
    [HideInInspector]
    public PositionDatabase positionDB;

    /// <summary>
    /// AudioSource for the indicator that recording started and stoped
    /// </summary>
    AudioSource source;

    /// <summary>
    /// The path where the text file and the audio data folder are located
    /// </summary>
    private static string savePath;
    /// <summary>
    /// Add after savePath to get the location of all the audio files
    /// </summary>
    private static string audioSavePathExt;

    /// <summary>
    /// Shows player how long the voice comment has been recorded
    /// </summary>
    public UnityEngine.UI.Text recordTimer;
    int recordTimeMilliSec;
    int recordTimeSec;

    /// <summary>
    /// The canvas which shows the recordTimer
    /// </summary>
    //----GameObject canvas;
    /// <summary>
    /// Reference to coroutine which counts the recordTimer up when comment is recorded
    /// </summary>
    Coroutine RecordTimerCoroutine;

    /// <summary>
    /// The path where the text file and the audio data folder are located (read only)
    /// </summary>
    public static string SavePath
    {
        get
        {
            return savePath;
        }
    }

    /// <summary>
    /// Add after savePath to get the location of all the audio files (read only)
    /// </summary>
    public static string AudioExt
    {
        get
        {
            return audioSavePathExt;
        }
    }
    //---------------------------------------------------------------------------------------------

    private void Start()
    {
        //savePath = Application.dataPath+ slash + "Resources" + slash + "Comments" + slash + directoryName + slash;
        savePath = Application.streamingAssetsPath + slash + "Comments" + slash + directoryName + slash;
        audioSavePathExt = "AudioFiles" + slash;

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

        source = GetComponent<AudioSource>();

        LoadOldSavedData();

        voiceTrigger = PhotonPlayerAvatar.LocalPlayerInstance.GetComponent<Dissonance.VoiceBroadcastTrigger>();
        commenter = PhotonPlayerAvatar.LocalPlayerInstance.GetComponent<PhotonView>().owner.NickName;

        //----recordTimer = GetComponentInChildren<UnityEngine.UI.Text>();
        //----canvas = GameObject.Find("RecordIndicatorCanvas");
        //----canvas.SetActive(false);
    }

    /// <summary>
    /// Used to play the sound effect when recording is started or stopped
    /// </summary>
    private void PlaySoundEffect()
    {
        source.Play();
    }

    /// <summary>
    /// Start recording the voice comment
    /// </summary>
    public void StartRecord()
    {
        //If there is no microphone connected, we can't record anything
        if (micConnected)
        {
            DisableVoiceChat();
            PlaySoundEffect();

            if (!Microphone.IsRecording(null))
            {
                tempAudioClip = Microphone.Start(null, true, 30, maxFreq);
                //Debug.Log("Recording started");
                //-----canvas.SetActive(true);
                RecordTimerCoroutine = StartCoroutine(RecordTimer());
            }
        }
        else
        {
            Debug.LogError("Microphone not connected");
        }
    }

    /// <summary>
    /// Runs the timer of how long is the recording at the moment
    /// </summary>
    IEnumerator RecordTimer()
    {
        //Debug.Log("Timer started");

        recordTimeMilliSec = 0;
        recordTimeSec = 0;

        while (true)
        {
            yield return new WaitForSeconds(0.01f);
            if (recordTimeMilliSec >= 99)
            {
                recordTimeSec += 1;
                recordTimeMilliSec = 0;
            }
            else
            {
                recordTimeMilliSec += 1;
            }
            recordTimer.text = string.Format("00:{0:00}:{1:00}", recordTimeSec, recordTimeMilliSec);
        }
    }

    /// <summary>
    /// Stops the recording of the voice comment
    /// </summary>
    public void StopRecord()
    {
        //If there is no microphone connected we can't even start the commenting so there is no need to try to stop it
        if (micConnected)
        {
            DisableVoiceChat();

            if (Microphone.IsRecording(null))
            {
                PlaySoundEffect();

                //Basicly how long did the player actually speak in the audio
                int lastPos = Microphone.GetPosition(null);
                if (lastPos != 0)
                {
                    //Get the samples of the temporary audioClip recorded and put the m to array
                    float[] samples = new float[tempAudioClip.samples];
                    tempAudioClip.GetData(samples, 0);

                    float[] finalSamples = new float[lastPos];

                    //Trim the audio
                    for (int i = 0; i < finalSamples.Length; i++)
                    {
                        finalSamples[i] = samples[i];
                    }
                    finalSamples = MonitorCommentVolume.CalculateAmplitudeMultiplier(finalSamples);
                    finalAudioClip = AudioClip.Create("FinalAudioClip", finalSamples.Length, 1, maxFreq, false);

                    finalAudioClip.SetData(finalSamples, 0);

                    Microphone.End(null);
                    //Debug.Log("Recording stopped");
                    StopCoroutine(RecordTimerCoroutine);
                    //----canvas.SetActive(false);

                    //Reset the timer
                    recordTimeSec = 0;
                    recordTimeMilliSec = 0;
                    recordTimer.text = string.Format("00:{0:00}:{1:00}", recordTimeSec, recordTimeMilliSec);

                    SaveRecordedAudio();
                    //Enable voice chat again
                    voiceTrigger.Mode = Dissonance.CommActivationMode.VoiceActivation;
                }
            }
        }
    }
    /// <summary>
    /// We don't want others to hear what we are saying when commenting
    /// </summary>
    void DisableVoiceChat()
    {
        if (voiceTrigger.Mode != Dissonance.CommActivationMode.None)
        {
            voiceTrigger.Mode = Dissonance.CommActivationMode.None;
            Microphone.End(null);
        }
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Load the old data from the file, so it won't get replaced
    /// </summary>
    void LoadOldSavedData()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(PositionDatabase));
        if (!File.Exists(SavePath + positionFileName))
        {
            File.Create(SavePath + positionFileName);
        }
        FileStream file = File.Open(SavePath + positionFileName, FileMode.Open);

        if (file.Length > 0)
        {
            positionDB = (PositionDatabase)serializer.Deserialize(file);
        }

        file.Close();
    }

    /// <summary>
    /// Save the voice comment as a wav file to savePath location
    /// </summary>
    void SaveRecordedAudio()
    {
        string filename = commenter + "_VoiceComment_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

        SavWav.Save(filename, finalAudioClip, savePath + audioSavePathExt);

        XmlSerializer serializer = new XmlSerializer(typeof(PositionDatabase));
        
        
        FileStream file = File.Create(savePath + positionFileName); 

        positionDB.list.Add(new PositionData());

        positionDB.list[positionDB.list.Count - 1].commenterName = commenter;
        positionDB.list[positionDB.list.Count - 1].recordName = filename;
        if(HoverTabletManager.CommentTarget == null)
        {
            positionDB.list[positionDB.list.Count - 1].targetName = null;
        }
        else
        {
            positionDB.list[positionDB.list.Count - 1].targetName = HoverTabletManager.CommentTarget.name;
        }
        positionDB.list[positionDB.list.Count - 1].position[0] = HoverTabletManager.CommentTarget.transform.position.x;
        positionDB.list[positionDB.list.Count - 1].position[1] = HoverTabletManager.CommentTarget.transform.position.y;
        positionDB.list[positionDB.list.Count - 1].position[2] = HoverTabletManager.CommentTarget.transform.position.z;

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