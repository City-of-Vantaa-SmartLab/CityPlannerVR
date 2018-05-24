﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialProgression : MonoBehaviour
{

 
    public int textlistsize1;
    public GameObject buttontext1;
    public GameObject buttontext2;
    public GameObject mirrorLights;
    public GameObject yesBut;
    

    public List<GameObject> tutexts = new List<GameObject>();
    public SteamVR_GazeTracker gaze;
    public Light textLight;
    private bool lightOn = false;
    public GameObject player;

    public List<Material> AvatarMaterials = new List<Material>();
    public int matListSize;
    public int check;
    public bool yesButton = false;
    public int matnum = 0;
    public int text_int = 0;
    public bool modelcycle = false;


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
        textlistsize1 = tutexts.Count;
        player = GameObject.Find("AvatarM");
      
        gaze = gameObject.GetComponent<SteamVR_GazeTracker>();
     
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
        
        if (matnum <= matListSize-1)
        {
            foreach (Transform child in player.transform)
            {
                child.GetComponent<MeshRenderer>().material = AvatarMaterials[matnum];
            }
            if (matnum == matListSize-1)
            {
                matnum = 0;
            }
            else
            {
                matnum++;
            }
        }
    }

    public void ToggleYes()
    {
        if (text_int <= 2)
        {
            yesButton = true;
            yesBut.GetComponentInChildren<Light>().enabled = false;
            NextText();
        }
        else
        {
            return;
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
            mirrorLights.SetActive(true);
            buttontext1.SetActive(true);
            text_int++;
        }
        
        else if (text_int == 1)
        {
            if (yesButton == true)
            {
                buttontext2.SetActive(false);
                buttontext1.SetActive(true);
                if (text_int == 1)
                {
                    text_int = 3;
                    part_time = tutexts[2];
                    part_time.SetActive(false);
                    part_time = tutexts[text_int];
                    part_time.SetActive(true);
                }
                else if (text_int == 2)
                {
                    part_time = tutexts[text_int];
                    part_time.SetActive(false);
                    text_int++;
                    part_time = tutexts[text_int];
                }
                
            }
            else
            {
                if (matnum == 3 && modelcycle == false)
                {
                    part_time = tutexts[text_int];
                    part_time.SetActive(false);
                    part_time = tutexts[text_int+1];
                    part_time.SetActive(true);
                    buttontext1.SetActive(false);
                    buttontext2.SetActive(true);
                    NextAvatar();
                    modelcycle = true;
                }

                else if (modelcycle == false)
                {
                    part_time = tutexts[text_int - 1];
                    part_time.SetActive(false);
                    part_time = tutexts[text_int];
                    part_time.SetActive(true);
                    buttontext1.SetActive(false);
                    buttontext2.SetActive(true);
                    NextAvatar();
                }

                else if (modelcycle == true)
                {
                    part_time = tutexts[text_int];
                    part_time.SetActive(false);
                    part_time = tutexts[text_int+1];
                    part_time.SetActive(true);
                    buttontext1.SetActive(false);
                    buttontext2.SetActive(true);
                    NextAvatar();
                }
            }
        }

        else if (text_int > 2 && text_int <= textlistsize1)
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

            return;
            }

        }
    }
  
 
    IEnumerator LightTimer()
    {
        yield return new WaitForSeconds(1.5f);

    }
    IEnumerator Timer()
    {
        yield return new WaitForSeconds(4f);

    }
}

  
