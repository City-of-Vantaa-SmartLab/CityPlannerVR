using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary> 
/// Receives events from InputListener and draws on the active controller.
/// </summary> 

public class DrawingManager : MonoBehaviour {

    public SteamVR_TrackedObject trackedObj1;
    public SteamVR_TrackedObject trackedObj2;
    public Material currentMaterial;

    private uint hand1Index;
    private uint hand2Index;
    private uint currentHandIndex;
    private LineRenderer currentLine;
    private int numClicks;
    private InputListener inputList;
    private bool triggerPressed;
    private bool alreadyDrawing;


    // Use this for initialization
    void Start () {
        inputList = gameObject.GetComponent<InputListener>();
        inputList.TriggerClicked += HandleTriggerClicked;
        inputList.TriggerLifted += HandleTriggerLifted;
        hand1Index = inputList.hand1Index;
        hand2Index = inputList.hand2Index;
        trackedObj1 = inputList.hand1TrackedObject;
        trackedObj2 = inputList.hand2TrackedObject;
        alreadyDrawing = false;

        if (!currentMaterial)
        {
            currentMaterial = Resources.Load("Materials/Marker", typeof(Material)) as Material;  //material most likely should be in the resources folder!
        }

    }

    private void OnDisable()
    {
        inputList.TriggerClicked -= HandleTriggerClicked;
        inputList.TriggerLifted -= HandleTriggerLifted;
    }


    private void HandleTriggerClicked(object sender, ClickedEventArgs e)
    {
        if (!inputList.lasersAreActive)
        {
            if (!alreadyDrawing)
                StartDrawing(sender, e);
            alreadyDrawing = true;
        }
    }

    private void HandleTriggerLifted(object sender, ClickedEventArgs e)
    {
        if (e.controllerIndex == currentHandIndex)
        {
            triggerPressed = false;
            alreadyDrawing = false;
        }

    }

    void StartDrawing(object sender, ClickedEventArgs e)
    {
        //Debug.Log("Start drawing");
        triggerPressed = true;
        GameObject go = new GameObject();
        currentLine = go.AddComponent<LineRenderer>();
        currentLine.material = currentMaterial;
        currentLine.startWidth = 0.02f;
        currentLine.endWidth = 0.02f;
        numClicks = 0;
        currentHandIndex = e.controllerIndex;

        if (e.controllerIndex == hand1Index)
            StartCoroutine(KeepDrawing(trackedObj1));
        else if (e.controllerIndex == hand2Index)
            StartCoroutine(KeepDrawing(trackedObj2));
    }

    IEnumerator KeepDrawing(SteamVR_TrackedObject trackedObject)
    {
        while (triggerPressed)
        {
            //Debug.Log("Still drawing");
            currentLine.positionCount = numClicks + 1;
            currentLine.SetPosition(numClicks, trackedObject.transform.position);
            numClicks++;
            yield return new WaitForSeconds(.01f);
        }

    }
}
