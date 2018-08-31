﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Give this objects name and facades to the tablet
/// </summary>
public class SetName : MonoBehaviour {

    public delegate void ChangeColor();
    public static event ChangeColor OnChangeColor;

    /// <summary>
    /// List of all the possible facades for this object
    /// </summary>
    public GameObject[] facades;
    int facadeIndex = 0;

    private void Start()
    {
        if(facades.Length > 0)
        {
            for (int i = 0; i < facades.Length; i++)
            {
                if (i > 0)
                {
                    facades[i].SetActive(false);
                }
            }
        }
    }

    public void GiveNameAndFacades()
    {
        HoverTabletManager.CommentTarget = gameObject;

        //[xxx]
        //if (facades.Length > 0)
        //{
        //    CheckIndex();
        //    //Give the facades to the button
        //    ChangeFacade.facades = facades;
        //    ChangeFacade.facadeIndex = facadeIndex;
        //    ChangeFacade.canChangeFacade = true;
        //}
        //else
        //{
        //    ChangeFacade.facades = null;
        //    ChangeFacade.facadeIndex = 0;
        //    ChangeFacade.canChangeFacade = false;
        //}
        //[xxx]

        if (OnChangeColor != null)
        {
            OnChangeColor();
        }
    }

    void CheckIndex()
    {
        for (int i = 0; i < facades.Length; i++)
        {
            if(facades[i].GetActive() == true)
            {
                facadeIndex = i;
                break;
            }
        }
    }
}
