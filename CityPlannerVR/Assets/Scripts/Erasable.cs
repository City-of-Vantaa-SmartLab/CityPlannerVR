using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Erasable : MonoBehaviour {

    Collider myCollider;
    bool alreadyHovering;
    Eraser eraser;
    
    // Use this for initialization
    void Start () {
        myCollider = gameObject.GetComponent<Collider>();
        alreadyHovering = false;
	}

    private void OnDestroy()
    {
        Unsubscribe();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!alreadyHovering && other.CompareTag("Eraser"))
        {
            eraser = other.GetComponent<Eraser>();
            if (eraser)
            {
                Subscribe();
            }
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (alreadyHovering && other.CompareTag("Eraser"))
        {

        }
    }


    private void Subscribe()
    {
        eraser.DestroyObjects += HandleDestroyObjects;
        eraser.ClearList += HandleClearList;
    }

    private void Unsubscribe()
    {
        eraser.DestroyObjects -= HandleDestroyObjects;
        eraser.ClearList -= HandleClearList;

    }



    private void HandleDestroyObjects(uint deviceIndex)
    {
        Destroy(gameObject, 0.1f);
    }

    private void HandleClearList(uint deviceIndex)
    {
        Unsubscribe();
    }


    // Update is called once per frame
    void Update () {
		
	}
}
