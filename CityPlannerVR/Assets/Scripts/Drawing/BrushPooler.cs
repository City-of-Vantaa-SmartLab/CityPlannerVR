using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushPooler : MonoBehaviour {

    public GameObject brushEntity;
    public Transform container;
    public List<GameObject> pooledBrushes;
    [SerializeField]
    private int pooledAmount = 100;

    private void Start()
    {
        pooledBrushes = new List<GameObject>();
        GenerateBrushPool();
    }

    private void GenerateBrushPool()
    {
        
        for (int i = 0; i < pooledAmount; i++)
        {
            GameObject obj = Instantiate(brushEntity);
            obj.SetActive(false);
            obj.transform.SetParent(container);
            pooledBrushes.Add(obj);
        }
        
    }

    public GameObject GetPooledObject()
    {
        for(int i = 0; i < pooledBrushes.Count; i++)
        {
            if (!pooledBrushes[i].activeInHierarchy)
            {
                return pooledBrushes[i];
            }
        }
        return null;
    }

    public void ResetPool()
    {
        for(int i = 0; i < pooledBrushes.Count; i++)
        {
            pooledBrushes[i].SetActive(false);
        }
    }
}
