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

    /// <summary>
    /// The index of a page in an array
    /// </summary>
    public int currentlyActivePageIndex = 0;
    /// <summary>
    /// The index of a page given to it
    /// </summary>
    private int pageIndex;
    public int PageIndex
    {
        get
        {
            return pageIndex;
        }
        set
        {
            pageIndex = value;
            ChangePage();
        }
    }

    private void Start()
    {
        pagesCanvas = GameObject.Find("PagesCanvas");
        childCount = pagesCanvas.transform.childCount;
        pages = new Page[childCount];
        GetAllPages();
        ActivateAndDeactivatePages();
    }

    /// <summary> Finds all the pages below the PagesCanvas </summary>
    private void GetAllPages()
    {
        for (int i = 0; i < childCount; i++)
        {
            pages[i] = pagesCanvas.transform.GetChild(i).GetComponent<Page>();
        }
    }

    private void ActivateAndDeactivatePages()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            if (i == currentlyActivePageIndex)
            {
                pages[i].gameObject.SetActive(true);
            }
            else
            {
                pages[i].gameObject.SetActive(false);
            }
        }
    }

    private void ChangePage()
    {
        GetPageIndex();
        ActivateAndDeactivatePages();
    }

    private void GetPageIndex()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            //Check the index of the page in the list
            if (pages[i].PageIndex == pageIndex)
            {
                currentlyActivePageIndex = i;
                break;
            }
        }
    }
}
