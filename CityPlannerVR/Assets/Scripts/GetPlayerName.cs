using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetPlayerName : MonoBehaviour {

    private Text text;
    public GameObject head;

	// Use this for initialization
	void Start () {
        text = GetComponentInChildren<Text>();

        text.text = PhotonPlayerAvatar.LocalPlayerInstance.GetComponent<PhotonView>().owner.NickName;
    }

    void Update()
    {
		//														add 180 to flip the label to face the correct direction
		gameObject.transform.localEulerAngles = new Vector3 (0, head.transform.localEulerAngles.y + 180f, 0);
    }
	
}
