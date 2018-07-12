using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeFacade : MonoBehaviour {

    /// <summary>
    /// List of all the possible facades for a object
    /// </summary>
    public static GameObject[] facades;
    public static int facadeIndex = 0;
    public static bool canChangeFacade = false;

    /// <summary>
    /// Used to scroll facades
    /// </summary>
    public void ScrollFacades()
    {
        if (canChangeFacade)
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
    }

    /// <summary>
    /// Change the facade of the building selected
    /// </summary>
    public void FacadeChange()
    {
        if (canChangeFacade) {
            for (int i = 0; i < facades.Length; i++)
            {
                if (i == facadeIndex)
                {
                    facades[i].SetActive(true);
                }
                else
                {
                    facades[i].SetActive(false);
                }
            }
        }
    }
}
