/// The object currentLineMesh utilises MeshLineRenderer script, which can be found
/// in a youtube tutorial (earlier version named GraphicsLineRenderer), which itself follows
/// instructions in: http://www.everyday3d.com/blog/index.php/2010/03/15/3-ways-to-draw-3d-lines-in-unity3d/
/// Youtube tutorial: https://youtu.be/eMJATZI0A7c?t=34m38s
/// Pastebin: https://pastebin.com/yzW5Nn7f


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;
using Valve.VR.InteractionSystem;


/// <summary> 
/// Receives events from InputListener and draws on the active controller.
/// </summary> 

public class DrawingManager : MonoBehaviour {

    public SteamVR_TrackedObject trackedObj1;
    public SteamVR_TrackedObject trackedObj2;
    public Material currentMaterial;
    public GameObject currentGO;

    private uint hand1Index;
    private uint hand2Index;
    private uint currentHandIndex;
    private LineRenderer currentLineRenderer;
    private MeshLineRenderer currentLineMesh;
    public List<Vector3> colliderPoints;
    private int numClicks;
    private InputListener inputList;

    private bool triggerPressed;
    private bool alreadyDrawing;
    private bool addToPreviousObject;
    public bool useMeshes;

    // Use this for initialization
    void Start () {
        inputList = gameObject.GetComponent<InputListener>();
        inputList.TriggerClicked += HandleTriggerClicked;
        inputList.TriggerLifted += HandleTriggerLifted;
        hand1Index = inputList.hand1Index;
        hand2Index = inputList.hand2Index;
        trackedObj1 = inputList.hand1TrackedObject;
        trackedObj2 = inputList.hand2TrackedObject;

        triggerPressed = false;
        alreadyDrawing = false;
        addToPreviousObject = false;
        useMeshes = false;

        if (!currentMaterial)
        {
            currentMaterial = Resources.Load("Materials/Marker", typeof(Material)) as Material;
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
            {
                triggerPressed = true;
                StartDrawing(sender, e);
                alreadyDrawing = true;
            }



        }
    }

    private void HandleTriggerLifted(object sender, ClickedEventArgs e)
    {
        if (e.controllerIndex == currentHandIndex)
        {
            triggerPressed = false;
            alreadyDrawing = false;
            Solidify();

        }

    }

    void StartDrawing(object sender, ClickedEventArgs e)
    {
        //Debug.Log("Start drawing");
        if (!addToPreviousObject)
        {
            CreateNewLine();
            colliderPoints = new List<Vector3>();
        }
        currentHandIndex = e.controllerIndex;

        if (currentLineRenderer)
        {
            if (e.controllerIndex == hand1Index)
                StartCoroutine(KeepDrawingLineRenderer(trackedObj1));
            else if (e.controllerIndex == hand2Index)
                StartCoroutine(KeepDrawingLineRenderer(trackedObj2));
        }
        else if (currentLineMesh)
        {
            if (e.controllerIndex == hand1Index)
                StartCoroutine(KeepDrawingLineMesh(trackedObj1));
            else if (e.controllerIndex == hand2Index)
                StartCoroutine(KeepDrawingLineMesh(trackedObj2));
        }

    }

    IEnumerator KeepDrawingLineRenderer(SteamVR_TrackedObject trackedObject)
    {
        while (triggerPressed)
        {
            //Debug.Log("Still drawing");
            currentLineRenderer.positionCount = numClicks + 1;
            colliderPoints.Add(trackedObject.transform.position);
            currentLineRenderer.SetPosition(numClicks, trackedObject.transform.position);
            numClicks++;
            
            yield return new WaitForSeconds(.01f);
        }

    }

    IEnumerator KeepDrawingLineMesh(SteamVR_TrackedObject trackedObject)
    {
        while (triggerPressed)
        {
            currentLineMesh.AddPoint(trackedObject.transform.position);
            yield return new WaitForSeconds(.05f);
        }

    }


    void Solidify()
    {
        if (currentLineMesh)
        {
            MeshCollider coll = currentGO.AddComponent<MeshCollider>();
            coll.convex = true;
        }

        currentGO.AddComponent<Interactable>();
        currentGO.AddComponent<Throwable>();
        currentGO.AddComponent<IsAttachedToHand>();
        currentGO.AddComponent<HighlightSelection>();
        addToPreviousObject = false;
    }

    void CreateNewLine()
    {
        currentGO = new GameObject();
        addToPreviousObject = true;
        if (useMeshes)
            CreateLineMesh();
        else
            CreateLineRenderer();
    }

    void CreateLineRenderer()
    {
        currentLineRenderer = currentGO.AddComponent<LineRenderer>();
        currentLineRenderer.material = currentMaterial;
        currentLineRenderer.startWidth = 0.02f;
        currentLineRenderer.endWidth = 0.02f;
        numClicks = 0;
    }

    void CreateLineMesh()
    {
        currentGO.AddComponent<MeshFilter>();
        currentGO.AddComponent<MeshRenderer>();

        currentLineMesh = currentGO.AddComponent<MeshLineRenderer>();
        currentLineMesh.setWidth(0.05f);
        currentLineMesh.lmat = currentMaterial;
    }










}
