using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;


/// <summary> 
/// Attached gameobject can be selected and highlighted using hardcoded shaders. 
/// Selection also utilises SelectionList script, adding attached object to player's selected list. 
/// </summary> 

//TODO?: add sound to selection function
//TODO?: Different higlight implementation, maybe materials


public class HighlightSelection : PunBehaviour
{
    
    //private Shader diffuse;
    private Renderer rend;
    private XRLineRenderer lineRend;


    public bool isHighlighted;
    public bool isSelected;
    public List<GameObject> lasersPointing; //will be implemented when laserpointers are added to photonplayeravatar


    private GameObject owner;
    [SerializeField]
    private SelectionList lista;
    [SerializeField]
    private SteamVR_GazeTracker gazeTracker;

    void Start()
    {
        isHighlighted = false;
        isSelected = false;

        //standard = Shader.Find("Standard");
        //highlight = Shader.Find("Valve/VR/Highlight");
        //selected = Shader.Find("FX/Flare");
        rend = this.GetComponent<MeshRenderer>();
        lineRend = this.GetComponent<XRLineRenderer>();
        gazeTracker = GetComponent<SteamVR_GazeTracker>();
        SubscriptionOn();
    }


    private void OnDestroy()
    {
        SubscriptionOff();
    }

    private void SubscriptionOn()
    {
        if (gazeTracker)
        {
            gazeTracker.GazeOn += HandleGazeOn;
            gazeTracker.GazeOff += HandleGazeOff;
        }
    }

    private void SubscriptionOff()
    {
        if (gazeTracker)
        {
            gazeTracker.GazeOn -= HandleGazeOn;
            gazeTracker.GazeOff -= HandleGazeOff;
        }
    }

    private void HandleClearSelection(int deviceIndex)
    {
        ToggleSelection(owner);
    }

    private void HandleGazeOn(object sender, GazeEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void HandleGazeOff(object sender, GazeEventArgs e)
    {
        throw new NotImplementedException();
    }

    public void ToggleHighlight(object sender, bool status)
    {

        if (isHighlighted)
        {
            isHighlighted = false;
            //priorize selection shader over highlight 
            if (!isSelected)
                ChangeShaderRPC("Standard");

        }
        else
        {
            isHighlighted = true;
            if (!isSelected)
                ChangeShaderRPC("Valve/VR/Highlight");
        }
        isHighlighted = status;
    }


    public void ToggleSelection(GameObject selectingPlayer)
    {
        if (selectingPlayer == null)
        {
            Debug.Log("Selecting player is null!");
            return;
        }
        if (owner != null && owner != selectingPlayer)
        {
            Debug.Log("Currently selected by someone else!");
            return;
        }

        if (isSelected)
        {
            isSelected = false;
            owner.GetComponent<InputMaster>().ClearSelections -= HandleClearSelection;  //releases ownership of selected item
            owner = null;
            lista.RemoveFromList(this.gameObject, lista.selectedList);
            if (tag == "Grid")
            {
                Destroy(transform.Find("Marker(Clone)").gameObject);
            }
            else
            {
                if (isHighlighted)
                    ChangeShaderRPC("Valve/VR/Highlight");
                else
                    ChangeShaderRPC("Standard");
            }


        }
        else
        {
            lista = selectingPlayer.GetComponent<SelectionList>();
            isSelected = lista.AddToList(this.gameObject, lista.selectedList);
            if (isSelected)
            {
                owner = selectingPlayer;
                owner.GetComponent<InputMaster>().ClearSelections += HandleClearSelection;
                if (tag == "Grid")
                {
                    var marker = Resources.Load("Prefabs/Marker", typeof(GameObject));
                    Instantiate(marker, (Vector3.up * 0.3f) + transform.position, transform.rotation, transform);
                    lista.UpdateGrid();
                }
                else
                {
                    ChangeShaderRPC("FX/Flare");
                }
            }

        }
    }



    public void ChangeShaderRPC(String shaderToBe)
    {
        photonView.RPC("ChangeShader", PhotonTargets.AllBufferedViaServer, shaderToBe);
    }

    [PunRPC]
    public void ChangeShader(String shaderToBe)
    {
        Shader tempShader = Shader.Find(shaderToBe);

        if (tempShader)
        {
            if (rend != null)
            {
                int length = rend.materials.Length;
                for (int i = 0; i < length; i++)
                {
                    rend.materials[i].shader = tempShader;
                }
            }
            else if (lineRend != null)
            {
                int length = lineRend.materials.Length;
                for (int i = 0; i < length; i++)
                {
                    lineRend.materials[i].shader = tempShader;
                }
            }
        }
        else
            Debug.Log("Could not find shader: " + shaderToBe);
        //Debug.Log ("Could not change shader to: " + shaderToBe.name); 
    }

}