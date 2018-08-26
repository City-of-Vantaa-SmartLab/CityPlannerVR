using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;
using System;


[Serializable]
public class FileInfoContainer
{
    public string filename;
    public string fullFilePath;
    public SyncFiles.Filetype fileType;
    public bool useFullPath;
}

/// <summary>
/// Syncfiles script is intended for asynchronous file accessing with database. It also adds FileInfoContainers
/// (via GenerateFileInfoContainer method) into SaveData to ease tracking of files that will be saved.
/// Wanted filetypes can be downloaded from the database using method SyncFromDatabaseAsync,
/// which will use the given filetype to distribute downloaded files to their respective folders
/// </summary>

public class SyncFiles : MonoBehaviour
{
    public enum Filetype { defaultType, image, voice };

    //public enum ToolType { Empty, Camera, RemoteGrabber, EditingLaser, Eraser, Painter, PathCamera, VideoCamera, Item, Button };  //includes modes for tools
    //public Filetype File { get; set; }

    private static char slash = Path.DirectorySeparatorChar;
    public float SyncToDatabaseInterval;
    public float SyncFromDatabaseInterval;  //Use a negative value to disable syncing


    private void Start()
    {
        if (SyncToDatabaseInterval == 0)
            SyncToDatabaseInterval = 5f;
        if (SyncToDatabaseInterval > 0)
            InvokeRepeating("StartSyncingToDatabaseAsync", 2f, SyncToDatabaseInterval);

        if (SyncFromDatabaseInterval == 0)
            SyncFromDatabaseInterval = 5f;
        if (SyncFromDatabaseInterval > 0)
            InvokeRepeating("StartSyncingFromDatabaseAsync", 2f, SyncFromDatabaseInterval);
    }

    private void StartSyncingToDatabaseAsync()
    {
        //Debug.Log("Sync to database called...");
        Task syncing = new Task(() => SyncToDatabase());
        syncing.Start();
        //SyncToDatabase();
    }

    private void StartSyncingFromDatabaseAsync()
    {
        //Debug.Log("Sync from database called...");
        //Task syncing = new Task(() => SyncToDatabase());
        //syncing.Start();
        SyncFromDatabaseAsync(Filetype.image);
        SyncFromDatabaseAsync(Filetype.voice);
        SyncFromDatabaseAsync(Filetype.defaultType);
    }


    private static void SyncToDatabase()
    {
        //Debug.Log("Why is this not called");
        if (SaveData.binarySyncing == true)
        {
            Debug.Log("Binary files are already syncing to database!");
            return;
        }

        //Debug.Log("Starting to sync files...");
        SaveData.binarySyncing = true;
        foreach (FileInfoContainer info in SaveData.syncFileContainer.datas)
        {
            try
            {
                if (!MongoDBAPI.UseDefaultConnections())
                {
                    Debug.Log("Skipping to next file");
                    continue;
                }
                if (info.fileType == Filetype.image)
                    MongoDBAPI.ImportBinaryFileToDatabase(MongoDBAPI.imageCollection, info);
                else if (info.fileType == Filetype.voice)
                    MongoDBAPI.ImportBinaryFileToDatabase(MongoDBAPI.voiceCollection, info);
                else
                    MongoDBAPI.ImportBinaryFileToDatabase(MongoDBAPI.defaultFileCollection, info);
            }
            catch (Exception ex)
            {
                Debug.Log("Error uploading file to database, returning fileinfo to temp array for next batch");
                Debug.Log("Exception error: " + ex);
                SaveData.AddData(info);  //The info will be redirected to SaveData.syncFileContainerTemp while SaveData.binarySyncing is true
            }


        }

        SaveData.ClearContainer(SaveData.syncFileContainer);
        SaveData.binarySyncing = false;
        foreach(FileInfoContainer info in SaveData.syncFileContainerTemp.datas)
            SaveData.AddData(info);
        SaveData.ClearContainer(SaveData.syncFileContainerTemp);
    }

    /// <summary>
    /// Starts syncing the files of given filetype from database
    /// </summary>
    
    private void SyncFromDatabaseAsync(Filetype filetype)
    {
        if (!MongoDBAPI.UseDefaultConnections())
            return;
        if (filetype == Filetype.image)
        {
            Task taskImages = new Task(() => MongoDBAPI.ExportBinaryFileFromDatabase(MongoDBAPI.imageCollection, MongoDBAPI.imageFileFolder));
            taskImages.Start();
        }
        else if (filetype == Filetype.voice)
        {
            Task taskVoices = new Task(() => MongoDBAPI.ExportBinaryFileFromDatabase(MongoDBAPI.voiceCollection, MongoDBAPI.voiceFileFolder));
            taskVoices.Start();
        }
        else
        {
            Task taskDefault = new Task(() => MongoDBAPI.ExportBinaryFileFromDatabase(MongoDBAPI.defaultFileCollection , MongoDBAPI.defaultFileFolder));
            taskDefault.Start();
        }

        //Task saveTask = new Task(() => SaveToDatabase());
        //saveTask.Start();
    }

    /// <summary>
    /// Generates and adds fileinfo to savedata containers. The containers should be automatically synced by method StartSyncingToDatabase.
    /// Filename and filetype are the most important, you can disregard fullfilepath if useFullPath is false
    /// </summary>

    public static void GenerateFileInfoContainer(string fileName, string fullFilePath, bool useFullPath, Filetype fileType)
    {
        Debug.Log("Generating file info...");
        FileInfoContainer temp = new FileInfoContainer();
        temp.useFullPath = useFullPath;
        temp.fullFilePath = fullFilePath;
        temp.fileType = fileType;
        if (string.IsNullOrEmpty(fileName))
            temp.filename = ExtractFilenameOnly(fullFilePath);
        else
            temp.filename = fileName;
        SaveData.AddData(temp);
        Debug.Log("File info added for saving");
    }

    public static string ExtractFilenameOnly(string fullFilePath)
    {
        int slashPosition = -1;
        Debug.Log("Extracting filename from: " + fullFilePath);
        for (int i = 0; i < fullFilePath.Length; i++)
        {
            if (fullFilePath[i] == slash)
                slashPosition = i;
        }

        if (slashPosition < 0)
            return "";

        string filename = fullFilePath.Substring(slashPosition + 1);
        return filename;
    }

}
