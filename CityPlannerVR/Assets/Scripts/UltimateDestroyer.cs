using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimateDestroyer : MonoBehaviour {

    private GameObject prop;

    private void OnCollisionEnter(Collision other)

    {
        
        PhotonNetwork.Destroy(other.gameObject);
        
    }
}

