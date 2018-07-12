using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeFacade : MonoBehaviour {

    [Tooltip("List of all the possible facades for this object")]
    public GameObject[] facades;
    int facadeIndex = 0;
    /// <summary>
    /// The facade that needs to be disabled when new one is activated (the old facade)
    /// </summary>
    GameObject previousFacade;

    private void Start()
    {
        previousFacade = facades[facadeIndex];
        foreach (var facade in facades)
        {
            facade.SetActive(false);
        }
    }

    /// <summary>
    /// Used to scroll facades
    /// </summary>
    public void ScrollFacades()
    {
        if (facades.Length > 0)
        {
            if (facadeIndex == facades.Length - 1)
            {
                facadeIndex = 0;
            }
            else
            {
                ++facadeIndex;
            }
        }
    }

    void FacadeChange()
    {
        previousFacade.SetActive(false);
        facades[facadeIndex].SetActive(true);
        previousFacade = facades[facadeIndex];
    }
}
