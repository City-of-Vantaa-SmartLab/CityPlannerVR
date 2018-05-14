using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialProgression : MonoBehaviour
{

    public List<string> tutorialtexts = new List<string>();
    public int textlistsize;
    int text_int = 0;
    public GameObject textfield;
    public TextMesh textfieldtext;
    public SteamVR_GazeTracker gaze;
    public float timer = 0f;
    //public float gazeTime = 2f;
    public Light textLight;
    private bool lightOn = false;
    private bool coroutineExecute = false;

    // Use this for initialization
    void Start()
    {
        Texts();
        textlistsize = tutorialtexts.Count;
        textfield = GameObject.Find("Aloitus");
        gaze = gameObject.GetComponent<SteamVR_GazeTracker>();
        textfieldtext = textfield.GetComponent<TextMesh>();
    }

    void Update()
    {
        if (gaze.isInGaze && text_int == 0)
        {
            NextText();

        }
    }


    public void NextText()
    {
        if (text_int < 6)
        {
            if (lightOn == false)
            {
                lightOn = true;
                textLight.enabled = lightOn;
                textfieldtext.text = tutorialtexts[text_int];
                text_int++;
                return;
            }
            if (lightOn == true)
            {
                lightOn = false;
                textLight.enabled = lightOn;

                Invoke("textLight.enabled", 1f);//lightOn = true;
                                                // textLight.enabled = lightOn;
                textfieldtext.text = tutorialtexts[text_int];
                text_int++;
                return;
            }

        }
    }


    public void GazeText()
    {

        if (gaze.isInGaze && text_int == 0)
        {
            NextText();

        }


    }



    public void Texts()
    {
        tutorialtexts.Add("Hei!Tervetuloa!");
        tutorialtexts.Add("Tule lähemmäs");
        tutorialtexts.Add("Valitse itsellesi hahmo painamalla nappulaa");
        tutorialtexts.Add("Oletko tyytyväinen hattuun");
        tutorialtexts.Add("Hienoa! Suuntaa ovesta ulos ja pidä hauskaa!");
        int length = tutorialtexts.Count;
        Debug.Log(length);
    }
}

   /* IEnumerator LightTimer(float time)
    {
        if (coroutineExecute)
            
    }
}*/
