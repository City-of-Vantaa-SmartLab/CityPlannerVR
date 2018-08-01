using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePage : MonoBehaviour {

    [Tooltip("The index of the page this button loads")]
    public int nextPageIndex;

    private HoverTabletManager hoverTablet;
    private Page page;
    private int previousPage;
    private int[] nextPages;

    private void Start()
    {
        hoverTablet = GetComponentInParent<HoverTabletManager>();
        page = GetComponentInParent<Page>();
        previousPage = page.previousPage;
        nextPages = page.nextPages;
    }

    /// <summary> Gives the HoverTabletManager the index of the next page when button is pressed </summary>
    public void ChangePageToNext()
    {
        if (nextPages.Contains(nextPageIndex))
        {
            hoverTablet.PageIndex = nextPageIndex;
        }
        else
        {
            Debug.LogError("Next page index not found");
        }
    }

    /// <summary> Gives the HoverTabletManager the index of the previous page when button is pressed </summary>
    public void ChangePageToPrevious()
    {
        hoverTablet.PageIndex = previousPage;
    }
}
