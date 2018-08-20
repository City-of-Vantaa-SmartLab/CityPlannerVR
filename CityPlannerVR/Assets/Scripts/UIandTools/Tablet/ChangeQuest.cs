using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeQuest : MonoBehaviour {

    public GameObject QuestObject;
    GameObject[] Quests;
    int currentQuestIndex = 0;

    private void Start()
    {
        Quests = new GameObject[QuestObject.transform.childCount];

        for (int i = 0; i < Quests.Length; i++)
        {
            Quests[i] = QuestObject.transform.GetChild(i).gameObject;
        }
    }

    public void NextQuest()
    {
        if(currentQuestIndex == Quests.Length - 1)
        {
            return;
        }
        currentQuestIndex++;
        Quests[currentQuestIndex - 1].SetActive(false);
        Quests[currentQuestIndex].SetActive(true);
    }
}
