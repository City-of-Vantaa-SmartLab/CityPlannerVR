using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof(MeshCombiner))]
public class MeshCombineEditor : Editor {

	void OnSceneGUI()
    {
        MeshCombiner mc = target as MeshCombiner;

        if (Handles.Button(Vector3.zero, Quaternion.identity, 1, 1, Handles.CylinderCap))
        {
            mc.AdvancedMerge();
        }
    }
}
