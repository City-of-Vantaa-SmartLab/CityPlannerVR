using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;


public class MenuSpawner : MonoBehaviour
{

    private Hand hand1;
    private Hand hand2;
    public Transform _camera;
    public GameObject menu;
    public bool menuActive = false;
    private InputMaster inputMaster;
    private PhotonSpawnableObject[] spawnslots;
    private bool isFirstTime = true;

    void Update()
    {

        if (isFirstTime && menu)
        {
            Debug.Log("Creating items for first time");
            FirstTime();
            isFirstTime = false;

        }
 
    }

    private void FirstTime()
    {
        hand1 = GameObject.Find("Player/SteamVRObjects/Hand1").GetComponent<Hand>();
        hand2 = GameObject.Find("Player/SteamVRObjects/Hand2").GetComponent<Hand>();
        menu = GameObject.Find("Inventory");
        _camera = GameObject.Find("VRCamera (eye)").transform;
       // menu.transform.position = _camera.transform.position;
        //menu.transform.position = new Vector3(menu.transform.position.x - 1, menu.transform.position.y, menu.transform.position.z - 0.5f);
        menu.transform.eulerAngles = new Vector3(-35, 130, 0);
        menu.SetActive(false);
        spawnslots = gameObject.GetComponentsInChildren<PhotonSpawnableObject>(true);

        inputMaster = GameObject.Find("Player").GetComponent<InputMaster>();
        if (inputMaster)
            Subscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    private void Subscribe()
    {
        inputMaster.Gripped += HandleGripped;
    }

    private void Unsubscribe()
    {
        inputMaster.Gripped -= HandleGripped;
    }


    private void HandleGripped(object sender, ClickedEventArgs e)
    {
        if (e.controllerIndex == 1)
        {

            Debug.Log("Gripped1");
        }

        else if (e.controllerIndex == 2)
        {
            if (menuActive == false)
            {
                Vector3 forwardPos = new Vector3(_camera.transform.forward.x, _camera.transform.forward.y - 0.4f, _camera.transform.forward.z);
                //menu2.transform.position = _camera.transform.position + Vector3.Normalize(_camera.transform.forward / 4);  


                //menu2.transform.LookAt(_camera);
                menu.transform.position = _camera.transform.position + Vector3.Normalize(forwardPos);
                menu.transform.rotation = Quaternion.LookRotation(forwardPos) * Quaternion.Euler(-90, 0, 0);
                //menu2.transform.localRotation = Quaternion.Euler(-35, 0, 0);
                menuActive = true;
                menu.SetActive(true);

            }

            else
            {
                foreach (PhotonSpawnableObject spawnslot in spawnslots)
                {

                    inputMaster.TriggerClicked -= spawnslot.HandleTriggerClicked;
                }
                menuActive = false;
                menu.SetActive(false);
            }

            Debug.Log("Gripped2");
        }
    }

};