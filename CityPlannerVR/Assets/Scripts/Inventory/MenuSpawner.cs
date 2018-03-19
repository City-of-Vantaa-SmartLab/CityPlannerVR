using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;


public class MenuSpawner : MonoBehaviour
{

    private Hand hand1;
    private Hand hand2;
    public Transform _camera;
    public GameObject menu1;
    public GameObject menu2;
    private bool menu1Active = false;
    private bool menu2Active = false;



    void Start()
    {
        hand1 = GameObject.Find("Player/SteamVRObjects/Hand1").GetComponent<Hand>();
        hand2 = GameObject.Find("Player/SteamVRObjects/Hand2").GetComponent<Hand>();
        menu1 = GameObject.Find("Inventory");
        menu2 = GameObject.Find("Inventory2");
        _camera = GameObject.Find("VRCamera").transform;
        menu2.transform.position = _camera.transform.position;
        menu2.transform.position = new Vector3(menu2.transform.position.x - 1, menu2.transform.position.y, menu2.transform.position.z - 0.5f);
        menu2.transform.eulerAngles = new Vector3(-35, 130, 0);
        menu2.SetActive(false);
        menu1.SetActive(false);

    }




    private void Update()
    {
        if (hand1.controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
        {
            if (menu1Active == false)
            {
                menu1Active = true;
                menu1.SetActive(true);

            }

            else
            {
                menu1Active = false;
                menu1.SetActive(false);
            }

            Debug.Log("Gripped1");
        }

        if (hand2.controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
        {
            if (menu2Active == false)
            {
                Vector3 forwardPos = new Vector3(_camera.transform.forward.x, _camera.transform.forward.y - 0.4f, _camera.transform.forward.z);
                //menu2.transform.position = _camera.transform.position + Vector3.Normalize(_camera.transform.forward / 4);  


                //menu2.transform.LookAt(_camera);
                menu2.transform.position = _camera.transform.position + Vector3.Normalize(forwardPos);
                menu2.transform.rotation = Quaternion.LookRotation(forwardPos) * Quaternion.Euler(-90, 0, 0);
                //menu2.transform.localRotation = Quaternion.Euler(-35, 0, 0);
                menu2Active = true;
                menu2.SetActive(true);

            }

            else
            {
                menu2Active = false;
                menu2.SetActive(false);
            }

            Debug.Log("Gripped2");
        }

    }

};