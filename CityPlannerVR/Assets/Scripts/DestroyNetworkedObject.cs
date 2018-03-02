using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Description: Destroys networked object on collision
/// TODO: Works, but somehow an object with this script (NetworkedObjectDestroyer) caused desync problems. Investigate
/// </summary>

public class DestroyNetworkedObject : NetworkBehaviour{

    [SerializeField]
    private string objectTag = "Spawnable";

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(objectTag))
        {
            //Debug.Log("Object " + other.gameObject.name + " destroyed.");
            NetworkServer.Destroy(other.gameObject);
        }
    }
}
