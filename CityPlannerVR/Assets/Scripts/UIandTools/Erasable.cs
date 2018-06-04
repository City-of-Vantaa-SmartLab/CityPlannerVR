using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Erasable : MonoBehaviour {

    //Collider myCollider;
    [SerializeField]
    private List<Eraser> terminators;


    void Start() {
        terminators = new List<Eraser>();
        //myCollider = gameObject.GetComponent<Collider>();
    }

    private void OnDestroy()
    {
        UnsubscribeAll();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Eraser"))
        {
            Eraser tryEraser;
            tryEraser = other.GetComponent<Eraser>();
            if (tryEraser)
                BeginHovering(tryEraser);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Eraser"))
        {
            Eraser tryEraser;
            tryEraser = other.GetComponent<Eraser>();
            if (tryEraser)
                EndHovering(tryEraser);
        }
    }

    private void Subscribe(Eraser eraser)
    {
        eraser.DestroyObjects += HandleDestroyObjects;
        eraser.RemoveFromList += HandleRemoveFromList;
    }

    private void Unsubscribe(Eraser eraser)
    {
        eraser.DestroyObjects -= HandleDestroyObjects;
        eraser.RemoveFromList -= HandleRemoveFromList;
    }

    private void UnsubscribeAll()
    {
        foreach(Eraser eraser in terminators)
        {
            Unsubscribe(eraser);
        }
        terminators.Clear();
    }



    private void HandleDestroyObjects(uint deviceIndex, Eraser eraser)
    {
        //Debug.Log("Reveived event: destroy objects");
        Destroy(gameObject, 0.1f);
    }

    private void HandleRemoveFromList(uint deviceIndex, Eraser eraser)
    {
        //Debug.Log("Reveived event: remove from list");
        Unsubscribe(eraser);
    }

    public void BeginHovering(Eraser eraser)
    {
        if (eraser == null)
        {
            Debug.Log("Cannot add to terminators, eraser is null!");
            return;
        }

        if (!terminators.Contains(eraser))
        {
            //Debug.Log("Adding "+ eraser.name + " to terminators on object: " + this.name);
            terminators.Add(eraser);
            Subscribe(eraser);
        }
        //else
            //Debug.Log("eraser already in the list");
    }

    private void EndHovering(Eraser eraser)
    {
        if (eraser == null)
        {
            Debug.Log("Cannot remove from terminators, eraser is null!");
            return;
        }

        if (terminators.Contains(eraser))
        {
            //Debug.Log("Removing " + eraser.name + " from terminators on object: " + this.name);
            terminators.Remove(eraser);
            Unsubscribe(eraser);
        }
        //else
        //    Debug.Log("eraser already in the list");
    }

}
