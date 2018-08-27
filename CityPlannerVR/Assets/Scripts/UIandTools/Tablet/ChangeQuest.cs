using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeQuest : MonoBehaviour {

    public GameObject[] QuestObjects;
    GameObject QuestObject;
    GameObject[] Quests;
    int currentQuestIndex = 0;

    PhotonGameManager gameManager;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<PhotonGameManager>();

        QuestObject = QuestObjects[gameManager.GetMissionValue()];
        QuestObject.SetActive(true);

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
        List<string> info = new List<string>();
        info.Add("QuestIndex");
        info.Add(currentQuestIndex.ToString());

        GameObject.Find("GameManager").GetComponent<Logger>().LogActionLine("NextQuestOpened", info);
    }
}
