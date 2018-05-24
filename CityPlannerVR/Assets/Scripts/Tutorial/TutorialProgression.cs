using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialProgression : MonoBehaviour
{

   // public List<string> tutorialtexts = new List<string>();
  //  public int textlistsize;
    public int textlistsize1;
    int text_int = 0;
  //  public GameObject textfield;
    public List<GameObject> tutexts = new List<GameObject>();
  //  public TextMesh textfieldtext;
    public SteamVR_GazeTracker gaze;
  //  public float timer = 0f;
    //public float gazeTime = 2f;
    public Light textLight;
    private bool lightOn = false;
    public GameObject player;

    public List<Material> AvatarMaterials = new List<Material>();
    public int matListSize;
    public int check;


    private void Awake()
    {

        foreach (Material mat in Resources.LoadAll("AvatarM", typeof(Material)))
        {

            AvatarMaterials.Add(mat);
            matListSize = AvatarMaterials.Count;
            
        }

    }

    // Use this for initialization
    void Start()
    {
       // Texts();
        //textlistsize = tutorialtexts.Count;
        textlistsize1 = tutexts.Count;
      //  textfield = GameObject.Find("Aloitus");
        gaze = gameObject.GetComponent<SteamVR_GazeTracker>();
       // textfieldtext = textfield.GetComponent<TextMesh>();
    }

    void Update()
    {
        if (gaze.isInGaze && text_int == 0)
        {
            NextText();
        }
    }

    public void NextAvatar()
    {
        GameObject part_time;
        if (text_int == 0)
        {
            part_time = tutexts[text_int];
            part_time.SetActive(true);
            lightOn = true;
            textLight.enabled = lightOn;
            text_int++;
        }
        else if (text_int >= 1 && text_int <= textlistsize1)
        {

            if (!lightOn)
            {
                part_time = tutexts[text_int - 1];
                part_time.SetActive(false);
                lightOn = true;
                textLight.enabled = lightOn;
                part_time = tutexts[text_int];
                part_time.SetActive(true);
                text_int++;
                return;
            }
            else if (lightOn)
            {
                part_time = tutexts[text_int - 1];
                part_time.SetActive(false);
                lightOn = false;
                textLight.enabled = lightOn;
                LightTimer();
                part_time = tutexts[text_int];
                part_time.SetActive(true);
                text_int++;


                lightOn = true;
                textLight.enabled = lightOn;


                // textfieldtext.text = tutorialtexts[text_int];
                // text_int++;
                return;
            }

        }
    }


    public void NextText()
    {
        GameObject part_time;
        if (text_int == 0)
        {
            part_time = tutexts[text_int];
            part_time.SetActive(true);
            lightOn = true;
            textLight.enabled = lightOn;
            text_int++;
        }
        else if (text_int >= 1 && text_int <= textlistsize1)
        {
       
            if (!lightOn)
            {
                part_time = tutexts[text_int-1];
                part_time.SetActive(false);
                lightOn = true;
                textLight.enabled = lightOn;
                part_time = tutexts[text_int];
                part_time.SetActive(true);
                text_int++;
                return;
            }
            else if (lightOn)
            {
                part_time = tutexts[text_int - 1];
                part_time.SetActive(false);
                lightOn = false;
                textLight.enabled = lightOn;
                LightTimer();
                part_time = tutexts[text_int];
                part_time.SetActive(true);
                text_int++;
              

                lightOn = true;
                textLight.enabled = lightOn;

             
               // textfieldtext.text = tutorialtexts[text_int];
               // text_int++;
            return;
            }

        }
    }
  
   /* public void Texts()
    {
        tutorialtexts.Add("Hei!Tervetuloa!");
        tutorialtexts.Add("Tule lähemmäs");
        tutorialtexts.Add("Valitse itsellesi hahmo\npainamalla nappulaa");
        tutorialtexts.Add("Oletko tyytyväinen hattuun");
        tutorialtexts.Add("Hienoa! Suuntaa ovesta ulos ja pidä hauskaa!");
        int length = tutorialtexts.Count;
        Debug.Log(length);
    }
    */
    IEnumerator LightTimer()
    {
        yield return new WaitForSeconds(1.5f);

    }
}

  
