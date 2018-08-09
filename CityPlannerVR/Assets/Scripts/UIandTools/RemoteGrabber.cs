using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class RemoteGrabber : MonoBehaviour {

    #region Variables

    private GameObject mainTarget;
    public List<GameObject> targetList;
    private InputMaster inputMaster;
    private ToolManager toolManager;
    private ToolManager.ToolType myToolType = ToolManager.ToolType.RemoteGrabber;
    private Hand myHand;
    private CapsuleCollider myCollider;
    private MeshRenderer myMesh;
    private bool isPulling;

    public int myHandNumber;
    public float pullTime;

    #endregion

    #region Initialization

    // Use this for initialization
    void Start () {
        if (!InitOwn())
            Debug.Log("Failed to initialize RemoteGrabber!");
        if (pullTime == 0)
            pullTime = 1f;
        CheckTool();
        Subscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    private bool InitOwn()
    {
        toolManager = GetComponentInParent<ToolManager>();
        if (toolManager)
        {
            myHandNumber = toolManager.myHandNumber;
        }
        myMesh = gameObject.GetComponent<MeshRenderer>();
        myCollider = gameObject.GetComponent<CapsuleCollider>();

        inputMaster = toolManager.inputMaster;
        myHand = GetComponentInParent<Hand>();
        if (!inputMaster || !toolManager || !myHand)
            return false;
        return true;
    }

    private void CheckTool()
    {
        TogglePower(toolManager.Tool == myToolType);
    }

    private void Subscribe()
    {
        //if (!inputMaster)
        //    inputMaster = GameObject.Find("Player").GetComponent<InputMaster>();

        if (inputMaster)
        {
            inputMaster.TriggerClicked += HandleTriggerClicked;
            inputMaster.TriggerUnclicked += HandleTriggerUnclicked;
        }
        else
        {
            Debug.Log("Did not find inputmaster!");
        }
        toolManager.AnnounceToolChanged += HandleToolChange;

    }

    private void Unsubscribe()
    {
        if (inputMaster)
        {
            inputMaster.TriggerClicked -= HandleTriggerClicked;
            inputMaster.TriggerUnclicked -= HandleTriggerUnclicked;
        }
        else
        {
            Debug.Log("Did not find inputmaster!");
        }
        toolManager.AnnounceToolChanged -= HandleToolChange;
    }

    #endregion

    #region Events and grabbing

    private void HandleTriggerClicked(object sender, ClickedEventArgs e)
    {
        if (e.controllerIndex == myHandNumber)
        {
            CalculateMainTarget();
            if (mainTarget)
            {
                StartCoroutine(StartPulling(mainTarget, pullTime, 0.1f));
                //myHand.AttachObject(mainTarget);
            }
        }
    }

    private void HandleTriggerUnclicked(object sender, ClickedEventArgs e)
    {
        isPulling = false;
        targetList.Remove(mainTarget);
        mainTarget = null;
    }

    private void HandleToolChange(uint handNumber, ToolManager.ToolType tool)
    {
        myHandNumber = (int)handNumber;
        if (tool == myToolType)
            TogglePower(true);
        else
        {
            TogglePower(false);
        }

    }

    private IEnumerator StartPulling(GameObject target, float pullTime, float increment)
    {
        GameObject grabFromPrefab = (GameObject)Resources.Load("Prefabs/Effects/GrabFrom");
        //GameObject grabToPrefab = (GameObject)Resources.Load("Prefabs/Effects/GrabTo");

        GameObject grabFrom = Instantiate(grabFromPrefab);
        //GameObject grabTo = Instantiate(grabToPrefab);
        //grabTo.transform.parent = transform;

        //grabFrom.transform.localScale = Vector3.one;
        //grabTo.transform.localScale = Vector3.one;
        //grabFrom.transform.localPosition = Vector3.zero;
        //grabTo.transform.localPosition = Vector3.zero;

        grabFrom.transform.position = target.transform.position;

        isPulling = true;
        float counter = 0;
        while (isPulling && counter < pullTime)
        {
            counter += increment;
            yield return new WaitForSeconds(increment);
        }
        if (isPulling)
        {
            myHand.AttachObject(target);
            SteamVR_Controller.Input((int)myHand.controller.index).TriggerHapticPulse(1000);
        }
        isPulling = false;

        //Destroy(grabFrom, 5f);
        //Destroy(grabTo, 5f);

        //if (grabFrom)
        //    Destroy(grabFrom);
        //if (grabTo)
        //    Destroy(grabTo);

        //Toolbeltin instantioinnista prujut
        //Vector3 localPos = toolbelt.transform.localPosition;
        //Vector3 globalScale = toolbelt.transform.localScale; //record prefabs true scale
        //toolbelt.transform.parent = playerBody.transform;
        ////toolbelt.transform.parent = transform;

        //toolbelt.transform.localScale = Vector3.one; //required for lossy scale
        //toolbelt.transform.localScale = new Vector3(globalScale.x / toolbelt.transform.lossyScale.x,
        //    globalScale.y / toolbelt.transform.lossyScale.y, globalScale.z / toolbelt.transform.lossyScale.z);
        //toolbelt.transform.localPosition = localPos;
    }

    #endregion

    #region Collisions

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Spawnable"))
        {
            if (!CheckTargetList(other.gameObject))
            {
                //Debug.Log("Adding " + other.gameObject.name + " to target list");
                targetList.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Spawnable"))
        {
            if (CheckTargetList(other.gameObject))
            {
                //Debug.Log("Removing " + other.gameObject.name + " from target list");
                targetList.Remove(other.gameObject);
                //if (isPulling && other.gameObject == mainTarget)
                //    isPulling = false;
            }
        }

    }

    #endregion

    public void TogglePower(bool isEnabled)
    {
        myMesh.enabled = isEnabled;
        myCollider.enabled = isEnabled;
        if (isEnabled == false)
        {
            mainTarget = null;
            targetList.Clear();
        }
    }


    private bool CheckTargetList(GameObject other)
    {
        if (targetList.Count != 0)
        {
            //Debug.Log("Checking list for targets...");
            if (targetList.Contains(other.gameObject))
                return true;
        }
        return false;
    }

    private void CalculateMainTarget()
    {
        if (targetList.Count != 0)
        {
            GameObject temp = null;
            float minDistance = 100;
            foreach (GameObject GO in targetList)
            {
                float distance = Vector3.Distance(myHand.transform.position, GO.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    temp = GO;
                }
            }
            mainTarget = temp;
        }
    }


}
