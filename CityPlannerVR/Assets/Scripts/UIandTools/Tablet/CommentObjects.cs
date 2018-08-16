using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommentObjects : MonoBehaviour {

    HoverTabletManager hoverTablet;

    //[Tooltip("The index of the page this button loads")]
    readonly int nextPageIndex = 50;

    private void Start()
    {
        hoverTablet = GameObject.Find("Hover_tablet_prefab").GetComponent<HoverTabletManager>();
    }

    public void ChangeToObjectPage()
    {
        if(nextPageIndex == 0)
        {
            Debug.Log("NextPageIndex is 0");
        }
        else
        {
            hoverTablet.PageIndex = nextPageIndex;
        }
    }
}
