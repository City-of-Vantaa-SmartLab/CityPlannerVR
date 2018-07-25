using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDestroyer : MonoBehaviour {

    private GameObject prop;
    private PhotonNetworkedObject propN;
    private IEnumerator itemdestroy;


    private void OnCollisionEnter(Collision other)
   
    {
        Debug.Log(other.gameObject.name + " hit floor");
        prop = other.gameObject;
        propN = prop.GetComponent<PhotonNetworkedObject>();

        
        if (prop.CompareTag("Spawnable"))
        {
            itemdestroy = propN.ObjectDestroyCounter();
            propN.StartCoroutine(itemdestroy);
        }
    }

    private void OnCollisionExit(Collision other)
    {
        Debug.Log(prop.name + " exited floor");
        if (prop.CompareTag("Spawnable"))
        {
            propN.StopCoroutine(itemdestroy);
        }
    }
}
