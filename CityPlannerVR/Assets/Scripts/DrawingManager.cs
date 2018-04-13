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
using Photon;
using System;

/// <summary> 
/// Receives events from InputListener and draws on the active controller if painter tool is selected.
/// </summary> 

public class DrawingManager : PunBehaviour {

    public int myHandNumber; //This should be set at inspector to either 1 or 2
    public SteamVR_TrackedObject myTrackedObj;
    public Material currentMaterial;
    public GameObject currentGO;
    //public GameObject colliderHolder;
    //public List<BoxCollider> colliderList;
    public List<Vector3> vectorList;


    [SerializeField]
    private uint myDeviceIndex;
    private LineRenderer currentLineRenderer;
    private float lineRendererWidth = 0.02f;
    private MeshLineRenderer currentLineMesh;
    private int numClicks;
    private InputListener inputListener;
    private ToolManager toolManager;
    private ToolManager.ToolType myTool;

    private bool initOwnSuccess;
    private bool triggerPressed;
    private bool alreadyDrawing;
    private bool addToPreviousObject;
    public bool useMeshes;

    // Use this for initialization
    void Start () {
        initOwnSuccess = InitOwn();  //for later checking in script
        if (!initOwnSuccess)
            Debug.Log("Failed to initialize DrawingManager on hand" + myHandNumber);
        //else
        //    Debug.Log("Initialized DrawingManager");

        Subscribe();
        triggerPressed = false;
        alreadyDrawing = false;
        addToPreviousObject = false;
        useMeshes = false;

        if (!currentMaterial)
        {
            currentMaterial = Resources.Load("Materials/Marker", typeof(Material)) as Material;
        }
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    private bool InitOwn()
    {
        if (myHandNumber == 0)
            Debug.Log("Hand number not set for DrawingManager! Set at inspector to either 1 or 2");
        inputListener = GameObject.Find("Player").GetComponent<InputListener>();
        toolManager = gameObject.GetComponentInParent<ToolManager>();
        if (toolManager)
            myTool = toolManager.currentTool;
        myTrackedObj = gameObject.GetComponent<SteamVR_TrackedObject>();

        if (!inputListener || !toolManager || !myTrackedObj)
            return false;
        return true;
    }

    private void Subscribe()
    {
        if (!inputListener)
            inputListener = GameObject.Find("Player").GetComponent<InputListener>();

        if (inputListener)
        {
            inputListener.TriggerClicked += HandleTriggerClicked;
            inputListener.TriggerLifted += HandleTriggerLifted;
            if (myHandNumber == 1)
                inputListener.Hand1DeviceFound += HandleMyIndexFound;
            if (myHandNumber == 2)
                inputListener.Hand2DeviceFound += HandleMyIndexFound;
        }
        else
        {
            Debug.Log("Did not find inputlistener!");
        }
        toolManager.OnToolChange += HandleToolChange;

    }

    private void Unsubscribe()
    {
        if (inputListener)
        {
            inputListener.TriggerClicked -= HandleTriggerClicked;
            inputListener.TriggerLifted -= HandleTriggerLifted;
            if (myHandNumber == 1)
                inputListener.Hand1DeviceFound -= HandleMyIndexFound;
            if (myHandNumber == 2)
                inputListener.Hand2DeviceFound -= HandleMyIndexFound;
        }
        else
        {
            Debug.Log("Did not find inputlistener!");
        }
        toolManager.OnToolChange -= HandleToolChange;
    }


    private void HandleTriggerClicked(object sender, ClickedEventArgs e)
    {
        if (myTool == ToolManager.ToolType.Painter)
        {
            if (!alreadyDrawing && e.controllerIndex == myDeviceIndex)
            {
                triggerPressed = true;
                StartDrawing(sender, e);
                alreadyDrawing = true;
            }
        }
    }

    private void HandleTriggerLifted(object sender, ClickedEventArgs e)
    {

        if (e.controllerIndex == myDeviceIndex && alreadyDrawing)
        {
            triggerPressed = false;
            alreadyDrawing = false;
            Solidify();
            currentLineMesh = null;
            currentLineRenderer = null;
        }

    }

    private void HandleMyIndexFound(uint deviceIndex)
    {
        myDeviceIndex = deviceIndex;
        //if (myHandNumber == 1)
        //    inputListener.Hand1DeviceFound -= HandleMyIndexFound;
        //if (myHandNumber == 2)
        //    inputListener.Hand2DeviceFound -= HandleMyIndexFound;
    }

    private void HandleToolChange(uint deviceIndex, ToolManager.ToolType tool)
    {
        //Debug.Log("Tool change initiated");
        if (deviceIndex == myDeviceIndex)
        {
            myTool = tool;
            //Debug.Log("Tool change succesful");
        }
    }

    void StartDrawing(object sender, ClickedEventArgs e)
    {
        //Debug.Log("Start drawing");
        if (!addToPreviousObject)
        {
            CreateNewLine();
            vectorList = new List<Vector3>();
        }

        if (currentLineRenderer)
        {
            if (e.controllerIndex == myDeviceIndex)
                StartCoroutine(KeepDrawingLineRenderer(myTrackedObj));
        }
        else if (currentLineMesh)
        {
            if (e.controllerIndex == myDeviceIndex)
                StartCoroutine(KeepDrawingLineMesh(myTrackedObj));
        }

    }

    IEnumerator KeepDrawingLineRenderer(SteamVR_TrackedObject trackedObject)
    {
        while (triggerPressed)
        {
            //Debug.Log("Still drawing");
            currentLineRenderer.positionCount = numClicks + 1;
            vectorList.Add(trackedObject.transform.position);
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

    //Moves current object to drawings holder and creates colliders
    void Solidify()
    {
        GameObject drawings = GameObject.Find("Drawings");
        if (!drawings)
        {
            Debug.Log("Creating drawings holder");
            drawings = new GameObject("Drawings");
            AddPhotonComponents(drawings);
        }
        if (drawings)
        {
            currentGO.name = "Doodle" + drawings.transform.childCount;
            currentGO.transform.parent = drawings.transform;
            Debug.Log("Transferred "+ currentGO.name + " to new parent: " + drawings.name);
        }
        else
            Debug.Log("Could not find drawings holder at root!");


        if (currentLineMesh)
        {
            MeshCollider coll = currentGO.AddComponent<MeshCollider>();
            coll.convex = true;
        }

        if(currentLineRenderer)
        {
            //CreateColliders();
        }
        addToPreviousObject = false;

        Rigidbody rigidbody = currentGO.AddComponent<Rigidbody>();
        rigidbody.useGravity = false;

        AddInteractionComponents(currentGO);
        AddPhotonComponents(currentGO);
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
        currentLineRenderer.startWidth = lineRendererWidth;
        currentLineRenderer.endWidth = lineRendererWidth;
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

    private void AddInteractionComponents(GameObject go)
    {
        go.AddComponent<Interactable>();
        go.AddComponent<Throwable>();
        go.AddComponent<IsAttachedToHand>();
        go.AddComponent<HighlightSelection>();
        go.AddComponent<Erasable>();
    }

    void AddPhotonComponents(GameObject go)
    {
        PhotonView photonV = go.AddComponent<PhotonView>();
        //PhotonTransformView photonTV = go.AddComponent<PhotonTransformView>();  //replaced by photon networked object script
        go.AddComponent<PhotonObjectOwnershipHandler>();
        PhotonNetworkedObject netObject = go.AddComponent<PhotonNetworkedObject>();

        //photonTV.m_PositionModel.SynchronizeEnabled = true;
        //photonTV.m_RotationModel.SynchronizeEnabled = true;

        photonV.ObservedComponents = new List<Component>();
        //photonV.ObservedComponents.Add(photonTV);
        photonV.ObservedComponents.Add(netObject);
        photonV.synchronization = ViewSynchronization.UnreliableOnChange;
    }

    private void CreateColliders()
    {
        //public GameObject colliderHolder;
        //public List<BoxCollider> colliderList;
        //public List<Vector3> vectorList;
        //List<BoxCollider> colliderList = new List<BoxCollider>();

        //LineRenderer linRend = gameObject.GetComponent<LineRenderer>();

        //List<Vector3> vectorList2 = new List<Vector3>(linRend.positionCount);
        //linRend.GetPositions(vectorList2); // List<Vector3> vs. Vector3[] !!!

        Vector3 previousVector = vectorList[0];
        Vector3 link;
        int counter = 0;

        foreach (Vector3 vector in vectorList)
        {

            if (counter < 1)
            {
                counter++;
                continue;
            }
            counter = 0;
            GameObject colliderHolder = new GameObject("Collider" + currentGO.transform.childCount);
            colliderHolder.transform.parent = currentGO.transform;
            BoxCollider box = colliderHolder.AddComponent<BoxCollider>();
            box.isTrigger = true;
            link = vector - previousVector;

            //box.size = Vector3.one * lineRendererWidth;
            box.size = new Vector3(link.magnitude, lineRendererWidth, lineRendererWidth);
            //box.transform.LookAt(vector);
            box.center = currentGO.transform.position + previousVector + (link / 2);

            previousVector = vector;
        }

        //vectorList.Clear();
    }

}
