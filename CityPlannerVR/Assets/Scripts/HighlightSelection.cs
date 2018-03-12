using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 
/// Attached gameobject can be selected and highlighted using hardcoded shaders. 
/// Selection also utilises SelectionList script, adding attached object to GameController object's selected list. 
/// </summary> 

//TODO: booleans toggleHighlight and toggleSelect are only for debugging purposes, 
//  therefore they will be removed with the update function later 
//TODO?: add sound to selection function 


public class HighlightSelection : MonoBehaviour
{

    //private Shader diffuse; 
    private Shader standard;
    private Shader highlight;
    private Shader selected;
    private Renderer rend;
    private XRLineRenderer lineRend;



    public bool isHighlighted;
    //  public bool toggleHighlight; 
    public bool isSelected;
    //  public bool toggleSelect; 

    private GameObject gameController;
    private SelectionList lista;


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


    public void ToggleSelection()
    {
        if (isSelected)
        {
            gameController.GetComponent<SelectionList>().RemoveFromList(this.gameObject, lista.selectedList);
            isSelected = false;
            if (isHighlighted)
                ChangeShader(highlight);
            else
                ChangeShader(standard);


        }
        else
        {
            isSelected = true;
            ChangeShader(selected);
            gameController.GetComponent<SelectionList>().AddToList(this.gameObject, lista.selectedList);
        }
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


    void Start()
    {
        isHighlighted = false;
        //    toggleHighlight = false; 
        isSelected = false;
        //    toggleSelect = false; 

        standard = Shader.Find("Standard");
        highlight = Shader.Find("Valve/VR/Highlight");
        selected = Shader.Find("FX/Flare");
        rend = this.GetComponent<MeshRenderer>();
        lineRend = this.GetComponent<XRLineRenderer>();

        gameController = GameObject.FindGameObjectWithTag("GameController");
        lista = gameController.GetComponent<SelectionList>();


    }

    //  //Will be removed! 
    //  void Update() { 
    //    if (toggleHighlight) { 
    //      ToggleHighlight (); 
    //      toggleHighlight = false; 
    //    } 
    // 
    //    if (toggleSelect) { 
    //      ToggleSelection (); 
    //      toggleSelect = false; 
    // 
    // 
    //    } 
    // 
    //  } 

}