using UnityEngine;
using UnityEngine.Networking;

public class DestroyNetworkedObject : NetworkBehaviour{

    [SerializeField]
    private string objectTag = "Spawnable";

    private void OnEnterTrigger(Collider other)
    {
        if(other.tag == objectTag)
        {
            Debug.Log("Object " + other.gameObject.name + " destroyed.");
            NetworkServer.Destroy(other.gameObject);
        }
    }
}
