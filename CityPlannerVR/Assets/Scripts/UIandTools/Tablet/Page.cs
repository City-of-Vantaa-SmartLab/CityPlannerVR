using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores some information that every page has
/// </summary>

public class Page : MonoBehaviour {

    /// <summary> Index used to control a page </summary>
    [Tooltip("Index used to control a page")]
    public int PageIndex = 0;
    /// <summary> Index of the page we came from </summary>
    [Tooltip("Index of the page we came from")]
    public int previousPage;
    /// <summary> Indexes of the pages we can go to </summary>
    [Tooltip("Indexes of the pages we can go to")]
    public int[] nextPages;
}
