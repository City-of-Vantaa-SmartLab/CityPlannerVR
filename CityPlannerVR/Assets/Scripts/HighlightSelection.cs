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

//TODO: booleans toggleHighlight and toggleSelect are only for debugging purposes
//TODO?: add sound to selection function
//TODO: Instead of changing shaders, change materials


public class HighlightSelection : PunBehaviour
{

    //private Shader diffuse; 
    private Shader standard;
    private Shader highlight;
    private Shader selected;
    private Renderer rend;
    private XRLineRenderer lineRend;



    public bool isHighlighted;
    public bool isSelected;

    private GameObject owner;
    [SerializeField]
    private SelectionList lista;

    void Start()
    {
        isHighlighted = false;
        isSelected = false;

        standard = Shader.Find("Standard");
        highlight = Shader.Find("Valve/VR/Highlight");
        selected = Shader.Find("FX/Flare");
        rend = this.GetComponent<MeshRenderer>();
        lineRend = this.GetComponent<XRLineRenderer>();

    }


    public void ToggleHighlight()
    {
        if (isHighlighted)
        {
            isHighlighted = false;
            //priorize selection shader over highlight 
            if (!isSelected)
                ChangeShader(standard);

        }
        else
        {
            isHighlighted = true;
            if (!isSelected)
                ChangeShader(highlight);
        }

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
            owner.GetComponent<InputListener>().LasersAreOff -= HandleLasersOff;  //releases ownership of selected item
            owner = null;
            lista.RemoveFromList(this.gameObject, lista.selectedList);
            if (tag == "Grid")
            {
                Destroy(transform.Find("Marker(Clone)").gameObject);
            }
            else
            {
                if (isHighlighted)
                    ChangeShader(highlight);
                else
                    ChangeShader(standard);
            }


        }
        else
        {
            lista = selectingPlayer.GetComponent<SelectionList>();
            isSelected = lista.AddToList(this.gameObject, lista.selectedList);
            if (isSelected)
            {
                owner = selectingPlayer;
                owner.GetComponent<InputListener>().LasersAreOff += HandleLasersOff;
                if (tag == "Grid")
                {
                    var marker = Resources.Load("Prefabs/Marker", typeof(GameObject));
                    Instantiate(marker, (Vector3.up * 0.3f) + transform.position, transform.rotation, transform);
                    lista.UpdateGrid();
                }
                else
                {
                    ChangeShader(selected);
                }
            }

        }
    }

    private void HandleLasersOff(object sender, ClickedEventArgs e)
    {
        ToggleSelection(owner);
    }

    public void ChangeShader(Shader shaderToBe)
    {
        if (rend != null)
        {
            int length = rend.materials.Length;
            for (int i = 0; i < length; i++)
            {
                rend.materials[i].shader = shaderToBe;
            }
        }
        else if (lineRend != null)
        {
            int length = lineRend.materials.Length;
            for (int i = 0; i < length; i++)
            {
                lineRend.materials[i].shader = shaderToBe;
            }
        }
        //Debug.Log ("Could not change shader to: " + shaderToBe.name); 
    }

}