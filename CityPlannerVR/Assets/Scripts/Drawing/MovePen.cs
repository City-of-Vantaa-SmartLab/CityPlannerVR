using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePen : MonoBehaviour {

    public float speed = 0.5f;

	void Update () {
        transform.Translate(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * Time.deltaTime * speed);
        if (Input.GetKey(KeyCode.RightShift))
        {
            transform.Translate(-Vector3.forward * speed * 0.1f);
        }
        if (Input.GetKey(KeyCode.RightControl))
        {
            transform.Translate(Vector3.forward * speed * 0.1f);
        }
    }
}
