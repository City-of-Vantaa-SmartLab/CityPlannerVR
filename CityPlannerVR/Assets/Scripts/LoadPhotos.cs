using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LoadPhotos : MonoBehaviour {

    List<Texture> photos;

    DirectoryInfo info;
    FileInfo[] fileInfo;

    string photoPath;
    char slash = Path.DirectorySeparatorChar;

    void Start()
    {
        photoPath = Application.streamingAssetsPath + slash + "Screenshots";
        info = new DirectoryInfo(RecordComment.SavePath + RecordComment.AudioExt);
        fileInfo = info.GetFiles();
    }

    IEnumerator LoadCommentsFromStreamingAssets(string path)
    {
        int index = 0;
        WWW request = null;
        
        for (int i = 0; i < fileInfo.Length; i++)
        {
            request = new WWW("file:///" + path);
            if (fileInfo[i].Name.EndsWith(".png"))
            {
                while (!request.isDone)
                {
                    yield return null;
                }
                photos.Add(request.texture);
                photos[index].name = fileInfo[i].Name;

                index++;
            }
        }
        yield return request;

    }
}
