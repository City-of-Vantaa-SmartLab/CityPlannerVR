using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPathHandler : MonoBehaviour {

	//The gameobject to be instantiated
    public GameObject pathPoint;

	public GameObject videoCamera;
	public GameObject cameraScreen;

    public GameObject pathDrawer;

	[HideInInspector]
	public int myHandNumber;


    public Valve.VR.InteractionSystem.Hand hand;


    //public ToolManager toolManager;

    #region private variables

    GameObject player;

    InputMaster inputMaster;
	XRLineRenderer line;
    CheckPlayerSize playerSize;

	//The instantiated gameobject
	GameObject point;

	PathVideoCamera pathVideoCamera;

	static int pathPointIndex = 0;

	GameObject selectedPoint;
	bool holdTrigger = false;

    ToolManager toolManager;

    #endregion

    private void Awake()
    {
        player = GameObject.Find("Player");
        inputMaster = player.GetComponent<InputMaster>();
        line = GameObject.Find("CameraPathLineDrawer").GetComponent<XRLineRenderer>();
        playerSize = player.GetComponent<CheckPlayerSize>();

        pathVideoCamera = videoCamera.GetComponent<PathVideoCamera>();
        toolManager = hand.GetComponent<ToolManager>();

        pathVideoCamera.pathPoints = new List<GameObject>();
        InitializePathLine();
        //pathVideoCamera.tool = PathVideoCamera.Tool.Add;

        videoCamera.SetActive(false);
        cameraScreen.SetActive(false);
    }

    void Start(){

		Subscribe ();

		//myHandNumber = toolManager.myHandNumber;
		Debug.Log ("myHandNumber = " + myHandNumber);
	}

    private void OnEnable()
    {
        if (inputMaster != null)
        {
            Subscribe();
            if (pathVideoCamera.pathPoints.Count > 0)
            {
                for (int i = 0; i < pathVideoCamera.pathPoints.Count; i++)
                {
                    pathVideoCamera.pathPoints[i].SetActive(true);
                }
                pathDrawer.SetActive(true);
            }
        }
    }

    private void OnDisable()
    {
        if (inputMaster != null)
        {
            if (hand.otherHand.GetComponent<ToolManager>().Tool != ToolManager.ToolType.PathCamera)
            {
                Unsubscribe();
                if (pathVideoCamera.pathPoints.Count > 0)
                {
                    for (int i = 0; i < pathVideoCamera.pathPoints.Count; i++)
                    {
                        pathVideoCamera.pathPoints[i].SetActive(false);
                    }
                    pathDrawer.SetActive(false);
                }
            }
        }
    }

    void Subscribe(){

        inputMaster.TriggerClicked += TriggerPressed;
        inputMaster.TriggerUnclicked += TriggerReleased;

        inputMaster.TriggerClicked += InstantiatePathPoint;
        inputMaster.TriggerClicked += ActivatePointMoving;
        inputMaster.TriggerClicked += RemovePoint;
		inputMaster.TriggerClicked += ActivatePathCamera;
	}

	void Unsubscribe(){
		inputMaster.TriggerClicked -= TriggerPressed;
		inputMaster.TriggerUnclicked -= TriggerReleased;

		inputMaster.TriggerClicked -= InstantiatePathPoint;
		inputMaster.TriggerClicked -= ActivatePointMoving;
		inputMaster.TriggerClicked -= RemovePoint;
		inputMaster.TriggerClicked -= ActivatePathCamera;
	}

	//--------------------------------------------------------------------------------------------------------------------------------
	//													INSTANTIATE POINT
	//--------------------------------------------------------------------------------------------------------------------------------

	private void InstantiatePathPoint(object sender, ClickedEventArgs e)
    {
		if (e.controllerIndex == myHandNumber) {
            if (toolManager.Tool == ToolManager.ToolType.PathCamera)
            {
                if (!playerSize.isSmall)
                {
                    if (pathVideoCamera.tool == PathVideoCamera.Tool.Add)
                    {
                        point = Instantiate(pathPoint, transform.position, transform.rotation) as GameObject;
                        pathVideoCamera.pathPoints.Add(point);

                        DrawLineBetweenPoints();
                    }
                }
            }
		}
    }

	private void DrawLineBetweenPoints(){

		line.SetVertexCount (pathPointIndex + 1);
		line.SetPosition (pathPointIndex, pathVideoCamera.pathPoints[pathPointIndex].transform.position);
		pathPointIndex++;
		
	}

	private void InitializePathLine(){
		line.SetVertexCount (0);
		pathPointIndex = 0;
	}

	//--------------------------------------------------------------------------------------------------------------------------------
	//													MOVE POINT
	//--------------------------------------------------------------------------------------------------------------------------------
	private void ActivatePointMoving(object sender, ClickedEventArgs e){
		if (e.controllerIndex == myHandNumber) {
            if (toolManager.Tool == ToolManager.ToolType.PathCamera)
            {
                if (!playerSize.isSmall)
                {
                    if (pathVideoCamera.tool == PathVideoCamera.Tool.Move)
                    {
                        StartCoroutine(MovePoint());
                    }
                }
            }
		}
	}

	private IEnumerator MovePoint(){

		if (selectedPoint != null) {

			while (holdTrigger) {
				selectedPoint.transform.position = transform.position;
				selectedPoint.transform.rotation = transform.rotation;

				int index = pathVideoCamera.pathPoints.IndexOf (selectedPoint);
				line.SetPosition (index, selectedPoint.transform.position);

				yield return null;
			}
		}

		else {
			yield break;
		}
	}

	private void TriggerPressed(object sender, ClickedEventArgs e){
		if (e.controllerIndex == myHandNumber) {
			holdTrigger = true;
		}
	}

	private void TriggerReleased(object sender, ClickedEventArgs e){
		if (e.controllerIndex == myHandNumber) {
			holdTrigger = false;
		}
	}

	//--------------------------------------------------------------------------------------------------------------------------------
	//													REMOVE POINT
	//--------------------------------------------------------------------------------------------------------------------------------

	private void RemovePoint(object sender, ClickedEventArgs e){
		if (e.controllerIndex == myHandNumber) {
            if (toolManager.Tool == ToolManager.ToolType.PathCamera)
            {
                if (pathVideoCamera.tool == PathVideoCamera.Tool.Remove)
                {
                    if (!playerSize.isSmall)
                    {
                        if (selectedPoint != null)
                        {

                            pathVideoCamera.pathPoints.Remove(selectedPoint);

                            Destroy(selectedPoint);

                            ReDrawPath();

                            selectedPoint = null;
                            //pathVideoCamera.tool = PathVideoCamera.Tool.Add;
                        }
                    }
                }
            }
		}
	}

	private void ReDrawPath(){

		InitializePathLine ();
		line.SetVertexCount (pathVideoCamera.pathPoints.Count);
		pathPointIndex = pathVideoCamera.pathPoints.Count;

		for (int i = 0; i < pathVideoCamera.pathPoints.Count; i++) {
			line.SetPosition(i, pathVideoCamera.pathPoints[i].transform.position);
		}
	}
		
	//--------------------------------------------------------------------------------------------------------------------------------

	void OnCollisionEnter(Collision other){
		if (other.gameObject.tag == "CameraPathPoint") {
			selectedPoint = other.gameObject;

			//pathVideoCamera.tool = PathVideoCamera.Tool.Move;
		}
	}

	void OnCollisionExit(Collision other){
		if (other.gameObject.tag == "CameraPathPoint") {
			selectedPoint = null;

			//pathVideoCamera.tool = PathVideoCamera.Tool.Add;
		}
	}

	void ActivatePathCamera(object sender, ClickedEventArgs e){
		if (e.controllerIndex == myHandNumber) {
            if (toolManager.Tool == ToolManager.ToolType.PathCamera)
            {
                if (pathVideoCamera.tool == PathVideoCamera.Tool.Capture)
                {
                    if (pathVideoCamera.pathPoints.Count > 0)
                    {
                        videoCamera.SetActive(true);

                        cameraScreen.SetActive(true);
                        cameraScreen.transform.parent = gameObject.transform;
                        cameraScreen.transform.localPosition = Vector3.zero;
                        cameraScreen.transform.localRotation = Quaternion.identity;


                        pathVideoCamera.InitializeCamera();
                    }
                }
            }
		}
	}
}
