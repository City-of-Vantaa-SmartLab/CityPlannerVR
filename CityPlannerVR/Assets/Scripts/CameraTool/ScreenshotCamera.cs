using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Takes a picture of the scene
/// (this code was made with the help of this link
/// https://answers.unity.com/questions/22954/how-to-save-a-picture-take-screenshot-from-a-camer.html)
/// </summary>

public class ScreenshotCamera : MonoBehaviour {

	public int resWidth = 2048;
	public int resHeight = 1024;

	Camera ssCamera;
    RenderTexture rt;
    AudioSource clickSound;

    InputMaster inputMaster;

    Valve.VR.InteractionSystem.Teleport teleport;

    public GameObject cameraScreen;
    public Material cameraScreenMaterial;

    //This value is got from the cameraHandler that activates this object
    public int myHandNumber;

    int index = 0;
    //All the fixed points where the screenshot camera can be (first 2 are in players hands)
    public GameObject[] points;


    public static string ScreenshotName(int width, int height){

		return string.Format ("{0}/screenshots/screen_{1}x{2}_{3}.png", Application.dataPath, width, height, System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
	}

	void Awake(){
        ssCamera = GetComponent<Camera> ();

        rt = new RenderTexture(resWidth, resHeight, 24);
        ssCamera.targetTexture = rt;
        ssCamera.fieldOfView = 60;

        clickSound = GetComponent<AudioSource>();

        teleport = GameObject.Find("Teleporting").GetComponent<Valve.VR.InteractionSystem.Teleport>();

        cameraScreenMaterial.mainTexture = rt;
        cameraScreen.GetComponent<MeshRenderer>().material = cameraScreenMaterial;

        cameraScreen.SetActive(false);

        inputMaster = GameObject.Find("Player").GetComponent<InputMaster>();
    }

    private void OnEnable()
    {
        gameObject.transform.parent = points[myHandNumber - 1].transform;
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localRotation = Quaternion.identity;

        cameraScreen.transform.parent = points[myHandNumber - 1].transform;
        cameraScreen.transform.localRotation = Quaternion.identity;

        //Left hand
        if(myHandNumber == 1)
        {
            cameraScreen.transform.localPosition = new Vector3(0.15f, 0, 0);
        }
        else
        {
            cameraScreen.transform.localPosition = new Vector3(-0.15f, 0, 0);
        }
        
        cameraScreen.SetActive(true);

        Subscribe();
    }

    private void OnDisable()
    {
        cameraScreen.SetActive(false);

        Unsubscribe();
    }

    private void Subscribe()
    {
        if (inputMaster)
        {
            teleport.disableTeleport = true;

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
            teleport.disableTeleport = false;

            inputMaster.TriggerClicked -= TakeScreenshot;

            inputMaster.PadClicked -= ChangePoint;

        }
        else
        {
            Debug.LogError("Did not find inputmaster!");
        }
    }

    void TakeScreenshot(object sender, ClickedEventArgs e)
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
        
    }
    //---------------------------------------------------------------------------------------------------------------------------------------------------------


    void ChangePoint(object sender, ClickedEventArgs e) {
        if(e.padX > 0.7f)
        {
            ChangePointRight();
        }
        else if(e.padX < -0.7f)
        {
            ChangePointLeft();
        }
    }


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

        Debug.Log("index is " + index);

        gameObject.transform.parent = points[index].transform;
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localRotation = Quaternion.identity;
    }

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

        Debug.Log("index is " + index);

        gameObject.transform.parent = points[index].transform;
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localRotation = Quaternion.identity;
    }

}
