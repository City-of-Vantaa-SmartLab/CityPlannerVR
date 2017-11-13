using UnityEngine;
using UnityEngine.Networking;

public class ColorSetter : NetworkBehaviour
{

    [SyncVar]
    private Color playerColor;

    public override void OnStartClient()
    {
        if (isServer)
        {
            //Set player color to Random color
            playerColor = Color.HSVToRGB(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1);
        }
        //Change player color
        gameObject.GetComponent<Renderer>().material.color = playerColor;
    }
}