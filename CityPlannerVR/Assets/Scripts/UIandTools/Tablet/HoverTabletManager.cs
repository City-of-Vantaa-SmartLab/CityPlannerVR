using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the hover tablet
/// </summary>
public class HoverTabletManager : MonoBehaviour {

    /// <summary> The target we want to comment or do something else with </summary>
    public static GameObject commentTarget;
    /// <summary> Index of currently open page </summary>
    public static int openPage;

    /// <summary> The parent of all pages </summary>
    private GameObject pagesCanvas;
    /// <summary> Reference to all the pages are stored here </summary>
    private Page[] pages;
    /// <summary> The amount of pages </summary>
    private int childCount;

    private void Start()
    {
        pagesCanvas = GameObject.Find("PagesCanvas");
        childCount = pagesCanvas.transform.childCount;
        pages = new Page[childCount];
        GetAllPages();
        //AssignPageIndexes();
        DeactivatePages();
    }

    /// <summary> Finds all the pages below the PagesCanvas </summary>
    void GetAllPages()
    {
        for (int i = 0; i < childCount; i++)
        {
            pages[i] = pagesCanvas.transform.GetChild(i).GetComponent<Page>();
        }
    }

    void DeactivatePages()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            if (i != 2)
            {
                pages[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary> Finds and assigns an unic index to pages of the tablet </summary>
    void AssignPageIndexes()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].PageIndex = i;
            //close all other pages except the first one
            if(i != 2)
            {
                pages[i].gameObject.SetActive(false);
            }
        }
    }
}
