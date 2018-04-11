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

    InputListener inputListener;

    public GameObject quad;
    public Material quadMaterial;

    public int myHandNumber;
    private uint myDeviceIndex;

    public static string ScreenshotName(int width, int height){

		return string.Format ("{0}/screenshots/screen_{1}x{2}_{3}.png", Application.dataPath, width, height, System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
	}

	void Awake(){
        ssCamera = GetComponent<Camera> ();
        rt = new RenderTexture(resWidth, resHeight, 24);
        ssCamera.targetTexture = rt;
        ssCamera.fieldOfView = 60;

        clickSound = GetComponent<AudioSource>();

        quadMaterial.mainTexture = rt;
        quad.GetComponent<MeshRenderer>().material = quadMaterial;

        //subscribaa inputManageriin ja poista lateUpdate
        inputListener = GameObject.Find("Player").GetComponent<InputListener>();
        Subscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    private void Subscribe()
    {
        if (inputListener)
        {
            inputListener.TriggerClicked += TakeScreenshot;
            inputListener.TriggerLifted += TakeScreenshot;
            if (myHandNumber == 1)
                inputListener.Hand1DeviceFound += HandleMyIndexFound;
            if (myHandNumber == 2)
                inputListener.Hand2DeviceFound += HandleMyIndexFound;
        }
        else
        {
            Debug.LogError("Did not find inputlistener!");
        }
    }

    private void Unsubscribe()
    {
        if (inputListener)
        {
            inputListener.TriggerClicked -= TakeScreenshot;
            inputListener.TriggerLifted -= TakeScreenshot;
            if (myHandNumber == 1)
                inputListener.Hand1DeviceFound -= HandleMyIndexFound;
            if (myHandNumber == 2)
                inputListener.Hand2DeviceFound -= HandleMyIndexFound;
        }
        else
        {
            Debug.LogError("Did not find inputlistener!");
        }
    }

    private void HandleMyIndexFound(uint deviceIndex)
    {
        myDeviceIndex = deviceIndex;
        if (myHandNumber == 1)
            inputListener.Hand1DeviceFound -= HandleMyIndexFound;
        if (myHandNumber == 2)
            inputListener.Hand2DeviceFound -= HandleMyIndexFound;
    }

    void TakeScreenshot(object sender, ClickedEventArgs e)
    {
        if (gameObject.GetActive() == true)
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

            rt = new RenderTexture(resWidth, resHeight, 24);
            ssCamera.targetTexture = rt;
            quadMaterial.mainTexture = rt;
            quad.GetComponent<MeshRenderer>().material = quadMaterial;
        }
    }
}
