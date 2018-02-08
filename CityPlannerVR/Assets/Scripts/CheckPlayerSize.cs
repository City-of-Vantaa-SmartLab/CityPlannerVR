﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPlayerSize : MonoBehaviour {

    //are we small
    bool isInPedesrianMode = false;
    public bool isSmall
    {
        get
        {
            CalculateMode();
            return isInPedesrianMode;
        }
    }

    void CalculateMode()
    {
        //hardcoded values for now
        if (transform.localScale == new Vector3(0.025f, 0.025f, 0.025f))        {
            isInPedesrianMode = true;
        }
        else if (transform.localScale == new Vector3(1, 1, 1))
        {
            isInPedesrianMode = false;
        }
        else
        {
            Debug.LogError("Player scale changed and forgotten to change CheckPlayerSize values");
        }
    }
}
