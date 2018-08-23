using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Takes a picture of the scene
/// </summary>
/// 
// (this code was made with the help of this link
// https://answers.unity.com/questions/22954/how-to-save-a-picture-take-screenshot-from-a-camer.html)

public class ScreenshotCamera : MonoBehaviour {

    [Tooltip("Resolution width of the picture")]
	public int resWidth = 2048;
    [Tooltip("Resolution Height of the picture")]
	public int resHeight = 1024;

    /// <summary>
    /// Screenshot camera objects camera component
    /// </summary>
	Camera ssCamera;
    /// <summary>
    /// RenderTexture used to show to player what the camera sees
    /// </summary>
    RenderTexture rt;
    AudioSource clickSound;

    InputMaster inputMaster;

    public GameObject cameraScreen;
    public GameObject cameraBackcscreen;
    public Material cameraScreenMaterial;

    //This value is got from the cameraHandler that activates this object
    public int myHandNumber;
    /// <summary>
    /// Index for the camera point changing
    /// </summary>
    int index = 0;
    /// <summary>
    /// All the fixed points where the screenshot camera can be (first 2 are in players hands)
    /// </summary>
    public GameObject[] points;

    public LoadPhotos loadPhotos;
    /// <summary>
    /// Makes and returns the name which is given to the screenshot when it's saved
    /// </summary>
    /// <param name="width">Width of the picture in pixels</param>
    /// <param name="height">Height of the picture in pixels</param>
    /// <returns></returns>
    public static string ScreenshotName(int width, int height){

        string folder = "Screenshots";
        string fileExtender = ".png";
        string fileName = "screen_" + width + "x" + height + "_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + fileExtender;
        string pathName = Application.streamingAssetsPath;
        // '/' or '\' depending on the operating system
        char slash = System.IO.Path.DirectorySeparatorChar;

        string folderPathName = pathName + slash + folder;

        //If there is no such directory, create one
        if (!System.IO.Directory.Exists(folderPathName))
        {
            System.IO.Directory.CreateDirectory(folderPathName);
        }

		return folderPathName + slash + fileName;
	}

	void Awake(){
        ssCamera = GetComponent<Camera> ();

        rt = new RenderTexture(resWidth, resHeight, 24);
        ssCamera.targetTexture = rt;
        ssCamera.fieldOfView = 60;

        clickSound = GetComponent<AudioSource>();

        cameraScreenMaterial.mainTexture = rt;
        cameraScreen.GetComponent<MeshRenderer>().material = cameraScreenMaterial;

        cameraScreen.SetActive(false);
        cameraBackcscreen.SetActive(false);

        inputMaster = GameObject.Find("Player").GetComponent<InputMaster>();
    }

    private void OnEnable()
    {
        //                            myHandNumber is 1 or 2, but the place for them in the array are 0 and 1
        gameObject.transform.parent.parent.parent = points[myHandNumber - 1].transform;  //changes grandcameraholder's parent
        gameObject.transform.parent.parent.localPosition = Vector3.zero;
        gameObject.transform.parent.parent.localRotation = Quaternion.identity;

        cameraScreen.transform.parent.parent = points[myHandNumber - 1].transform;
        cameraScreen.transform.parent.localRotation = Quaternion.identity;

        ////Left hand
        //if(myHandNumber == 1)
        //{
        //    cameraScreen.transform.localPosition = new Vector3(0.15f, 0, 0);
        //}
        ////Right hand
        //else
        //{
        //    cameraScreen.transform.localPosition = new Vector3(-0.15f, 0, 0);
        //}

        cameraScreen.SetActive(true);
        cameraBackcscreen.SetActive(true);

        Subscribe();
    }

    private void OnDisable()
    {
        Unsubscribe();

        cameraScreen.SetActive(false);
        cameraBackcscreen.SetActive(false);
    }

    private void Subscribe()
    {
        if (inputMaster)
        {
            inputMaster.TriggerClicked += TakeScreenshot;

            inputMaster.PadClicked += ChangePoint;

        }
        else
        {
            Debug.LogError("Did not find inputmaster!");
        }
    }

    private void Unsubscribe()
    {
        if (inputMaster)
        {
            inputMaster.TriggerClicked -= TakeScreenshot;

            inputMaster.PadClicked -= ChangePoint;
        }
        else
        {
            Debug.LogError("Did not find inputmaster!");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender">Which controller sent info</param>
    /// <param name="e">The info the controller sent</param>
    void TakeScreenshot(object sender, ClickedEventArgs e)
    {
        if(e.controllerIndex == myHandNumber)
        {
            Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
            ssCamera.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            ssCamera.targetTexture = null;
            RenderTexture.active = null;
            rt.Release();
            byte[] bytes = screenShot.EncodeToPNG();
            string filename = ScreenshotName(resWidth, resHeight);
            System.IO.File.WriteAllBytes(filename, bytes);
            Debug.Log("Took screenshot to: " + filename);
            clickSound.Play();

            //New texture for the next picture
            rt = new RenderTexture(resWidth, resHeight, 24);
            ssCamera.targetTexture = rt;
            cameraScreenMaterial.mainTexture = rt;
            cameraScreen.GetComponent<MeshRenderer>().material = cameraScreenMaterial;

            loadPhotos.Load();
        }
        
    }
    //---------------------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Change the point where the camera is
    /// </summary>
    /// <param name="sender">Which controller sent info</param>
    /// <param name="e">The info the controller sent</param>
    void ChangePoint(object sender, ClickedEventArgs e) {

        if (e.controllerIndex == myHandNumber)
        {
            if (e.padX > 0.7f)
            {
                ChangePointRight();
            }
            else if (e.padX < -0.7f)
            {
                ChangePointLeft();
            }
        }
    }

    /// <summary>
    /// Change the point where the camera is to right
    /// </summary>
    void ChangePointRight()
    {
        if(index >= points.Length - 1)
        {
            //We have looped around
            index = 0;
        }
        else
        {
            index++;
        }

        gameObject.transform.parent.parent = points[index].transform;
        gameObject.transform.parent.localPosition = Vector3.zero;
        gameObject.transform.parent.localRotation = Quaternion.identity;
    }

    /// <summary>
    /// Change the point where the camera is to left
    /// </summary>
    void ChangePointLeft()
    {
        if (index <= 0)
        {
            //We have looped around
            index = points.Length - 1;
        }
        else
        {
            index--;
        }

        gameObject.transform.parent.parent = points[index].transform;
        gameObject.transform.parent.localPosition = Vector3.zero;
        gameObject.transform.parent.localRotation = Quaternion.identity;
    }
}
