using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Page : MonoBehaviour {

    [Tooltip("Index used to control this page")]
    public int PageIndex = 0;
    [Tooltip("Index of the page we came from")]
    public int previousPage;
    [Tooltip("Indexes of the pages we can go to")]
    public int[] nextPages;
}
