using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LoadPhotos : MonoBehaviour {

    public GameObject PhotoObject;
    public GameObject PhotoObjectParent;

    //List<Sprite> photos;

    DirectoryInfo info;
    FileInfo[] fileInfo;

    int photoIndex = 0;

    string photoPath;
    char slash = Path.DirectorySeparatorChar;

    

    void Start()
    {
        photoPath = Application.streamingAssetsPath + slash + "Screenshots" + slash;
        info = new DirectoryInfo(photoPath);

        Load();
    }

    public void Load()
    {
        StartCoroutine(LoadPhotosFromStreamingAssets(photoPath));
    }

    IEnumerator LoadPhotosFromStreamingAssets(string path)
    {
        int index = 0;
        WWW request = null;
        Texture texture;
        Sprite sprite;

        fileInfo = null;
        fileInfo = info.GetFiles();

        for (int i = photoIndex; i < fileInfo.Length; i++)
        {
            request = new WWW("file:///" + path + fileInfo[i].Name);
            if (fileInfo[i].Name.EndsWith(".png"))
            {
                Debug.Log("index = " + index);
                yield return request;
                
                texture = request.texture;
                sprite = Sprite.Create(texture as Texture2D, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

                GameObject photoObject = Instantiate(PhotoObject);
                photoObject.transform.parent = PhotoObjectParent.transform;
                photoObject.transform.localPosition = Vector3.zero;
                photoObject.transform.localRotation = Quaternion.identity;
                photoObject.transform.localScale = Vector3.one;
                photoObject.GetComponent<UnityEngine.UI.Image>().sprite = sprite;

                //photos.Add(sprite);
                //photos[index].name = fileInfo[i].Name;

                index++;
            }
            photoIndex++;
        }
    }
}
