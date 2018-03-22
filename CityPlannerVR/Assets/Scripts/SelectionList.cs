//TODO: gridtile.containedobject untested 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class SelectionList : MonoBehaviour
{

    public List<GameObject> selectedList;
    private CreateGrid gridParent;
    [SerializeField]
    private bool onlyGrid;

    public bool AddToList(GameObject go, List<GameObject> list)
    {
        if (ContainsGameObject(go, list))
        {
            Debug.Log("The list already contains: " + go.name);
            return true;
        }
        else
        {
            if (list.Count == 0)
            {
                if (go.CompareTag("Grid"))
                {
                    onlyGrid = true;
                }
                else
                    onlyGrid = false;

                list.Add(go);
                return true;
            }

            if (onlyGrid && go.CompareTag("Grid"))
            {
                list.Add(go);
                return true;
            }
            else if (!onlyGrid && !go.CompareTag("Grid"))
            {
                list.Add(go);
                return true;
            }
            else
            {
                if (go.CompareTag("Grid"))
                {
                    Debug.Log("Cannot add gridtile to a non-grid list!");
                }
                else if (onlyGrid)
                {
                    Debug.Log("Cannot add object to a gridlist!");
                }
                else
                    Debug.Log("Could not add object!");
                return false;
            }
        }
    }

    public void RemoveFromList(GameObject go, List<GameObject> list)
    {
        if (ContainsGameObject(go, list))
        {
            list.Remove(go);

        }
        else
            Debug.Log(go.name + " not found!");
    }

    public GameObject GetObj(int index)
    {
        GameObject go = selectedList[index];
        if (go == null)
            Debug.Log("Object could not be find using index: " + index);
        return go;
    }

    public void DeSelectAll(List<GameObject> list)
    {
        //foreach (GameObject go in list)
        //{
        //    RemoveFromList(go, list);
        //}

        list.Clear();

    }

    public void ChangeSelectionToList(List<GameObject> newSelectionsList, List<GameObject> list)
    {
        DeSelectAll(list);
        AddListToSelections(newSelectionsList, list);
    }

    public void AddListToSelections(List<GameObject> addedList, List<GameObject> list)
    {
        foreach (GameObject go in addedList)
            AddToList(go, list);
    }

    bool ContainsGameObject(GameObject go, List<GameObject> list)
    {
        return list.Contains(go);
    }

    //TODO: Returns objects within an area that have the appropriate tag 
    public void GetObjectsInArea(GridTile grid1, GridTile grid2, string tag, List<GameObject> list)
    {
        List<GameObject> buildingList = new List<GameObject>();
        GridTile tempTile = null;
        float xStart, zStart, xEnd, zEnd;

        if (grid1.xPos < grid2.xPos)
        {
            xStart = grid1.xPos;
            xEnd = grid2.xPos;
        }
        else
        {
            xStart = grid2.xPos;
            xEnd = grid1.xPos;
        }

        if (grid1.zPos < grid2.zPos)
        {
            zStart = grid1.zPos;
            zEnd = grid2.zPos;
        }
        else
        {
            zStart = grid2.zPos;
            zEnd = grid1.zPos;
        }


        for (float i = zStart; i <= zEnd; i++)
        {

            for (float j = xStart; j <= xEnd; j++)
            {
                tempTile = gridParent.GetTileAt(j, i);
                if (tempTile.State == GridTile.GridState.Empty)
                    continue;

                if (tempTile.containedObject.tag == "Building")
                    AddToList(tempTile.containedObject, list);


            }

            if (list.Count == 0)
            {
                Debug.Log("Could not find objects!");
            }

        }

    }

    public void UpdateGrid()
    {
        GameObject previousMarker = null;
        GameObject newMarker = null;

        foreach (GameObject go in selectedList)
        {
            newMarker = go.transform.Find("Marker(Clone)").gameObject;
            if (newMarker == null)
            {
                Debug.Log("Could not find marker of " + go.name);
                continue;
            }

            if (previousMarker == null)
            {
                previousMarker = newMarker;
                continue;
            }

            if (!AddLine(previousMarker, newMarker))
            {
                Debug.Log("Could not add line from" + go.name);
            }

            previousMarker = newMarker;
        }

        previousMarker = GetObj(selectedList.Count - 1);
        previousMarker = previousMarker.transform.Find("Marker(Clone)").gameObject;

        newMarker = GetObj(0);
        newMarker = newMarker.transform.Find("Marker(Clone)").gameObject;

        newMarker.GetComponent<HighlightLine>().alreadySet = false;
        if (!AddLine(previousMarker, newMarker))
        {
            Debug.Log("Could not add a line between first and last marker!");
        }

        
    }


    bool AddLine(GameObject go1, GameObject go2)
    {
        HighlightLine newLine = go2.GetComponent<HighlightLine>();
        if (newLine == null)
        {
            Debug.Log("Could not find newline component of marker in " + go2.name);
            return false;
        }

        if (!newLine.alreadySet)
        {
            newLine.go1 = go1;
            newLine.go2 = go2;
            newLine.UpdateLine();
        }
        return true;
    }


    // Use this for initialization 
    void Start()
    {
        selectedList = new List<GameObject>();
        gridParent = GameObject.FindGameObjectWithTag("GridParent").GetComponent<CreateGrid>();
        onlyGrid = false;
    }

}