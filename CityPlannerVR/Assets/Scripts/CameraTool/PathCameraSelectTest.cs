using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCameraSelectTest : MonoBehaviour {

	public ToolManager toolManager;

	void OnTriggerEnter(Collider other){
		toolManager.Tool = ToolManager.ToolType.PathCamera;
	}
}
