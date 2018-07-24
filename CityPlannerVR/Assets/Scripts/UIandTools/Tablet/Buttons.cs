using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buttons : MonoBehaviour {

    /// <summary>
    /// This buttons parent page
    /// </summary>
    private Page parentPage;

    private void Start()
    {
        parentPage = GetComponentInParent<Page>();
    }

    public void MoveForward()
    {

    }

    public void MoveBack()
    {
       
    }
}
