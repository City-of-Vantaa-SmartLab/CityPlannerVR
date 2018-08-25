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

    private string folderPathName;
    private string folder;
    private string fileName;
    private string fileExtender;
    private string pathName;
    private string streamingAssets = Application.streamingAssetsPath;
    private char slash = Path.DirectorySeparatorChar;

    private void StartSyncingToDatabaseAsync()
    {
        Task syncing = new Task(() => SyncToDatabase());
        syncing.Start();
    }


    private void SyncToDatabase()
    {
        if (SaveData.binarySyncing == true)
        {
            Debug.Log("Binary files are already syncing to database!");
            return;
        }

        SaveData.binarySyncing = true;
        foreach (FileInfoContainer info in SaveData.syncFileContainer.datas)
        {
            try
            {
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

    public void GenerateFileInfoContainer(string fileName, string fullFilePath, bool useFullPath, Filetype fileType)
    {
        FileInfoContainer temp = new FileInfoContainer();
        temp.useFullPath = useFullPath;
        temp.fullFilePath = fullFilePath;
        temp.filename = fileName;
        SaveData.AddData(temp);
    }

}
