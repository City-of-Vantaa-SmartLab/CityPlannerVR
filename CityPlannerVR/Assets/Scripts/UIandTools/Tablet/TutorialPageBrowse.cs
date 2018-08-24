using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPageBrowse : MonoBehaviour {

    public GameObject[] tutorialPages;
    int openPageIndex = 0;

    public int OpenPageIndex
    {
        get
        {
            return openPageIndex;
        }

        set
        {
            tutorialPages[openPageIndex].SetActive(false);
            openPageIndex = value;
            tutorialPages[openPageIndex].SetActive(true);
        }
    }

    private void Start()
    {
        for (int i = 0; i < tutorialPages.Length; i++)
        {
            if(i == 0)
            {
                tutorialPages[i].SetActive(true);
            }
            else
            {
                tutorialPages[i].SetActive(false);
            }
        }
    }

    public void ChangePageRight()
    {
        if(openPageIndex == tutorialPages.Length - 1)
        {
            openPageIndex = 0;
            tutorialPages[tutorialPages.Length - 1].SetActive(false);
        }
        else
        {
            openPageIndex++;
            tutorialPages[openPageIndex - 1].SetActive(false);
        }

        tutorialPages[openPageIndex].SetActive(true);
        
    }

    public void ChangePageLeft()
    {
        if (openPageIndex == 0)
        {
            openPageIndex = tutorialPages.Length - 1;
            tutorialPages[0].SetActive(false);
        }
        else
        {
            openPageIndex--;
            tutorialPages[openPageIndex + 1].SetActive(false);
        }

        tutorialPages[openPageIndex].SetActive(true);
    }
}
