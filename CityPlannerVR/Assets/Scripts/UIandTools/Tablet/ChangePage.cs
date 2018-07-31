using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePage : MonoBehaviour {

    [Tooltip("The index of the page this button loads")]
    public int nextPageIndex;

    private Page page;
    private int previousPage;
    private int[] nextPages;

    private void Start()
    {
        page = GetComponentInParent<Page>();
        previousPage = page.previousPage;
        nextPages = page.nextPages;
    }

    public void ChangePageWhenPressed()
    {
        if (nextPages.Contains(nextPageIndex))
        {

        }
    }
}
